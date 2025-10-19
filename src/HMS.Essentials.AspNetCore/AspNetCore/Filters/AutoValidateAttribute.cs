using Microsoft.AspNetCore.Mvc.Filters;

namespace HMS.Essentials.AspNetCore.Filters;

/// <summary>
/// Attribute to enable automatic FluentValidation for an action or controller.
/// When applied, the FluentValidationActionFilter will automatically validate request models.
/// </summary>
/// <example>
/// <code>
/// [AutoValidate]
/// [HttpPost]
/// public async Task&lt;IActionResult&gt; CreateProduct(CreateProductDto dto)
/// {
///     // Validation is performed automatically before reaching here
///     // If validation fails, a BadRequest response is returned automatically
///     return Ok(await _productService.CreateAsync(dto));
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AutoValidateAttribute : Attribute, IFilterFactory
{
    /// <summary>
    /// Gets a value indicating whether this filter can be reused across requests.
    /// </summary>
    public bool IsReusable => false;

    /// <summary>
    /// Creates an instance of the FluentValidationActionFilter.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
    /// <returns>An instance of IFilterMetadata.</returns>
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetService(typeof(Microsoft.Extensions.Logging.ILogger<FluentValidationActionFilter>)) 
            as Microsoft.Extensions.Logging.ILogger<FluentValidationActionFilter>;

        if (logger == null)
        {
            throw new InvalidOperationException(
                "ILogger<FluentValidationActionFilter> could not be resolved from the service provider.");
        }

        return new FluentValidationActionFilter(logger);
    }
}
