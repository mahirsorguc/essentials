using HMS.Essentials.Modularity.DependencyInjection;
using HMS.Essentials.ObjectMapping;
using HMS.Essentials.SequentialGuid;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.MediatR;

public abstract class BaseCqrsHandler
{
    [InjectProperty] public required IEssentialsLazyServiceProvider LazyServiceProvider { get; set; }

    protected ISequentialGuidGenerator SequentialGuidGenerator =>
        LazyServiceProvider.LazyGetRequiredService<ISequentialGuidGenerator>().Value;

    protected ILogger Logger =>
        LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>().Value.CreateLogger(GetType());
    
    protected IObjectMapper ObjectMapper =>
        LazyServiceProvider.LazyGetRequiredService<IObjectMapper>().Value;
}