using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations
{
    public class PollsConfiguration : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            // Table name
            builder.ToTable("Polls");

            // Primary key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Summary)
                .HasMaxLength(1000);

            builder.Property(p => p.IsPublished)
                .HasDefaultValue(false);

            builder.Property(p => p.StartsAt)
                .IsRequired();

            builder.Property(p => p.EndsAt)
                .IsRequired();

            // === Audit Fields ===
            builder.Property(p => p.CreatedById)
                .IsRequired()
                .HasMaxLength(450); // matches Identity UserId size

            builder.Property(p => p.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()") // database default
                .ValueGeneratedOnAdd();

            builder.Property(p => p.UpdatedById)
                .HasMaxLength(450);

            builder.Property(p => p.UpdatedOn)
                .ValueGeneratedOnUpdate();

        }
    }
}
