namespace HMS.Essentials.ObjectMapping;

/// <summary>
/// Provides object-to-object mapping functionality.
/// </summary>
public interface IObjectMapper
{
    /// <summary>
    /// Maps the source object to a new destination object.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination object.</typeparam>
    /// <param name="source">The source object.</param>
    /// <returns>A new instance of the destination object.</returns>
    TDestination Map<TDestination>(object source);

    /// <summary>
    /// Maps the source object to a new destination object.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="TDestination">The type of the destination object.</typeparam>
    /// <param name="source">The source object.</param>
    /// <returns>A new instance of the destination object.</returns>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Maps the source object to an existing destination object.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="TDestination">The type of the destination object.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="destination">The destination object.</param>
    /// <returns>The destination object with mapped values.</returns>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
}
