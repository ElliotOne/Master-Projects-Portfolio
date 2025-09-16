using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Models.Enums;
using StartupTeam.Module.JobManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.JobManagement.Validation
{
    public class JobApplicationInterviewDateValidationTests
    {
        [Fact]
        public void JobApplication_ShouldPassValidation_WhenInterviewDateIsValidForInterviewScheduledStatus()
        {
            // Arrange
            var jobApplication = new JobApplicationUpdateFormDto
            {
                Status = JobApplicationStatus.InterviewScheduled,
                InterviewDate = DateTime.Now.AddDays(1) // Future date
            };
            var validationContext = new ValidationContext(jobApplication);

            var attribute = new JobApplicationInterviewDateValidation();

            // Act
            var result = attribute.GetValidationResult(jobApplication.InterviewDate, validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void JobApplication_ShouldFailValidation_WhenInterviewDateIsMissingForInterviewScheduledStatus()
        {
            // Arrange
            var jobApplication = new JobApplicationUpdateFormDto
            {
                Status = JobApplicationStatus.InterviewScheduled,
                InterviewDate = null // Missing interview date
            };
            var validationContext = new ValidationContext(jobApplication);

            var attribute = new JobApplicationInterviewDateValidation();

            // Act
            var result = attribute.GetValidationResult(jobApplication.InterviewDate, validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Interview Date is required when the application status is 'Interview Scheduled'.", result.ErrorMessage);
        }

        [Fact]
        public void JobApplication_ShouldFailValidation_WhenInterviewDateIsInPast()
        {
            // Arrange
            var jobApplication = new JobApplicationUpdateFormDto
            {
                Status = JobApplicationStatus.InterviewScheduled,
                InterviewDate = DateTime.Now.AddDays(-1) // Past date
            };
            var validationContext = new ValidationContext(jobApplication);

            var attribute = new JobApplicationInterviewDateValidation();

            // Act
            var result = attribute.GetValidationResult(jobApplication.InterviewDate, validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Interview date must be a future date.", result.ErrorMessage);
        }

        [Fact]
        public void JobApplication_ShouldPassValidation_WhenInterviewDateIsNullForNonInterviewScheduledStatus()
        {
            // Arrange
            var jobApplication = new JobApplicationUpdateFormDto
            {
                Status = JobApplicationStatus.Submitted,
                InterviewDate = null // Interview date is not required for Submitted status
            };
            var validationContext = new ValidationContext(jobApplication);

            var attribute = new JobApplicationInterviewDateValidation();

            // Act
            var result = attribute.GetValidationResult(jobApplication.InterviewDate, validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void JobApplication_ShouldPassValidation_WhenInterviewDateIsNotDateTime()
        {
            // Arrange
            var jobApplication = new JobApplicationUpdateFormDto();
            var validationContext = new ValidationContext(jobApplication);

            var attribute = new JobApplicationInterviewDateValidation();

            // Act
            var result = attribute.GetValidationResult("Not a date", validationContext); // Non-DateTime value

            // Assert
            Assert.Equal(ValidationResult.Success, result); // Non-DateTime values should be ignored
        }
    }
}
