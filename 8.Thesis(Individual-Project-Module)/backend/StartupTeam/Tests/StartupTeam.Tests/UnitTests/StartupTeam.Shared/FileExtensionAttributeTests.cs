using Microsoft.AspNetCore.Http;
using Moq;
using StartupTeam.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Shared
{
    public class FileExtensionAttributeTests
    {
        [Fact]
        public void FileExtension_ShouldPassValidation_WhenExtensionIsValid()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var validExtensions = new[] { ".pdf", ".docx" };
            var attribute = new FileExtensionAttribute(validExtensions);

            // Create a mock file with a valid extension
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("document.pdf");

            // Act
            var result = attribute.GetValidationResult(mockFile.Object, validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void FileExtension_ShouldFailValidation_WhenExtensionIsInvalid()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var validExtensions = new[] { ".pdf", ".docx" };
            var attribute = new FileExtensionAttribute(validExtensions);

            // Create a mock file with an invalid extension
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("image.png");

            // Act
            var result = attribute.GetValidationResult(mockFile.Object, validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Invalid file type. Allowed types are: .pdf, .docx", result!.ErrorMessage);
        }

        [Fact]
        public void FileExtension_ShouldPassValidation_WhenNoFileIsProvided()
        {
            // Arrange
            var validationContext = new ValidationContext(new object());
            var validExtensions = new[] { ".pdf", ".docx" };
            var attribute = new FileExtensionAttribute(validExtensions);

            // Act
            var result = attribute.GetValidationResult(null, validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }
    }
}
