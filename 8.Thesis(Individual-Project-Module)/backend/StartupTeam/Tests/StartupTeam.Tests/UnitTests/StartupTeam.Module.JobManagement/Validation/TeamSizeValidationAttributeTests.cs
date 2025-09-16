using StartupTeam.Module.JobManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.JobManagement.Validation
{
    public class TeamSizeValidationAttributeTests
    {
        [Fact]
        public void TeamSize_ShouldPassValidation_WhenWithinValidRange()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new TeamSizeValidationAttribute(minSize: 1, maxSize: 10000);

            // Act
            var result = attribute.GetValidationResult(50, validationContext); // Valid team size

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void TeamSize_ShouldFailValidation_WhenBelowMinSize()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new TeamSizeValidationAttribute(minSize: 5, maxSize: 10000);

            // Act
            var result = attribute.GetValidationResult(3, validationContext); // Team size below minSize

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Team size must be between 5 and 10000.", result.ErrorMessage);
        }

        [Fact]
        public void TeamSize_ShouldFailValidation_WhenAboveMaxSize()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new TeamSizeValidationAttribute(minSize: 1, maxSize: 100);

            // Act
            var result = attribute.GetValidationResult(150, validationContext); // Team size above maxSize

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Team size must be between 1 and 100.", result.ErrorMessage);
        }

        [Fact]
        public void TeamSize_ShouldPassValidation_WhenEqualToMinSize()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new TeamSizeValidationAttribute(minSize: 5, maxSize: 100);

            // Act
            var result = attribute.GetValidationResult(5, validationContext); // Team size equals minSize

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void TeamSize_ShouldPassValidation_WhenEqualToMaxSize()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new TeamSizeValidationAttribute(minSize: 1, maxSize: 100);

            // Act
            var result = attribute.GetValidationResult(100, validationContext); // Team size equals maxSize

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void TeamSize_ShouldPassValidation_WhenSizeIsNull()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new TeamSizeValidationAttribute(minSize: 1, maxSize: 100);

            // Act
            var result = attribute.GetValidationResult(null, validationContext); // Null value (size is optional)

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }
    }
}