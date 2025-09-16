using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.JobManagement.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FoundingYearValidationAttribute : ValidationAttribute
    {
        private readonly int _minYear;

        public FoundingYearValidationAttribute(int minYear = 2000)
        {
            _minYear = minYear;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                int currentYear = DateTime.Now.Year;
                if (year < _minYear || year > currentYear)
                {
                    return new ValidationResult($"Founding year must be between {_minYear} and {currentYear}.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
