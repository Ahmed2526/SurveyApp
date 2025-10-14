using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations
{
    public class VoteAnswersConfiguration : IEntityTypeConfiguration<VoteAnswer>
    {
        public void Configure(EntityTypeBuilder<VoteAnswer> builder)
        {
            // Table name
            builder.ToTable("VoteAnswers");

            // Primary key
            builder.HasKey(va => va.Id);

            // QuestionId and AnswerId are foreign keys but not explicitly navigations yet
            builder.Property(va => va.QuestionId)
                   .IsRequired();

            builder.Property(va => va.AnswerId)
                   .IsRequired();

            // Unique index — ensure a user can only answer each question once per vote
            builder.HasIndex(va => new { va.VoteId, va.QuestionId })
                   .IsUnique();

            // Optional: add constraint names for clarity
            builder.HasIndex(va => va.AnswerId)
                   .HasDatabaseName("IX_VoteAnswers_AnswerId");
        }
    }
}
