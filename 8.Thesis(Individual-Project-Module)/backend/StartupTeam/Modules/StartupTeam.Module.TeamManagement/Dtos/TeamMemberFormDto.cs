using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.TeamManagement.Dtos
{
    public class TeamMemberFormDto
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TeamRoleId { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        public string IndividualFullName { get; set; } = string.Empty;
    }
}
