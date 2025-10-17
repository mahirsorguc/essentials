namespace HMS.MainApp.Samples;

public struct CreateSampleDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; }
}