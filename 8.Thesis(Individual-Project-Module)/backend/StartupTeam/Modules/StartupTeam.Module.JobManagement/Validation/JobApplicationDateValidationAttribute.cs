using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.JobManagement.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JobApplicationDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue <= DateTime.Now)
                {
                    return new ValidationResult("Application deadline must be a future date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
