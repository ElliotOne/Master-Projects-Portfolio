using StartupTeam.Module.TeamManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.TeamManagement.Validation
{
    public class DueDateValidationAttributeTests
    {
        [Fact]
        public void DueDate_ShouldPassValidation_WhenFutureDate()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new DueDateValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(DateTime.Now.AddDays(5), validationContext); // Future date

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void DueDate_ShouldFailValidation_WhenPastDate()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new DueDateValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(DateTime.Now.AddDays(-1), validationContext); // Past date

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Due date must be a future date.", result.ErrorMessage);
        }

        [Fact]
        public void DueDate_ShouldFailValidation_WhenDateIsToday()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new DueDateValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(DateTime.Now, validationContext); // Today's date (not in the future)

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Due date must be a future date.", result.ErrorMessage);
        }

        [Fact]
        public void DueDate_ShouldPassValidation_WhenValueIsNull()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var attribute = new DueDateValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(null, validationContext); // Null value (valid case)

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }
    }
}
