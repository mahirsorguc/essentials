using HMS.Essentials.Domain.Repositories;

namespace HMS.MainApp.Samples;

public interface ISampleRepository : IRepository<Sample, Guid>
{
}