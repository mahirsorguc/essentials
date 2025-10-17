using FluentValidation;
using FluentValidation.Results;

namespace HMS.Essentials.FluentValidation.Validators;

/// <summary>
/// Abstract base class for asynchronous validators.
/// Provides helper methods for async validation scenarios.
/// </summary>
/// <typeparam name="T">The type being validated.</typeparam>
public abstract class AsyncValidatorBase<T> : AbstractValidator<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncValidatorBase{T}"/> class.
    /// </summary>
    protected AsyncValidatorBase()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
    }

    /// <summary>
    /// Validates that an entity exists asynchronously.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <param name="id">The identifier to check.</param>
    /// <param name="existsCheck">Function to check if entity exists.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if entity exists; otherwise false.</returns>
    protected async Task<bool> EntityExistsAsync<TId>(
        TId id,
        Func<TId, CancellationToken, Task<bool>> existsCheck,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(existsCheck);
        return await existsCheck(id, cancellationToken);
    }

    /// <summary>
    /// Validates that a value is unique asynchronously.
    /// </summary>
    /// <typeparam name="TValue">The type of value to check.</typeparam>
    /// <param name="value">The value to check for uniqueness.</param>
    /// <param name="uniqueCheck">Function to check if value is unique.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if value is unique; otherwise false.</returns>
    protected async Task<bool> BeUniqueAsync<TValue>(
        TValue value,
        Func<TValue, CancellationToken, Task<bool>> uniqueCheck,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uniqueCheck);
        return await uniqueCheck(value, cancellationToken);
    }

    /// <summary>
    /// Validates that a property has changed from its original value.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    /// <param name="currentValue">The current value.</param>
    /// <param name="originalValueProvider">Function to get the original value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if value has changed; otherwise false.</returns>
    protected async Task<bool> HasChangedAsync<TValue>(
        TValue currentValue,
        Func<CancellationToken, Task<TValue>> originalValueProvider,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(originalValueProvider);
        var originalValue = await originalValueProvider(cancellationToken);
        return !EqualityComparer<TValue>.Default.Equals(currentValue, originalValue);
    }

    /// <summary>
    /// Performs batch validation asynchronously.
    /// Useful for validating collections of items.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    /// <param name="items">The items to validate.</param>
    /// <param name="itemValidator">Validator for individual items.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validation results for all items.</returns>
    protected async Task<IEnumerable<ValidationResult>> ValidateBatchAsync<TItem>(
        IEnumerable<TItem> items,
        IValidator<TItem> itemValidator,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(itemValidator);

        var validationTasks = items
            .Select(item => itemValidator.ValidateAsync(item, cancellationToken));

        return await Task.WhenAll(validationTasks);
    }
}
