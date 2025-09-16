using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.JobManagement.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class TeamSizeValidationAttribute : ValidationAttribute
    {
        private readonly int _minSize;
        private readonly int _maxSize;

        public TeamSizeValidationAttribute(int minSize = 1, int maxSize = 10000)
        {
            _minSize = minSize;
            _maxSize = maxSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int size)
            {
                if (size < _minSize || size > _maxSize)
                {
                    return new ValidationResult($"Team size must be between {_minSize} and {_maxSize}.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
