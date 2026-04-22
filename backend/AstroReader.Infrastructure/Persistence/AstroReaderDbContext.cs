using AstroReader.Domain.Entities;
using AstroReader.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AstroReader.Infrastructure.Persistence;

public class AstroReaderDbContext : DbContext
{
    public AstroReaderDbContext(DbContextOptions<AstroReaderDbContext> options)
        : base(options)
    {
    }

    public DbSet<SavedChart> SavedCharts => Set<SavedChart>();
    public DbSet<PersonalProfile> PersonalProfiles => Set<PersonalProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PersonalProfileConfiguration());
        modelBuilder.ApplyConfiguration(new SavedChartConfiguration());
    }
}
