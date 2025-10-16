using AutoMapper;

namespace HMS.Essentials.AutoMapper.ObjectMapping;

/// <summary>
/// Base class for AutoMapper profiles in HMS Essentials framework.
/// Inherit from this class to create mapping profiles.
/// </summary>
public abstract class EssentialsProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EssentialsProfile"/> class.
    /// </summary>
    protected EssentialsProfile()
    {
        ConfigureMappings();
    }

    /// <summary>
    /// Configures the mappings for this profile.
    /// Override this method to define your mappings.
    /// </summary>
    protected abstract void ConfigureMappings();
}
