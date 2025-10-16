using HMS.Essentials.AutoMapper.ObjectMapping;
using HMS.Essentials.ObjectMapping;

namespace HMS.Essentials.AutoMapper.Samples;

/// <summary>
/// Sample AutoMapper profile demonstrating how to create mappings.
/// </summary>
public class SampleMappingProfile : EssentialsProfile
{
    /// <inheritdoc/>
    protected override void ConfigureMappings()
    {
        // Example: Creating a simple mapping
        // CreateMap<SourceType, DestinationType>();
        
        // Example: Creating a reverse mapping
        // CreateMap<SourceType, DestinationType>()
        //     .ReverseMap();
        
        // Example: Custom property mapping
        // CreateMap<SourceType, DestinationType>()
        //     .ForMember(dest => dest.PropertyName, 
        //                opt => opt.MapFrom(src => src.SourceProperty));
    }
}
