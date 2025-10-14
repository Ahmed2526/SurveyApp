using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class UniqueAndValidVoteAnswersAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable<VoteAnswerRequest> voteAnswers)
            return ValidationResult.Success;

        // Step 1️: Check for duplicate QuestionIds
        var duplicateIds = voteAnswers
            .GroupBy(v => v.QuestionId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateIds.Any())
        {
            return new ValidationResult($"Duplicate QuestionId(s) detected: {string.Join(", ", duplicateIds)}");
        }

        // Step 2️: Check AnswerId belongs to QuestionId
        // Access DbContext through ValidationContext
        var dbContext = (ApplicationDbContext?)validationContext.GetService(typeof(ApplicationDbContext));

        if (dbContext is null)
        {
            throw new InvalidOperationException("Database context is not available for validation.");
        }

        foreach (var vote in voteAnswers)
        {
            bool validAnswer = dbContext.Answers
                .AsNoTracking()
                .Any(a => a.Id == vote.AnswerId && a.QuestionId == vote.QuestionId);

            if (!validAnswer)
            {
                return new ValidationResult(
                    $"AnswerId {vote.AnswerId} does not belong to QuestionId {vote.QuestionId}."
                );
            }
        }

        return ValidationResult.Success;
    }
}
