using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations
{
    public class AnswersConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            // Table name
            builder.ToTable("Answers");

            // Primary key
            builder.HasKey(p => p.Id);

            builder.Property(e => e.Content)
                .IsRequired().HasMaxLength(3000);

            //Unique constraint: Answer content must be unique within each Question
            builder.HasIndex(q => new { q.QuestionId, q.Content })
                .IsUnique();


            ////Generate Some Answers
            builder.HasData(
            // Poll 1 — Technology Trends
            new Answer { Id = 1, Content = "Artificial Intelligence", QuestionId = 1 },
            new Answer { Id = 2, Content = "Quantum Computing", QuestionId = 1 },
            new Answer { Id = 3, Content = "Blockchain", QuestionId = 1 },

            new Answer { Id = 4, Content = "Yes, it will create new job categories", QuestionId = 2 },
            new Answer { Id = 5, Content = "No, it will cause job loss", QuestionId = 2 },
            new Answer { Id = 6, Content = "Not sure", QuestionId = 2 },

            new Answer { Id = 7, Content = "Daily", QuestionId = 3 },
            new Answer { Id = 8, Content = "Weekly", QuestionId = 3 },
            new Answer { Id = 9, Content = "Rarely", QuestionId = 3 },

            // Poll 2 — Environmental Awareness
            new Answer { Id = 10, Content = "Always", QuestionId = 4 },
            new Answer { Id = 11, Content = "Sometimes", QuestionId = 4 },
            new Answer { Id = 12, Content = "Never", QuestionId = 4 },

            new Answer { Id = 13, Content = "Yes, definitely", QuestionId = 5 },
            new Answer { Id = 14, Content = "Maybe, if affordable", QuestionId = 5 },
            new Answer { Id = 15, Content = "No, price matters more", QuestionId = 5 },

            new Answer { Id = 16, Content = "Yes, carbon taxes are essential", QuestionId = 6 },
            new Answer { Id = 17, Content = "No, it hurts the economy", QuestionId = 6 },
            new Answer { Id = 18, Content = "Not sure", QuestionId = 6 },

            // Poll 3 — Healthcare and Lifestyle
            new Answer { Id = 19, Content = "0–2 times", QuestionId = 7 },
            new Answer { Id = 20, Content = "3–5 times", QuestionId = 7 },
            new Answer { Id = 21, Content = "6+ times", QuestionId = 7 },

            new Answer { Id = 22, Content = "Yes", QuestionId = 8 },
            new Answer { Id = 23, Content = "No", QuestionId = 8 },

            new Answer { Id = 24, Content = "Less than 5 hours", QuestionId = 9 },
            new Answer { Id = 25, Content = "5–7 hours", QuestionId = 9 },
            new Answer { Id = 26, Content = "8 or more hours", QuestionId = 9 },

            // Poll 4 — Education in the Digital Era
            new Answer { Id = 27, Content = "Online learning", QuestionId = 10 },
            new Answer { Id = 28, Content = "Traditional classroom", QuestionId = 10 },
            new Answer { Id = 29, Content = "Hybrid (both)", QuestionId = 10 },

            new Answer { Id = 30, Content = "Yes, much improved", QuestionId = 11 },
            new Answer { Id = 31, Content = "Somewhat", QuestionId = 11 },
            new Answer { Id = 32, Content = "No improvement", QuestionId = 11 },

            new Answer { Id = 33, Content = "Yes, digital only", QuestionId = 12 },
            new Answer { Id = 34, Content = "Keep both digital and physical", QuestionId = 12 },
            new Answer { Id = 35, Content = "Prefer only physical", QuestionId = 12 }
        );

        }
    }
}
