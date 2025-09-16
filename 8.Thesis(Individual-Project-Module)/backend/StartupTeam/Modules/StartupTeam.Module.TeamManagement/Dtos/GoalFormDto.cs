using StartupTeam.Module.TeamManagement.Models.Enums;
using StartupTeam.Module.TeamManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.TeamManagement.Dtos
{
    public class GoalFormDto
    {
        public Guid? Id { get; set; }

        public Guid TeamId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DueDateValidation]
        public DateTime DueDate { get; set; }

        [Required]
        public GoalStatus Status { get; set; }
    }
}
