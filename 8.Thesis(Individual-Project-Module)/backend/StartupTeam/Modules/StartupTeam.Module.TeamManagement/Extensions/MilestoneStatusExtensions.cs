using StartupTeam.Module.TeamManagement.Models.Enums;

namespace StartupTeam.Module.TeamManagement.Extensions
{
    public static class MilestoneStatusExtensions
    {
        public static string ToFriendlyString(this MilestoneStatus status)
        {
            return status switch
            {
                MilestoneStatus.Pending => "Pending",
                MilestoneStatus.InProgress => "In Progress",
                MilestoneStatus.Completed => "Completed",
                MilestoneStatus.Overdue => "Overdue",
                _ => status.ToString()
            };
        }
    }
}
