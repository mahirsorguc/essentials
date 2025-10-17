using HMS.Essentials.Domain.Entities;

namespace HMS.MainApp.Samples;

public class Sample : AggregateRoot<Guid>
{
    protected Sample()
    {
    }

    public Sample(Guid id, string name, string description, bool isActive) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        IsActive = isActive;
    }

    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public bool IsActive { get; protected set; }
}