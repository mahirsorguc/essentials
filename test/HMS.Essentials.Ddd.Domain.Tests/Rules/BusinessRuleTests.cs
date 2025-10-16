using HMS.Essentials.Domain.Rules;
using Shouldly;

namespace HMS.Essentials.Ddd.Domain.Tests.Rules;

public class BusinessRuleTests
{
    [Fact]
    public void BusinessRule_IsBroken_ShouldReturnCorrectValue()
    {
        // Arrange
        var validRule = new TestBusinessRule(false);
        var brokenRule = new TestBusinessRule(true);

        // Act & Assert
        validRule.IsBroken().ShouldBeFalse();
        brokenRule.IsBroken().ShouldBeTrue();
    }

    [Fact]
    public void BusinessRule_ShouldHaveMessage()
    {
        // Arrange
        var rule = new TestBusinessRule(true);

        // Act & Assert
        rule.Message.ShouldNotBeNullOrEmpty();
        rule.Message.ShouldBe("Test business rule is broken");
    }

    [Fact]
    public void BusinessRuleValidationException_ShouldContainBrokenRule()
    {
        // Arrange
        var rule = new TestBusinessRule(true);

        // Act
        var exception = new BusinessRuleValidationException(rule);

        // Assert
        exception.BrokenRule.ShouldBe(rule);
        exception.Message.ShouldBe(rule.Message);
        exception.Details.ShouldBe(rule.Message);
    }

    [Fact]
    public void BusinessRuleValidationException_WithDetails_ShouldContainCustomDetails()
    {
        // Arrange
        var rule = new TestBusinessRule(true);
        var customDetails = "Custom validation details";

        // Act
        var exception = new BusinessRuleValidationException(rule, customDetails);

        // Assert
        exception.BrokenRule.ShouldBe(rule);
        exception.Message.ShouldBe(rule.Message);
        exception.Details.ShouldBe(customDetails);
    }

    [Fact]
    public void BusinessRuleValidationException_ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var rule = new TestBusinessRule(true);
        var exception = new BusinessRuleValidationException(rule);

        // Act
        var result = exception.ToString();

        // Assert
        result.ShouldContain("TestBusinessRule");
        result.ShouldContain(rule.Message);
    }

    [Fact]
    public void BusinessRule_WithComplexValidation_ShouldWork()
    {
        // Arrange
        var rule = new AmountMustBePositiveRule(-10);

        // Act & Assert
        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Amount must be positive");
    }

    [Fact]
    public void BusinessRule_WithParameters_ShouldEvaluateCorrectly()
    {
        // Arrange
        var validRule = new AmountMustBePositiveRule(100);
        var brokenRule = new AmountMustBePositiveRule(-50);

        // Act & Assert
        validRule.IsBroken().ShouldBeFalse();
        brokenRule.IsBroken().ShouldBeTrue();
    }

    [Fact]
    public void BusinessRule_CanBeUsedInDomainLogic()
    {
        // Arrange
        var customer = new TestCustomer();
        var rule = new CustomerMustBeActiveRule(customer);

        // Act
        customer.Deactivate();

        // Assert
        rule.IsBroken().ShouldBeTrue();
    }

    [Fact]
    public void ThrowingBusinessRuleException_ShouldWorkInRealScenario()
    {
        // Arrange
        var customer = new TestCustomer();
        customer.Deactivate();

        // Act & Assert
        var exception = Should.Throw<BusinessRuleValidationException>(() => 
            customer.PlaceOrder());

        exception.BrokenRule.ShouldBeOfType<CustomerMustBeActiveRule>();
        exception.Message.ShouldBe("Customer must be active to place orders");
    }

    [Fact]
    public void MultipleBusinessRules_CanBeValidated()
    {
        // Arrange
        var customer = new TestCustomer();
        customer.Deactivate();

        var rules = new List<IBusinessRule>
        {
            new CustomerMustBeActiveRule(customer),
            new AmountMustBePositiveRule(100)
        };

        // Act
        var brokenRules = rules.Where(r => r.IsBroken()).ToList();

        // Assert
        brokenRules.Count.ShouldBe(1);
        brokenRules[0].ShouldBeOfType<CustomerMustBeActiveRule>();
    }

    // Test business rules
    private class TestBusinessRule : IBusinessRule
    {
        private readonly bool _isBroken;

        public TestBusinessRule(bool isBroken)
        {
            _isBroken = isBroken;
        }

        public string Message => "Test business rule is broken";

        public bool IsBroken() => _isBroken;
    }

    private class AmountMustBePositiveRule : IBusinessRule
    {
        private readonly decimal _amount;

        public AmountMustBePositiveRule(decimal amount)
        {
            _amount = amount;
        }

        public string Message => "Amount must be positive";

        public bool IsBroken() => _amount <= 0;
    }

    private class CustomerMustBeActiveRule : IBusinessRule
    {
        private readonly TestCustomer _customer;

        public CustomerMustBeActiveRule(TestCustomer customer)
        {
            _customer = customer;
        }

        public string Message => "Customer must be active to place orders";

        public bool IsBroken() => !_customer.IsActive;
    }

    // Test domain entity
    private class TestCustomer
    {
        public bool IsActive { get; private set; } = true;

        public void Deactivate()
        {
            IsActive = false;
        }

        public void PlaceOrder()
        {
            var rule = new CustomerMustBeActiveRule(this);
            if (rule.IsBroken())
            {
                throw new BusinessRuleValidationException(rule);
            }

            // Order placement logic...
        }
    }
}
