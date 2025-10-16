using AutoMapper;
using HMS.Essentials.ObjectMapping;

namespace HMS.Essentials.AutoMapper.ObjectMapping;

/// <summary>
/// AutoMapper implementation of <see cref="IObjectMapper"/>.
/// </summary>
public class AutoMapperObjectMapper : IObjectMapper
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoMapperObjectMapper"/> class.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance.</param>
    public AutoMapperObjectMapper(IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc/>
    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source);
    }

    /// <inheritdoc/>
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return _mapper.Map<TSource, TDestination>(source);
    }

    /// <inheritdoc/>
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return _mapper.Map(source, destination);
    }
}
