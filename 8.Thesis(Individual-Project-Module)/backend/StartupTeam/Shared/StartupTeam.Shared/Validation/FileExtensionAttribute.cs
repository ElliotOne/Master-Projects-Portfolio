using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Shared.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FileExtensionAttribute : ValidationAttribute
    {
        private readonly string[] _validExtensions;

        public FileExtensionAttribute(string[] validExtensions)
        {
            _validExtensions = validExtensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_validExtensions.Contains(extension))
                {
                    return new ValidationResult($"Invalid file type. Allowed types are: {string.Join(", ", _validExtensions)}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
