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


            //Generate Some Polls
            builder.HasData(
            new Poll
            {
                Id = 1,
                Title = "Technology Trends 2025",
                Summary = "A survey on the latest technology trends and innovations shaping 2025.",
                IsPublished = true,
                StartsAt = new DateOnly(2025, 10, 1),
                EndsAt = new DateOnly(2025, 12, 31),
                CreatedById = "Ahmed"
            },
            new Poll
            {
                Id = 2,
                Title = "Environmental Awareness",
                Summary = "Survey on public awareness about sustainability and climate change.",
                IsPublished = true,
                StartsAt = new DateOnly(2025, 9, 15),
                EndsAt = new DateOnly(2026, 1, 15),
                CreatedById = "Ahmed"
            },
            new Poll
            {
                Id = 3,
                Title = "Healthcare and Lifestyle Habits",
                Summary = "Exploring people's health habits, fitness, and dietary preferences.",
                IsPublished = false,
                StartsAt = new DateOnly(2025, 11, 10),
                EndsAt = new DateOnly(2026, 2, 10),
                CreatedById = "Ahmed"
            },
            new Poll
            {
                Id = 4,
                Title = "Education in the Digital Era",
                Summary = "Opinions on the impact of technology on learning and education systems.",
                IsPublished = true,
                StartsAt = new DateOnly(2025, 8, 1),
                EndsAt = new DateOnly(2025, 10, 31),
                CreatedById = "Ahmed"
            }
        );



        }
    }
}
