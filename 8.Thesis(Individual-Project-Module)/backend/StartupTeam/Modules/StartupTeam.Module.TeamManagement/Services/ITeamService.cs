using StartupTeam.Module.TeamManagement.Dtos;

namespace StartupTeam.Module.TeamManagement.Services
{
    public interface ITeamService
    {
        Task<IEnumerable<GoalDto>> GetGoalsByTeamIdAsync(Guid teamId);
        Task<GoalFormDto?> GetGoalFormByIdAsync(Guid id);
        Task<bool> CreateGoalAsync(GoalFormDto goalFormDto);
        Task<bool> UpdateGoalAsync(GoalFormDto goalFormDto);
        Task<bool> DeleteGoalAsync(Guid id);

        Task<IEnumerable<MilestoneDto>> GetMilestonesByTeamIdAsync(Guid teamId);
        Task<MilestoneFromDto?> GetMilestoneFormByIdAsync(Guid id);
        Task<bool> CreateMilestoneAsync(MilestoneFromDto milestoneFromDto);
        Task<bool> UpdateMilestoneAsync(MilestoneFromDto milestoneFromDto);
        Task<bool> DeleteMilestoneAsync(Guid id);
        Task<bool> IsMilestoneDueDateValid(MilestoneFromDto milestoneFromDto);

        Task<IEnumerable<TeamMemberDto>> GetTeamMembersByTeamIdAsync(Guid teamId);
        Task<TeamMemberFormDto?> GetTeamMemberFormByIdAsync(Guid id);
        Task<bool> CreateTeamMemberAsync(TeamMemberFormDto teamMemberFormDto);
        Task<bool> UpdateTeamMemberAsync(TeamMemberFormDto teamMemberFormDto);
        Task<bool> DeleteTeamMemberAsync(Guid id);
        Task<bool> IsIndividualInTeamWithRole(Guid teamId, Guid individualId, Guid roleId);

        Task<IEnumerable<TeamRoleDto>> GetTeamRolesByTeamIdAsync(Guid teamId);
        Task<TeamRoleFormDto?> GetTeamRoleFormByIdAsync(Guid id);
        Task<bool> CreateTeamRoleAsync(TeamRoleFormDto teamRoleFormDto);
        Task<bool> UpdateTeamRoleAsync(TeamRoleFormDto teamRoleFormDto);
        Task<bool> DeleteTeamRoleAsync(Guid id);

        Task<IEnumerable<TeamDto>> GetTeamsByFounderIdAsync(Guid founderId);
        Task<IEnumerable<TeamDto>> GetTeamsByIndividualIdAsync(Guid individualId);
        Task<TeamDetailDto?> GetTeamByIdAsync(Guid id);
        Task<TeamFormDto?> GetTeamFormByIdAsync(Guid id);
        Task<bool> CreateTeamAsync(TeamFormDto teamFormDto);
        Task<bool> UpdateTeamAsync(TeamFormDto teamFormDto);
        Task<bool> DeleteTeamAsync(Guid id);
        Task<bool> IsIndividualMemberOfTeam(Guid teamId, Guid individualId);
    }
}
