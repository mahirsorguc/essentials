namespace HMS.Essentials.Swashbuckle.Configuration;

/// <summary>
/// Configuration options for Swagger/OpenAPI documentation.
/// </summary>
public class SwaggerOptions
{
    /// <summary>
    /// Gets or sets whether Swagger UI is enabled.
    /// Default is true.
    /// </summary>
    public bool EnableSwaggerUI { get; set; } = true;

    /// <summary>
    /// Gets or sets whether Swagger should be available in Production environment.
    /// Default is false for security reasons.
    /// </summary>
    public bool EnableInProduction { get; set; } = false;

    /// <summary>
    /// Gets or sets the API title.
    /// Default is "API Documentation".
    /// </summary>
    public string Title { get; set; } = "API Documentation";

    /// <summary>
    /// Gets or sets the API version.
    /// Default is "v1".
    /// </summary>
    public string Version { get; set; } = "v1";

    /// <summary>
    /// Gets or sets the API description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the terms of service URL.
    /// </summary>
    public string? TermsOfServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets the contact name.
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Gets or sets the contact email.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Gets or sets the contact URL.
    /// </summary>
    public string? ContactUrl { get; set; }

    /// <summary>
    /// Gets or sets the license name.
    /// </summary>
    public string? LicenseName { get; set; }

    /// <summary>
    /// Gets or sets the license URL.
    /// </summary>
    public string? LicenseUrl { get; set; }

    /// <summary>
    /// Gets or sets whether to include XML comments in Swagger documentation.
    /// Default is true.
    /// </summary>
    public bool IncludeXmlComments { get; set; } = true;

    /// <summary>
    /// Gets or sets the route prefix for Swagger UI.
    /// Default is "swagger". Use empty string for root.
    /// </summary>
    public string RoutePrefix { get; set; } = "swagger";

    /// <summary>
    /// Gets or sets whether to enable annotations.
    /// Default is true.
    /// </summary>
    public bool EnableAnnotations { get; set; } = true;

    /// <summary>
    /// Gets or sets custom CSS for Swagger UI.
    /// </summary>
    public string? CustomCss { get; set; }

    /// <summary>
    /// Gets or sets the document title for Swagger UI.
    /// </summary>
    public string? DocumentTitle { get; set; }

    /// <summary>
    /// Gets or sets whether to enable deep linking in Swagger UI.
    /// Default is true.
    /// </summary>
    public bool EnableDeepLinking { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to display operation ID in Swagger UI.
    /// Default is true.
    /// </summary>
    public bool DisplayOperationId { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to display request duration in Swagger UI.
    /// Default is true.
    /// </summary>
    public bool DisplayRequestDuration { get; set; } = true;

    /// <summary>
    /// Gets or sets the default models expand depth.
    /// Default is 1.
    /// </summary>
    public int DefaultModelsExpandDepth { get; set; } = 1;

    /// <summary>
    /// Gets or sets the default model expand depth.
    /// Default is 1.
    /// </summary>
    public int DefaultModelExpandDepth { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether to show extensions in Swagger UI.
    /// Default is false.
    /// </summary>
    public bool ShowExtensions { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to show common extensions in Swagger UI.
    /// Default is false.
    /// </summary>
    public bool ShowCommonExtensions { get; set; } = false;
}
