using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.TeamManagement.Dtos
{
    public class TeamRoleFormDto
    {
        public Guid? Id { get; set; }

        public Guid TeamId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
