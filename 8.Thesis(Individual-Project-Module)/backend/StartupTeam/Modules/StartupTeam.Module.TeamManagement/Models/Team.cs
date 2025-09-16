namespace StartupTeam.Module.TeamManagement.Models
{
    public class Team
    {
        public Guid Id { get; set; }

        // Startup Founder Id
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

        public ICollection<TeamRole> TeamRoles { get; set; } = new List<TeamRole>();

        public ICollection<Goal> Goals { get; set; } = new List<Goal>();

        public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    }
}
