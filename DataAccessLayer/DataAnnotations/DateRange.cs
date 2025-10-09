using System.ComponentModel.DataAnnotations;

namespace SurveyAppAPI.DataAnnotations
{
    public class DateRange : ValidationAttribute
    {
        private readonly string _EndsAt;

        public DateRange(string startsAt)
        {
            _EndsAt = startsAt;
            ErrorMessage = "The start date must be earlier than the end date.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDate = (DateOnly?)value;

            var endDateProperty = validationContext.ObjectType.GetProperty(_EndsAt);
            if (endDateProperty == null)
                throw new ArgumentException($"Property '{_EndsAt}' not found.");

            var endDate = (DateOnly?)endDateProperty.GetValue(validationContext.ObjectInstance);

            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}
