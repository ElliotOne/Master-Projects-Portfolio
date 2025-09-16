using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.JobManagement.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JobLocationValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var jobAdvertisement = (JobAdvertisementFormDto)validationContext.ObjectInstance;

            // JobLocation is required when JobLocationType is 'on-site' or 'hybrid'
            if ((jobAdvertisement.JobLocationType == JobLocationType.OnSite
                 || jobAdvertisement.JobLocationType == JobLocationType.Hybrid) &&
                string.IsNullOrWhiteSpace(jobAdvertisement.JobLocation))
            {
                return new ValidationResult(
                    "Job Location is required when Job Location Type is On-site or Hybrid.");
            }

            // JobLocation should be null or empty when JobLocationType is 'remote'
            if (jobAdvertisement.JobLocationType == JobLocationType.Remote
                && !string.IsNullOrWhiteSpace(jobAdvertisement.JobLocation))
            {
                return new ValidationResult(
                    "Job Location should be empty when Job Location Type is Remote.");
            }

            return ValidationResult.Success;
        }
    }
}
