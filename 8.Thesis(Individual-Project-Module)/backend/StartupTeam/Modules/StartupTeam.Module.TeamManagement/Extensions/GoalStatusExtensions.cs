using StartupTeam.Module.TeamManagement.Models.Enums;

namespace StartupTeam.Module.TeamManagement.Extensions
{
    public static class GoalStatusExtensions
    {
        public static string ToFriendlyString(this GoalStatus status)
        {
            return status switch
            {
                GoalStatus.NotStarted => "Not Started",
                GoalStatus.InProgress => "In Progress",
                GoalStatus.Completed => "Completed",
                GoalStatus.OnHold => "On Hold",
                _ => status.ToString()
            };
        }
    }
}
