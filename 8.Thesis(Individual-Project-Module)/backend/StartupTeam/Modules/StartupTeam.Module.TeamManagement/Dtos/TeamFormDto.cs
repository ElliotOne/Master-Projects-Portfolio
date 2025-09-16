using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.TeamManagement.Dtos
{
    public class TeamFormDto
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
