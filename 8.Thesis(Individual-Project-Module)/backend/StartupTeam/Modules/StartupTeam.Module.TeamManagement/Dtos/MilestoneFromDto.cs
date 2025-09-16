using StartupTeam.Module.TeamManagement.Models.Enums;
using StartupTeam.Module.TeamManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.TeamManagement.Dtos
{
    public class MilestoneFromDto
    {
        public Guid? Id { get; set; }

        public Guid TeamId { get; set; }

        public Guid? GoalId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DueDateValidation]
        public DateTime DueDate { get; set; }

        [Required]
        public MilestoneStatus Status { get; set; }
    }
}
