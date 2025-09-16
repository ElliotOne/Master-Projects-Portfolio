using StartupTeam.Shared.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.UserManagement.Dtos
{
    public class CompleteSignUpDto
    {
        [Required]
        [MaxLength(256)]
        [RegularExpression(RegexConstants.Email)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(256, MinimumLength = 6)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;

        public bool ExternalSignIn { get; set; }
    }
}
