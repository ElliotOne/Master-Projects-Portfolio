using StartupTeam.Module.JobManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.JobManagement.Validation
{
    public class JobApplicationDateValidationTests
    {
        private readonly ValidationContext _validationContext;

        public JobApplicationDateValidationTests()
        {
            _validationContext = new ValidationContext(new object());
        }

        [Fact]
        public void JobApplicationDate_ShouldPassValidation_WhenDateIsInFuture()
        {
            // Arrange
            var attribute = new JobApplicationDateValidationAttribute();
            var futureDate = DateTime.Now.AddDays(1); // Set future date

            // Act
            var result = attribute.GetValidationResult(futureDate, _validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void JobApplicationDate_ShouldFailValidation_WhenDateIsInPast()
        {
            // Arrange
            var attribute = new JobApplicationDateValidationAttribute();
            var pastDate = DateTime.Now.AddDays(-1); // Set past date

            // Act
            var result = attribute.GetValidationResult(pastDate, _validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Application deadline must be a future date.", result.ErrorMessage);
        }

        [Fact]
        public void JobApplicationDate_ShouldFailValidation_WhenDateIsPresent()
        {
            // Arrange
            var attribute = new JobApplicationDateValidationAttribute();
            var presentDate = DateTime.Now; // Set present date

            // Act
            var result = attribute.GetValidationResult(presentDate, _validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Application deadline must be a future date.", result.ErrorMessage);
        }

        [Fact]
        public void JobApplicationDate_ShouldPassValidation_WhenValueIsNull()
        {
            // Arrange
            var attribute = new JobApplicationDateValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(null, _validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result); // Null should be valid
        }

        [Fact]
        public void JobApplicationDate_ShouldPassValidation_WhenValueIsNotDateTime()
        {
            // Arrange
            var attribute = new JobApplicationDateValidationAttribute();

            // Act
            var result = attribute.GetValidationResult("Not a date", _validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result); // Non-DateTime values should be ignored
        }
    }
}
