using HMS.Essentials.MediatR;

namespace HMS.MainApp.Samples;

[UnitOfWork]
public sealed class CreateSampleCommand : ICommand<SampleDto>
{
    public CreateSampleCommand(string name, string description, bool isActive)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }

    public string Name { get; private set; }
    public  string Description { get; private set; }
    public bool IsActive { get; private set; }
}