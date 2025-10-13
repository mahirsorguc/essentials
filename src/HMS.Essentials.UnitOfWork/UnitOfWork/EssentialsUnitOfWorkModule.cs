using HMS.Essentials.Modularity;

namespace HMS.Essentials.UnitOfWork;

/// <summary>
///     Unit of Work module providing transaction management capabilities.
/// </summary>
[DependsOn(typeof(EssentialsCoreModule))]
public class EssentialsUnitOfWorkModule : EssentialsModule
{
}