using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Models.Enums;
using StartupTeam.Module.JobManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.JobManagement.Validation
{
    public class JobLocationValidationAttributeTests
    {
        [Fact]
        public void JobLocation_ShouldPassValidation_WhenJobLocationTypeIsOnSiteAndLocationIsProvided()
        {
            // Arrange
            var jobAdvertisement = new JobAdvertisementFormDto
            {
                JobLocationType = JobLocationType.OnSite,
                JobLocation = "New York"
            };
            var validationContext = new ValidationContext(jobAdvertisement);

            var attribute = new JobLocationValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(jobAdvertisement.JobLocation, validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void JobLocation_ShouldFailValidation_WhenJobLocationTypeIsOnSiteAndLocationIsNotProvided()
        {
            // Arrange
            var jobAdvertisement = new JobAdvertisementFormDto
            {
                JobLocationType = JobLocationType.OnSite,
                JobLocation = null
            };
            var validationContext = new ValidationContext(jobAdvertisement);

            var attribute = new JobLocationValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(jobAdvertisement.JobLocation, validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Job Location is required when Job Location Type is On-site or Hybrid.", result.ErrorMessage);
        }

        [Fact]
        public void JobLocation_ShouldPassValidation_WhenJobLocationTypeIsRemoteAndLocationIsEmpty()
        {
            // Arrange
            var jobAdvertisement = new JobAdvertisementFormDto
            {
                JobLocationType = JobLocationType.Remote,
                JobLocation = string.Empty
            };
            var validationContext = new ValidationContext(jobAdvertisement);

            var attribute = new JobLocationValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(jobAdvertisement.JobLocation, validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void JobLocation_ShouldFailValidation_WhenJobLocationTypeIsRemoteAndLocationIsProvided()
        {
            // Arrange
            var jobAdvertisement = new JobAdvertisementFormDto
            {
                JobLocationType = JobLocationType.Remote,
                JobLocation = "New York"
            };
            var validationContext = new ValidationContext(jobAdvertisement);

            var attribute = new JobLocationValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(jobAdvertisement.JobLocation, validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Job Location should be empty when Job Location Type is Remote.", result.ErrorMessage);
        }

        [Fact]
        public void JobLocation_ShouldPassValidation_WhenJobLocationTypeIsHybridAndLocationIsProvided()
        {
            // Arrange
            var jobAdvertisement = new JobAdvertisementFormDto
            {
                JobLocationType = JobLocationType.Hybrid,
                JobLocation = "San Francisco"
            };
            var validationContext = new ValidationContext(jobAdvertisement);

            var attribute = new JobLocationValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(jobAdvertisement.JobLocation, validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void JobLocation_ShouldFailValidation_WhenJobLocationTypeIsHybridAndLocationIsNotProvided()
        {
            // Arrange
            var jobAdvertisement = new JobAdvertisementFormDto
            {
                JobLocationType = JobLocationType.Hybrid,
                JobLocation = null
            };
            var validationContext = new ValidationContext(jobAdvertisement);

            var attribute = new JobLocationValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(jobAdvertisement.JobLocation, validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Job Location is required when Job Location Type is On-site or Hybrid.", result.ErrorMessage);
        }
    }
}
