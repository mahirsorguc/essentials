using HMS.Essentials.Data;
using HMS.Essentials.Modularity;

namespace HMS.Essentials;

[DependsOn(
    typeof(EssentialsDataModule)
)]
public class EssentialsDemoModule : EssentialsModule
{
}