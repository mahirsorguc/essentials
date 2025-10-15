using HMS.Essentials.Swashbuckle.Configuration;
using Shouldly;

namespace HMS.Essentials.Swashbuckle.Tests.Configuration;

/// <summary>
/// Tests for SwaggerOptions configuration class.
/// </summary>
public class SwaggerOptionsTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var options = new SwaggerOptions();

        // Assert
        options.EnableSwaggerUI.ShouldBeTrue();
        options.EnableInProduction.ShouldBeFalse();
        options.Title.ShouldBe("API Documentation");
        options.Version.ShouldBe("v1");
        options.Description.ShouldBeNull();
        options.TermsOfServiceUrl.ShouldBeNull();
        options.ContactName.ShouldBeNull();
        options.ContactEmail.ShouldBeNull();
        options.ContactUrl.ShouldBeNull();
        options.LicenseName.ShouldBeNull();
        options.LicenseUrl.ShouldBeNull();
        options.IncludeXmlComments.ShouldBeTrue();
        options.RoutePrefix.ShouldBe("swagger");
        options.EnableAnnotations.ShouldBeTrue();
        options.CustomCss.ShouldBeNull();
        options.DocumentTitle.ShouldBeNull();
        options.EnableDeepLinking.ShouldBeTrue();
        options.DisplayOperationId.ShouldBeTrue();
        options.DisplayRequestDuration.ShouldBeTrue();
        options.DefaultModelsExpandDepth.ShouldBe(1);
        options.DefaultModelExpandDepth.ShouldBe(1);
        options.ShowExtensions.ShouldBeFalse();
        options.ShowCommonExtensions.ShouldBeFalse();
    }

    [Fact]
    public void EnableSwaggerUI_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.EnableSwaggerUI = false;

        // Assert
        options.EnableSwaggerUI.ShouldBeFalse();
    }

    [Fact]
    public void EnableInProduction_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.EnableInProduction = true;

        // Assert
        options.EnableInProduction.ShouldBeTrue();
    }

    [Fact]
    public void Title_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var title = "My Custom API";

        // Act
        options.Title = title;

        // Assert
        options.Title.ShouldBe(title);
    }

    [Fact]
    public void Version_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var version = "v2";

        // Act
        options.Version = version;

        // Assert
        options.Version.ShouldBe(version);
    }

    [Fact]
    public void Description_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var description = "This is my API description";

        // Act
        options.Description = description;

        // Assert
        options.Description.ShouldBe(description);
    }

    [Fact]
    public void TermsOfServiceUrl_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var url = "https://example.com/terms";

        // Act
        options.TermsOfServiceUrl = url;

        // Assert
        options.TermsOfServiceUrl.ShouldBe(url);
    }

    [Fact]
    public void ContactName_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var name = "API Support";

        // Act
        options.ContactName = name;

        // Assert
        options.ContactName.ShouldBe(name);
    }

    [Fact]
    public void ContactEmail_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var email = "support@example.com";

        // Act
        options.ContactEmail = email;

        // Assert
        options.ContactEmail.ShouldBe(email);
    }

    [Fact]
    public void ContactUrl_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var url = "https://example.com/contact";

        // Act
        options.ContactUrl = url;

        // Assert
        options.ContactUrl.ShouldBe(url);
    }

    [Fact]
    public void LicenseName_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var license = "MIT";

        // Act
        options.LicenseName = license;

        // Assert
        options.LicenseName.ShouldBe(license);
    }

    [Fact]
    public void LicenseUrl_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var url = "https://opensource.org/licenses/MIT";

        // Act
        options.LicenseUrl = url;

        // Assert
        options.LicenseUrl.ShouldBe(url);
    }

    [Fact]
    public void IncludeXmlComments_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.IncludeXmlComments = false;

        // Assert
        options.IncludeXmlComments.ShouldBeFalse();
    }

    [Fact]
    public void RoutePrefix_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var prefix = "api-docs";

        // Act
        options.RoutePrefix = prefix;

        // Assert
        options.RoutePrefix.ShouldBe(prefix);
    }

    [Fact]
    public void EnableAnnotations_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.EnableAnnotations = false;

        // Assert
        options.EnableAnnotations.ShouldBeFalse();
    }

    [Fact]
    public void CustomCss_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var css = "/custom.css";

        // Act
        options.CustomCss = css;

        // Assert
        options.CustomCss.ShouldBe(css);
    }

    [Fact]
    public void DocumentTitle_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();
        var title = "My API Documentation";

        // Act
        options.DocumentTitle = title;

        // Assert
        options.DocumentTitle.ShouldBe(title);
    }

    [Fact]
    public void EnableDeepLinking_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.EnableDeepLinking = false;

        // Assert
        options.EnableDeepLinking.ShouldBeFalse();
    }

    [Fact]
    public void DisplayOperationId_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.DisplayOperationId = false;

        // Assert
        options.DisplayOperationId.ShouldBeFalse();
    }

    [Fact]
    public void DisplayRequestDuration_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.DisplayRequestDuration = false;

        // Assert
        options.DisplayRequestDuration.ShouldBeFalse();
    }

    [Fact]
    public void DefaultModelsExpandDepth_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.DefaultModelsExpandDepth = 2;

        // Assert
        options.DefaultModelsExpandDepth.ShouldBe(2);
    }

    [Fact]
    public void DefaultModelExpandDepth_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.DefaultModelExpandDepth = 3;

        // Assert
        options.DefaultModelExpandDepth.ShouldBe(3);
    }

    [Fact]
    public void ShowExtensions_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.ShowExtensions = true;

        // Assert
        options.ShowExtensions.ShouldBeTrue();
    }

    [Fact]
    public void ShowCommonExtensions_CanBeSet()
    {
        // Arrange
        var options = new SwaggerOptions();

        // Act
        options.ShowCommonExtensions = true;

        // Assert
        options.ShowCommonExtensions.ShouldBeTrue();
    }

    [Fact]
    public void AllProperties_CanBeSetTogether()
    {
        // Arrange & Act
        var options = new SwaggerOptions
        {
            EnableSwaggerUI = false,
            EnableInProduction = true,
            Title = "Custom API",
            Version = "v2",
            Description = "Custom Description",
            TermsOfServiceUrl = "https://example.com/terms",
            ContactName = "Support Team",
            ContactEmail = "support@example.com",
            ContactUrl = "https://example.com/contact",
            LicenseName = "MIT",
            LicenseUrl = "https://opensource.org/licenses/MIT",
            IncludeXmlComments = false,
            RoutePrefix = "api-docs",
            EnableAnnotations = false,
            CustomCss = "/custom.css",
            DocumentTitle = "My API Docs",
            EnableDeepLinking = false,
            DisplayOperationId = false,
            DisplayRequestDuration = false,
            DefaultModelsExpandDepth = 2,
            DefaultModelExpandDepth = 3,
            ShowExtensions = true,
            ShowCommonExtensions = true
        };

        // Assert
        options.EnableSwaggerUI.ShouldBeFalse();
        options.EnableInProduction.ShouldBeTrue();
        options.Title.ShouldBe("Custom API");
        options.Version.ShouldBe("v2");
        options.Description.ShouldBe("Custom Description");
        options.TermsOfServiceUrl.ShouldBe("https://example.com/terms");
        options.ContactName.ShouldBe("Support Team");
        options.ContactEmail.ShouldBe("support@example.com");
        options.ContactUrl.ShouldBe("https://example.com/contact");
        options.LicenseName.ShouldBe("MIT");
        options.LicenseUrl.ShouldBe("https://opensource.org/licenses/MIT");
        options.IncludeXmlComments.ShouldBeFalse();
        options.RoutePrefix.ShouldBe("api-docs");
        options.EnableAnnotations.ShouldBeFalse();
        options.CustomCss.ShouldBe("/custom.css");
        options.DocumentTitle.ShouldBe("My API Docs");
        options.EnableDeepLinking.ShouldBeFalse();
        options.DisplayOperationId.ShouldBeFalse();
        options.DisplayRequestDuration.ShouldBeFalse();
        options.DefaultModelsExpandDepth.ShouldBe(2);
        options.DefaultModelExpandDepth.ShouldBe(3);
        options.ShowExtensions.ShouldBeTrue();
        options.ShowCommonExtensions.ShouldBeTrue();
    }
}
