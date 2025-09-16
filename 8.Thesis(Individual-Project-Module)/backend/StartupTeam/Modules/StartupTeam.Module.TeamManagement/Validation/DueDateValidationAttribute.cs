using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.TeamManagement.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DueDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue <= DateTime.Now)
                {
                    return new ValidationResult("Due date must be a future date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
