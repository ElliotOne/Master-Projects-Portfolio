namespace StartupTeam.Module.TeamManagement.Dtos
{
    public class TeamMemberDto
    {
        public Guid Id { get; set; }
        public string IndividualUserName { get; set; } = string.Empty;
        public string IndividualFullName { get; set; } = string.Empty;
        public string TeamRoleName { get; set; } = string.Empty;
    }
}
