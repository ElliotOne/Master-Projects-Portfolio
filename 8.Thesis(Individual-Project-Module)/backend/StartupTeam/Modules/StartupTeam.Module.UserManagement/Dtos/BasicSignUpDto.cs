using StartupTeam.Shared.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.UserManagement.Dtos
{
    public class BasicSignUpDto
    {
        [Required]
        [MaxLength(256)]
        [RegularExpression(RegexConstants.Email)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
