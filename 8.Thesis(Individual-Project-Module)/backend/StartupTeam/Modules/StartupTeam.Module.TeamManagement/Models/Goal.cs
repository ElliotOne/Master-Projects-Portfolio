using StartupTeam.Module.TeamManagement.Models.Enums;

namespace StartupTeam.Module.TeamManagement.Models
{
    public class Goal
    {
        public Guid Id { get; set; }

        public Guid TeamId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public GoalStatus Status { get; set; }

        public Team? Team { get; set; }
    }
}
