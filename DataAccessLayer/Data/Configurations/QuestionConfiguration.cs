using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            // Table name
            builder.ToTable("Questions");

            // Primary key
            builder.HasKey(p => p.Id);

            builder.Property(e => e.Content)
                .IsRequired().HasMaxLength(3000);

            //Unique constraint: question content must be unique within each poll
            builder.HasIndex(q => new { q.PollId, q.Content })
                .IsUnique();


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

            //Seed Sample Questions
            builder.HasData(
            // Poll 1 — Technology Trends
            new Question { Id = 1, Content = "Which emerging technology will have the biggest impact in 2025?", IsActive = true, PollId = 1, CreatedById = "Ahmed" },
            new Question { Id = 2, Content = "Do you believe AI will create more jobs than it replaces?", IsActive = true, PollId = 1, CreatedById = "Ahmed" },
            new Question { Id = 3, Content = "How often do you use AI tools like ChatGPT?", IsActive = true, PollId = 1, CreatedById = "Ahmed" },

            // Poll 2 — Environmental Awareness
            new Question { Id = 4, Content = "Do you recycle regularly?", IsActive = true, PollId = 2, CreatedById = "Ahmed" },
            new Question { Id = 5, Content = "Would you pay more for eco-friendly products?", IsActive = true, PollId = 2, CreatedById = "Ahmed" },
            new Question { Id = 6, Content = "Do you think governments should impose carbon taxes?", IsActive = true, PollId = 2, CreatedById = "Ahmed" },

            // Poll 3 — Healthcare and Lifestyle
            new Question { Id = 7, Content = "How many times a week do you exercise?", IsActive = true, PollId = 3, CreatedById = "Ahmed" },
            new Question { Id = 8, Content = "Do you track your daily calorie intake?", IsActive = true, PollId = 3, CreatedById = "Ahmed" },
            new Question { Id = 9, Content = "How many hours of sleep do you get on average?", IsActive = true, PollId = 3, CreatedById = "Ahmed" },

            // Poll 4 — Education in the Digital Era
            new Question { Id = 10, Content = "Do you prefer online or traditional classroom learning?", IsActive = true, PollId = 4, CreatedById = "Ahmed" },
            new Question { Id = 11, Content = "Have digital tools improved your learning productivity?", IsActive = true, PollId = 4, CreatedById = "Ahmed" },
            new Question { Id = 12, Content = "Should schools fully adopt digital textbooks?", IsActive = true, PollId = 4, CreatedById = "Ahmed" }
        );


        }
    }
}
