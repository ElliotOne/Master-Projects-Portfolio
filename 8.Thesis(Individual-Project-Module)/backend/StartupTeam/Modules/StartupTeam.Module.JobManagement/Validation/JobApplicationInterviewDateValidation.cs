using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.JobManagement.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JobApplicationInterviewDateValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var jobApplication = (JobApplicationUpdateFormDto)validationContext.ObjectInstance;

            // InterviewDate is required when the Status is 'InterviewScheduled'
            if (jobApplication.Status == JobApplicationStatus.InterviewScheduled && !jobApplication.InterviewDate.HasValue)
            {
                return new ValidationResult("Interview Date is required when the application status is 'Interview Scheduled'.");
            }

            if (value is DateTime dateValue)
            {
                if (dateValue <= DateTime.Now)
                {
                    return new ValidationResult("Interview date must be a future date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
