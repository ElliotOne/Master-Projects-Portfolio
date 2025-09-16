using StartupTeam.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.UserManagement.Dtos
{
    public class ProfileFormDto
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? PhoneNumber { get; set; }

        [Required]
        [MaxLength(256)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string LastName { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }

        [FileExtension(new[] { ".jpeg", ".jpg", ".png" })]
        public IFormFile? ProfilePictureFile { get; set; }
    }
}
