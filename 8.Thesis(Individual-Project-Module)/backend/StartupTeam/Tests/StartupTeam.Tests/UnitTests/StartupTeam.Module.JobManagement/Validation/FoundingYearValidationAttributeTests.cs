using StartupTeam.Module.JobManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.JobManagement.Validation
{
    public class FoundingYearValidationTests
    {
        private readonly ValidationContext _validationContext;

        public FoundingYearValidationTests()
        {
            _validationContext = new ValidationContext(new object());
        }

        [Fact]
        public void FoundingYear_ShouldPassValidation_WhenYearIsWithinRange()
        {
            // Arrange
            int currentYear = DateTime.Now.Year;
            var attribute = new FoundingYearValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(currentYear, _validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void FoundingYear_ShouldFailValidation_WhenYearIsLessThanMinYear()
        {
            // Arrange
            var attribute = new FoundingYearValidationAttribute(minYear: 2000);
            int invalidYear = 1999;

            // Act
            var result = attribute.GetValidationResult(invalidYear, _validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Founding year must be between 2000 and " + DateTime.Now.Year + ".", result.ErrorMessage);
        }

        [Fact]
        public void FoundingYear_ShouldFailValidation_WhenYearIsGreaterThanCurrentYear()
        {
            // Arrange
            var attribute = new FoundingYearValidationAttribute();
            int invalidYear = DateTime.Now.Year + 1;

            // Act
            var result = attribute.GetValidationResult(invalidYear, _validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"Founding year must be between 2000 and {DateTime.Now.Year}.", result.ErrorMessage);
        }

        [Fact]
        public void FoundingYear_ShouldPassValidation_WhenYearIsAtMinYear()
        {
            // Arrange
            var attribute = new FoundingYearValidationAttribute(minYear: 2000);
            int validYear = 2000;

            // Act
            var result = attribute.GetValidationResult(validYear, _validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void FoundingYear_ShouldFailValidation_WhenValueIsNotInteger()
        {
            // Arrange
            var attribute = new FoundingYearValidationAttribute();

            // Act
            var result = attribute.GetValidationResult("Not an integer", _validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void FoundingYear_ShouldPassValidation_WhenValueIsNull()
        {
            // Arrange
            var attribute = new FoundingYearValidationAttribute();

            // Act
            var result = attribute.GetValidationResult(null, _validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }
    }
}
