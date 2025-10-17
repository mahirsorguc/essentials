using HMS.Essentials.MediatR;

namespace HMS.MainApp.Samples;

[UnitOfWork]
public sealed class CreateSampleCommand : ICommand<SampleDto>
{
    public required CreateSampleDto CreateSampleDto { get; set; }
}