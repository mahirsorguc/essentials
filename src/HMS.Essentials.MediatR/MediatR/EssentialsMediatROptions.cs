namespace HMS.Essentials.MediatR;

public class EssentialsMediatROptions
{
    public bool FluentValidationEnabled { get; set; }
    public bool UnitOfWorkEnabled { get; set; }
    public bool PerformanceLoggingEnabled { get; set; }
    public bool RequestLoggingEnabled { get; set; }
}