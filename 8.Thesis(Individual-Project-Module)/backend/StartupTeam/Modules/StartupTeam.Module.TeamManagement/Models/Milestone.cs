using StartupTeam.Module.TeamManagement.Models.Enums;

namespace StartupTeam.Module.TeamManagement.Models
{
    public class Milestone
    {
        public Guid Id { get; set; }

        public Guid TeamId { get; set; }

        public Guid? GoalId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public MilestoneStatus Status { get; set; }

        public Team? Team { get; set; }

        public Goal? Goal { get; set; }
    }
}
