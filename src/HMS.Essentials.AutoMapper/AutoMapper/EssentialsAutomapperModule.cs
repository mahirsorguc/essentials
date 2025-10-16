using HMS.Essentials.Modularity;
using HMS.Essentials.ObjectMapping;

namespace HMS.Essentials.AutoMapper;

/// <summary>
///     HMS Essentials AutoMapper module.
///     Provides object-to-object mapping functionality using AutoMapper.
/// </summary>
[DependsOn(
    typeof(EssentialsObjectMappingModule)
)]
public class EssentialsAutomapperModule : EssentialsModule
{
}