using AstroReader.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstroReader.Infrastructure.Persistence.Configurations;

public class PersonalProfileConfiguration : IEntityTypeConfiguration<PersonalProfile>
{
    public void Configure(EntityTypeBuilder<PersonalProfile> builder)
    {
        builder.ToTable("PersonalProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.BirthTime)
            .IsRequired();

        builder.Property(x => x.BirthInstantUtc)
            .IsRequired();

        builder.Property(x => x.BirthPlace)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6)
            .IsRequired();

        builder.Property(x => x.TimezoneOffsetMinutes)
            .IsRequired();

        builder.Property(x => x.SelfPerceptionFocus)
            .HasMaxLength(280)
            .IsRequired();

        builder.Property(x => x.CurrentChallenge)
            .HasMaxLength(280)
            .IsRequired();

        builder.Property(x => x.DesiredInsight)
            .HasMaxLength(280)
            .IsRequired();

        builder.Property(x => x.SelfDescription)
            .HasMaxLength(600);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasOne<SavedChart>()
            .WithMany()
            .HasForeignKey(x => x.SavedChartId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CreatedAtUtc);
        builder.HasIndex(x => x.SavedChartId)
            .IsUnique()
            .HasFilter("[SavedChartId] IS NOT NULL");
    }
}
