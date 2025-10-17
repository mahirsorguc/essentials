namespace HMS.Essentials.FluentValidation;

/// <summary>
/// Configuration options for FluentValidation module.
/// </summary>
public class FluentValidationConfiguration
{
    /// <summary>
    /// Gets or sets whether to automatically validate on property changes.
    /// Default: true
    /// </summary>
    public bool AutomaticValidationEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to disable implicit child validator injection.
    /// Default: false
    /// </summary>
    public bool DisableImplicitChildValidatorInjection { get; set; } = false;

    /// <summary>
    /// Gets or sets the default validator selector.
    /// Default: null (uses FluentValidation default)
    /// </summary>
    public string? DefaultValidatorSelector { get; set; }

    /// <summary>
    /// Gets or sets whether to throw exceptions when validation fails.
    /// Default: true
    /// </summary>
    public bool ThrowOnValidationFailures { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to cascade mode to stop validation on first failure.
    /// Default: false (validates all rules)
    /// </summary>
    public bool StopOnFirstFailure { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum validation depth for nested objects.
    /// Default: 10
    /// </summary>
    public int MaxValidationDepth { get; set; } = 10;

    /// <summary>
    /// Gets or sets whether to validate async rules synchronously when possible.
    /// Default: false
    /// </summary>
    public bool ValidateAsyncRulesSynchronously { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to use detailed error messages.
    /// Default: true
    /// </summary>
    public bool UseDetailedErrors { get; set; } = true;

    /// <summary>
    /// Gets or sets custom error message templates.
    /// </summary>
    public Dictionary<string, string> CustomErrorMessages { get; set; } = new();

    /// <summary>
    /// Gets or sets the assemblies to scan for validators.
    /// If empty, no automatic assembly scanning will be performed.
    /// </summary>
    public List<string> AssembliesToScan { get; set; } = new();

    /// <summary>
    /// Gets or sets whether to include validators from referenced assemblies.
    /// Default: false
    /// </summary>
    public bool IncludeReferencedAssemblies { get; set; } = false;

    /// <summary>
    /// Gets or sets ASP.NET Core specific validation options.
    /// </summary>
    public AspNetCoreValidationOptions AspNetCore { get; set; } = new();

    /// <summary>
    /// Validates the configuration and throws if invalid.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (MaxValidationDepth < 1)
        {
            throw new InvalidOperationException(
                $"{nameof(MaxValidationDepth)} must be greater than 0.");
        }

        if (MaxValidationDepth > 100)
        {
            throw new InvalidOperationException(
                $"{nameof(MaxValidationDepth)} must not exceed 100 to prevent stack overflow.");
        }
    }
}

/// <summary>
/// ASP.NET Core specific validation configuration options.
/// </summary>
public class AspNetCoreValidationOptions
{
    /// <summary>
    /// Gets or sets whether to enable automatic model validation for ASP.NET Core.
    /// When enabled, DTOs will be automatically validated during model binding.
    /// Default: true
    /// </summary>
    public bool EnableAutoValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to disable DataAnnotations validation.
    /// When true, only FluentValidation rules will be used.
    /// Default: true
    /// </summary>
    public bool DisableDataAnnotationsValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable client-side validation adapters.
    /// When enabled, validation rules will be rendered for client-side validation.
    /// Default: false
    /// </summary>
    public bool EnableClientSideValidation { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to use standardized error response format.
    /// When enabled, validation errors will be returned in a consistent format.
    /// Default: true
    /// </summary>
    public bool UseStandardErrorResponse { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to include detailed error information in responses.
    /// Includes TraceId and Instance path in error responses.
    /// Default: true
    /// </summary>
    public bool IncludeErrorDetails { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to run validation asynchronously.
    /// When false, async validators will be called synchronously during model binding.
    /// Default: false (for backward compatibility)
    /// </summary>
    public bool RunValidationAsynchronously { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to validate child properties automatically.
    /// When true, nested objects will be validated recursively.
    /// Default: true
    /// </summary>
    public bool ImplicitlyValidateChildProperties { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to validate root collection elements automatically.
    /// When true, collection items will be validated.
    /// Default: true
    /// </summary>
    public bool ImplicitlyValidateRootCollectionElements { get; set; } = true;
}
