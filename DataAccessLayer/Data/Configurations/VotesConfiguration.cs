using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations
{
    public class VotesConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            // Table name
            builder.ToTable("Votes");

            // Primary key
            builder.HasKey(v => v.Id);

            // Properties
            builder.Property(v => v.SubmittedOn)
                   .HasDefaultValueSql("GETUTCDATE()") // default to UTC time at DB level
                   .IsRequired();

            // Relationships
            builder.HasOne(v => v.Poll)
                   .WithMany()
                   .HasForeignKey(v => v.PollId)
                   .OnDelete(DeleteBehavior.Cascade); // delete votes if poll deleted

            builder.HasOne(v => v.ApplicationUser)
                   .WithMany()
                   .HasForeignKey(v => v.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // prevent user delete if votes exist

            // Unique constraint: one vote per poll per user
            builder.HasIndex(v => new { v.PollId, v.UserId })
                   .IsUnique();

            // Property constraints
            builder.Property(v => v.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(v => v.PollId)
                   .IsRequired();
        }
    }
}
