using AstroReader.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstroReader.Infrastructure.Persistence.Configurations;

public class SavedChartConfiguration : IEntityTypeConfiguration<SavedChart>
{
    public void Configure(EntityTypeBuilder<SavedChart> builder)
    {
        builder.ToTable("SavedCharts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProfileName)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.PlaceName)
            .HasMaxLength(200);

        builder.Property(x => x.TimezoneIana)
            .HasMaxLength(100);

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.BirthTime)
            .IsRequired();

        builder.Property(x => x.TimezoneOffsetMinutes)
            .IsRequired();

        builder.Property(x => x.BirthInstantUtc)
            .IsRequired();

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6)
            .IsRequired();

        builder.Property(x => x.SunSign)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.MoonSign)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AscendantSign)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CalculationEngine)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.HouseSystemCode)
            .HasMaxLength(16);

        builder.Property(x => x.SnapshotVersion)
            .IsRequired();

        builder.Property(x => x.CalculatedChartJson)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
