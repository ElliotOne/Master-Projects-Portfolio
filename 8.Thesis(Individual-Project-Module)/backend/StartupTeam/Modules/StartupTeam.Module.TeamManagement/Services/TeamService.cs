using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.TeamManagement.Data;
using StartupTeam.Module.TeamManagement.Dtos;
using StartupTeam.Module.TeamManagement.Extensions;
using StartupTeam.Module.TeamManagement.Models;
using StartupTeam.Module.UserManagement.Services;

namespace StartupTeam.Module.TeamManagement.Services
{
    public class TeamService : ITeamService
    {
        private readonly TeamManagementDbContext _context;
        private readonly IUserService _userService;

        public TeamService(
            TeamManagementDbContext context,
            IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IEnumerable<GoalDto>> GetGoalsByTeamIdAsync(Guid teamId)
        {
            return await _context.Goals
                .Where(g => g.TeamId == teamId)
                .Select(g => new GoalDto()
                {
                    Id = g.Id,
                    Title = g.Title,
                    Description = g.Description,
                    DueDate = g.DueDate,
                    Status = g.Status.ToFriendlyString()
                })
                .ToListAsync();
        }

        public async Task<GoalFormDto?> GetGoalFormByIdAsync(Guid id)
        {
            var goal = await _context.Goals.FindAsync(id);

            if (goal == null)
            {
                return null;
            }

            var goalFormDto = new GoalFormDto()
            {
                Id = goal.Id,
                TeamId = goal.TeamId,
                Title = goal.Title,
                Description = goal.Description,
                DueDate = goal.DueDate,
                Status = goal.Status
            };

            return goalFormDto;
        }

        public async Task<bool> CreateGoalAsync(GoalFormDto goalFormDto)
        {
            var goal = new Goal()
            {
                TeamId = goalFormDto.TeamId,
                Title = goalFormDto.Title,
                Description = goalFormDto.Description,
                DueDate = goalFormDto.DueDate,
                Status = goalFormDto.Status
            };

            _context.Goals.Add(goal);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateGoalAsync(GoalFormDto goalFormDto)
        {
            var goal = await _context.Goals.FindAsync(goalFormDto.Id);

            if (goal == null)
            {
                return false;
            }

            goal.Title = goalFormDto.Title;
            goal.Description = goalFormDto.Description;
            goal.DueDate = goalFormDto.DueDate;
            goal.Status = goalFormDto.Status;

            _context.Entry(goal).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteGoalAsync(Guid id)
        {
            var goal = await _context.Goals.FindAsync(id);

            if (goal == null)
            {
                return false;
            }

            // Remove all milestones related to the goal
            var milestones = _context.Milestones
                .Where(m => m.GoalId == id)
                .AsQueryable();

            _context.Milestones.RemoveRange(milestones);
            _context.Goals.Remove(goal);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<MilestoneDto>> GetMilestonesByTeamIdAsync(Guid teamId)
        {
            return await _context.Milestones
                .Include(m => m.Goal)
                .Where(m => m.TeamId == teamId)
                .Select(m => new MilestoneDto()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    DueDate = m.DueDate,
                    Status = m.Status.ToFriendlyString(),
                    GoalTitle = m.Goal == null ? string.Empty : m.Goal.Title
                })
                .ToListAsync();
        }

        public async Task<MilestoneFromDto?> GetMilestoneFormByIdAsync(Guid id)
        {
            var milestone = await _context.Milestones.FindAsync(id);

            if (milestone == null)
            {
                return null;
            }

            var milestoneFromDto = new MilestoneFromDto()
            {
                Id = milestone.Id,
                TeamId = milestone.TeamId,
                GoalId = milestone.GoalId,
                Title = milestone.Title,
                Description = milestone.Description,
                DueDate = milestone.DueDate,
                Status = milestone.Status
            };

            return milestoneFromDto;
        }

        public async Task<bool> CreateMilestoneAsync(MilestoneFromDto milestoneFromDto)
        {
            var milestone = new Milestone()
            {
                TeamId = milestoneFromDto.TeamId,
                GoalId = milestoneFromDto.GoalId,
                Title = milestoneFromDto.Title,
                Description = milestoneFromDto.Description,
                DueDate = milestoneFromDto.DueDate,
                Status = milestoneFromDto.Status
            };

            _context.Milestones.Add(milestone);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateMilestoneAsync(MilestoneFromDto milestoneFromDto)
        {
            var milestone = await _context.Milestones.FindAsync(milestoneFromDto.Id);

            if (milestone == null)
            {
                return false;
            }

            milestone.GoalId = milestoneFromDto.GoalId;
            milestone.Title = milestoneFromDto.Title;
            milestone.Description = milestoneFromDto.Description;
            milestone.DueDate = milestoneFromDto.DueDate;
            milestone.Status = milestoneFromDto.Status;

            _context.Entry(milestone).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMilestoneAsync(Guid id)
        {
            var milestone = await _context.Milestones.FindAsync(id);

            if (milestone == null)
            {
                return false;
            }

            _context.Milestones.Remove(milestone);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsMilestoneDueDateValid(MilestoneFromDto milestoneFromDto)
        {
            if (milestoneFromDto.GoalId == null)
            {
                return true;
            }

            var goal = await _context.Goals.FindAsync(milestoneFromDto.GoalId);

            if (goal == null)
            {
                return true;
            }

            return milestoneFromDto.DueDate <= goal.DueDate;
        }

        public async Task<IEnumerable<TeamMemberDto>> GetTeamMembersByTeamIdAsync(Guid teamId)
        {
            var teamMembers = await _context.TeamMembers
                .Where(tm => tm.TeamId == teamId)
                .Include(tm => tm.TeamRole)
                .ToListAsync();

            var teamMemberDtos = new List<TeamMemberDto>();

            foreach (var tm in teamMembers)
            {
                // Fetch the individual for each team member
                var individual = await _userService.FindByIdAsync(tm.UserId);

                var teamMemberDto = new TeamMemberDto()
                {
                    Id = tm.Id,
                    IndividualUserName = individual?.UserName ?? string.Empty,
                    IndividualFullName = individual == null ?
                        string.Empty : $"{individual.FirstName} {individual.LastName}",
                    TeamRoleName = tm.TeamRole!.Name
                };

                teamMemberDtos.Add(teamMemberDto);
            }

            return teamMemberDtos;
        }

        public async Task<TeamMemberFormDto?> GetTeamMemberFormByIdAsync(Guid id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);

            if (teamMember == null)
            {
                return null;
            }

            var individual = await _userService.FindByIdAsync(teamMember.UserId);

            var teamMemberFormDto = new TeamMemberFormDto
            {
                Id = teamMember.Id,
                UserId = teamMember.UserId,
                TeamRoleId = teamMember.TeamRoleId,
                TeamId = teamMember.TeamId,
                IndividualFullName = individual == null ?
                    string.Empty : $"{individual.FirstName} {individual.LastName}",
            };

            return teamMemberFormDto;
        }

        public async Task<bool> CreateTeamMemberAsync(TeamMemberFormDto teamMemberFormDto)
        {
            var teamMember = new TeamMember
            {
                UserId = teamMemberFormDto.UserId,
                TeamRoleId = teamMemberFormDto.TeamRoleId,
                TeamId = teamMemberFormDto.TeamId
            };

            _context.TeamMembers.Add(teamMember);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTeamMemberAsync(TeamMemberFormDto teamMemberFormDto)
        {
            var teamMember = await _context.TeamMembers.FindAsync(teamMemberFormDto.Id);

            if (teamMember == null)
            {
                return false;
            }

            teamMember.TeamRoleId = teamMemberFormDto.TeamRoleId;

            _context.Entry(teamMember).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTeamMemberAsync(Guid id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);

            if (teamMember == null)
            {
                return false;
            }

            _context.TeamMembers.Remove(teamMember);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsIndividualInTeamWithRole(Guid teamId, Guid individualId, Guid roleId)
        {
            return await _context.TeamMembers
                .AnyAsync(tm =>
                    tm.UserId == individualId
                    && tm.TeamId == teamId
                    && tm.TeamRoleId == roleId);
        }

        public async Task<IEnumerable<TeamRoleDto>> GetTeamRolesByTeamIdAsync(Guid teamId)
        {
            var teamRoles = await _context.TeamRoles
                .Where(tr => tr.TeamId == teamId)
                .ToListAsync();

            return teamRoles.Select(
                role => new TeamRoleDto()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                });
        }

        public async Task<TeamRoleFormDto?> GetTeamRoleFormByIdAsync(Guid id)
        {
            var teamRole = await _context.TeamRoles.FindAsync(id);

            if (teamRole == null)
            {
                return null;
            }

            var teamRoleFormDto = new TeamRoleFormDto()
            {
                Id = teamRole.Id,
                TeamId = teamRole.TeamId,
                Name = teamRole.Name,
                Description = teamRole.Description
            };

            return teamRoleFormDto;
        }

        public async Task<bool> CreateTeamRoleAsync(TeamRoleFormDto teamRoleFormDto)
        {
            var teamRole = new TeamRole()
            {
                Name = teamRoleFormDto.Name,
                Description = teamRoleFormDto.Description,
                TeamId = teamRoleFormDto.TeamId
            };

            _context.TeamRoles.Add(teamRole);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTeamRoleAsync(TeamRoleFormDto teamRoleFormDto)
        {
            var teamRole = await _context.TeamRoles.FindAsync(teamRoleFormDto.Id);

            if (teamRole == null)
            {
                return false;
            }

            teamRole.Name = teamRoleFormDto.Name;
            teamRole.Description = teamRoleFormDto.Description;

            _context.Entry(teamRole).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTeamRoleAsync(Guid id)
        {
            var teamRole = await _context.TeamRoles.FindAsync(id);

            if (teamRole == null)
            {
                return false;
            }

            // Remove all team members related to the team role+
            var teamMembers = _context.TeamMembers
                .Where(tm => tm.TeamRoleId == id)
                .AsQueryable();

            _context.TeamMembers.RemoveRange(teamMembers);
            _context.TeamRoles.Remove(teamRole);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TeamDto>> GetTeamsByFounderIdAsync(Guid founderId)
        {
            var teams = await _context.Teams
                .Where(t => t.UserId == founderId)
                .ToListAsync();

            return teams.Select(
                team => new TeamDto()
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description
                });
        }

        public async Task<IEnumerable<TeamDto>> GetTeamsByIndividualIdAsync(Guid individualId)
        {
            var teams = await _context.TeamMembers
                .Include(tm => tm.Team)
                .Where(tm => tm.UserId == individualId)
                .Select(tm => tm.Team)
                .Distinct()
                .ToListAsync();

            return teams.Select(
                team => new TeamDto()
                {
                    Id = team!.Id,
                    Name = team.Name,
                    Description = team.Description
                });
        }

        public async Task<TeamDetailDto?> GetTeamByIdAsync(Guid id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return null;
            }

            return new TeamDetailDto()
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                UserId = team.UserId
            };
        }

        public async Task<TeamFormDto?> GetTeamFormByIdAsync(Guid id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return null;
            }

            return new TeamFormDto()
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                UserId = team.UserId
            };
        }

        public async Task<bool> CreateTeamAsync(TeamFormDto teamFormDto)
        {
            var team = new Team
            {
                UserId = teamFormDto.UserId,
                Name = teamFormDto.Name,
                Description = teamFormDto.Description
            };

            _context.Teams.Add(team);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTeamAsync(TeamFormDto teamFormDto)
        {
            var team = await _context.Teams.FindAsync(teamFormDto.Id);

            if (team == null)
            {
                return false;
            }

            team.Name = teamFormDto.Name;
            team.Description = teamFormDto.Description;

            _context.Entry(team).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTeamAsync(Guid id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return false;
            }

            _context.Teams.Remove(team);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsIndividualMemberOfTeam(Guid teamId, Guid individualId)
        {
            return await _context.TeamMembers
                .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == individualId);
        }
    }
}
