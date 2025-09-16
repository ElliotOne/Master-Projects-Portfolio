namespace StartupTeam.Module.TeamManagement.Dtos
{
    public class MilestoneDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string GoalTitle { get; set; } = string.Empty;
    }
}
