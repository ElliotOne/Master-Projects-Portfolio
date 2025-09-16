namespace StartupTeam.Module.TeamManagement.Models
{
    public class TeamRole
    {
        public Guid Id { get; set; }

        public Guid TeamId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Team? Team { get; set; }

        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    }
}
