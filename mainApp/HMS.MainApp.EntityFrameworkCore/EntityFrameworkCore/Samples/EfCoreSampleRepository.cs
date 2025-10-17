using HMS.Essentials.EntityFrameworkCore.Repositories;
using HMS.MainApp.Samples;

namespace HMS.MainApp.EntityFrameworkCore.Samples;

public class EfCoreSampleRepository : EfCoreRepository<MainAppDbContext, Sample, Guid>, ISampleRepository
{
    public EfCoreSampleRepository(MainAppDbContext dbContext) : base(dbContext)
    {
    }
}