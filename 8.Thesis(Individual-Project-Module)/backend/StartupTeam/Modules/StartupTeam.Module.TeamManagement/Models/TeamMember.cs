namespace StartupTeam.Module.TeamManagement.Models
{
    public class TeamMember
    {
        public Guid Id { get; set; }

        // Skilled Individual Id
        public Guid UserId { get; set; }

        public Guid TeamRoleId { get; set; }

        public TeamRole? TeamRole { get; set; }

        public Guid TeamId { get; set; }

        public Team? Team { get; set; }
    }
}
