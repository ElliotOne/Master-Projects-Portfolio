using Microsoft.EntityFrameworkCore;
using Moq;
using StartupTeam.Module.TeamManagement.Data;
using StartupTeam.Module.TeamManagement.Dtos;
using StartupTeam.Module.TeamManagement.Models;
using StartupTeam.Module.TeamManagement.Models.Enums;
using StartupTeam.Module.TeamManagement.Services;
using StartupTeam.Module.UserManagement.Services;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.TeamManagement.Services
{
    public class TeamServiceTests
    {
        private TeamService _teamService;
        private TeamManagementDbContext _dbContext;
        private Mock<IUserService> _userServiceMock;

        public void InitializeDatabase()
        {
            // Use a new InMemoryDatabase for each test, ensuring unique database name
            var options = new DbContextOptionsBuilder<TeamManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for each test
                .Options;

            _dbContext = new TeamManagementDbContext(options);
            _userServiceMock = new Mock<IUserService>();

            // Initialize TeamService with in-memory db and mock user service
            _teamService = new TeamService(_dbContext, _userServiceMock.Object);
        }

        private void SeedData()
        {
            var founderId = Guid.NewGuid();

            // Create the first team
            var techTeamId = Guid.NewGuid();
            var techTeam = new Team
            {
                Id = techTeamId,
                Name = "Tech Team",
                Description = "A team focused on tech",
                UserId = founderId
            };

            // Create the second team
            var devTeamId = Guid.NewGuid();
            var devTeam = new Team
            {
                Id = devTeamId,
                Name = "Dev Team",
                Description = "Development Team",
                UserId = founderId
            };

            // Add teams to the context
            _dbContext.Teams.AddRange(techTeam, devTeam);

            // Create goals for the first team
            var techTeamGoal = new Goal
            {
                Id = Guid.NewGuid(),
                TeamId = techTeamId,
                Title = "Goal 1",
                Description = "Complete the project",
                DueDate = DateTime.Now.AddDays(30),
                Status = GoalStatus.InProgress
            };

            // Create milestones for the first team
            var techTeamMilestone = new Milestone
            {
                Id = Guid.NewGuid(),
                TeamId = techTeamId,
                Title = "Milestone 1",
                Description = "First phase complete",
                DueDate = DateTime.Now.AddDays(15),
                Status = MilestoneStatus.InProgress
            };

            // Add goals and milestones to the context
            _dbContext.Goals.Add(techTeamGoal);
            _dbContext.Milestones.Add(techTeamMilestone);

            // Create team roles for both teams
            var techTeamRole = new TeamRole
            {
                Id = Guid.NewGuid(),
                Name = "Tech Lead",
                TeamId = techTeamId,
                Description = "Leads the tech efforts"
            };

            var devTeamRole = new TeamRole
            {
                Id = Guid.NewGuid(),
                Name = "Developer",
                TeamId = devTeamId,
                Description = "Responsible for coding"
            };

            // Add team roles to the context
            _dbContext.TeamRoles.AddRange(techTeamRole, devTeamRole);

            // Create team members for both teams
            var techTeamMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = techTeamId,
                TeamRoleId = techTeamRole.Id,
                UserId = Guid.NewGuid()
            };

            var devTeamMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = devTeamId,
                TeamRoleId = devTeamRole.Id,
                UserId = Guid.NewGuid()
            };

            // Add team members to the context
            _dbContext.TeamMembers.AddRange(techTeamMember, devTeamMember);

            // Save all changes
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetGoalsByTeamIdAsync_ShouldReturnGoals_WhenGoalsExist()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamId = _dbContext.Teams.First().Id;

            // Act
            var result = await _teamService.GetGoalsByTeamIdAsync(teamId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("Goal 1", result.First().Title);
        }

        [Fact]
        public async Task GetGoalFormByIdAsync_ShouldReturnFormDto_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var goalId = _dbContext.Goals.First().Id;

            // Act
            var result = await _teamService.GetGoalFormByIdAsync(goalId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Goal 1", result.Title);
        }

        [Fact]
        public async Task GetGoalFormByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            InitializeDatabase();

            var goalId = Guid.NewGuid();

            // Act
            var result = await _teamService.GetGoalFormByIdAsync(goalId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateGoalAsync_ShouldReturnTrue_WhenGoalCreated()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var formDto = new GoalFormDto
            {
                TeamId = Guid.NewGuid(),
                Title = "New Goal",
                Description = "Goal description",
                DueDate = DateTime.Now.AddDays(20),
                Status = GoalStatus.InProgress
            };

            // Act
            var result = await _teamService.CreateGoalAsync(formDto);

            // Assert
            Assert.True(result);
            Assert.Equal(2, _dbContext.Goals.Count());  // Assuming one goal was seeded
        }

        [Fact]
        public async Task UpdateGoalAsync_ShouldReturnTrue_WhenGoalUpdatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var goalId = _dbContext.Goals.First().Id;

            var formDto = new GoalFormDto
            {
                Id = goalId,
                Title = "Updated Goal Title",
                Description = "Updated description",
                DueDate = DateTime.Now.AddDays(15),
                Status = GoalStatus.Completed
            };

            // Act
            var result = await _teamService.UpdateGoalAsync(formDto);

            // Assert
            Assert.True(result);
            var updatedGoal = await _dbContext.Goals.FindAsync(goalId);
            Assert.Equal("Updated Goal Title", updatedGoal.Title);
        }

        [Fact]
        public async Task DeleteGoalAsync_ShouldReturnTrue_WhenGoalDeletedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var goalId = _dbContext.Goals.First().Id;

            // Act
            var result = await _teamService.DeleteGoalAsync(goalId);

            // Assert
            Assert.True(result);
            Assert.Null(await _dbContext.Goals.FindAsync(goalId));
        }

        [Fact]
        public async Task GetMilestonesByTeamIdAsync_ShouldReturnMilestones_WhenMilestonesExist()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamId = _dbContext.Teams.First().Id;

            // Act
            var result = await _teamService.GetMilestonesByTeamIdAsync(teamId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("Milestone 1", result.First().Title);
        }

        [Fact]
        public async Task GetMilestonesByTeamIdAsync_ShouldReturnEmptyList_WhenNoMilestonesExist()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid(); // No milestones for this team

            // Act
            var result = await _teamService.GetMilestonesByTeamIdAsync(teamId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMilestoneFormByIdAsync_ShouldReturnFormDto_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var milestoneId = _dbContext.Milestones.First().Id;

            // Act
            var result = await _teamService.GetMilestoneFormByIdAsync(milestoneId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Milestone 1", result.Title);
        }

        [Fact]
        public async Task GetMilestoneFormByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            InitializeDatabase();
            var milestoneId = Guid.NewGuid();

            // Act
            var result = await _teamService.GetMilestoneFormByIdAsync(milestoneId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateMilestoneAsync_ShouldReturnTrue_WhenMilestoneCreated()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var milestoneFormDto = new MilestoneFromDto
            {
                TeamId = Guid.NewGuid(),
                GoalId = Guid.NewGuid(),
                Title = "New Milestone",
                Description = "Milestone description",
                DueDate = DateTime.Now.AddDays(20),
                Status = MilestoneStatus.InProgress
            };

            // Act
            var result = await _teamService.CreateMilestoneAsync(milestoneFormDto);

            // Assert
            Assert.True(result);
            Assert.Equal(2, _dbContext.Milestones.Count());  // Assuming one milestone was seeded
        }

        [Fact]
        public async Task UpdateMilestoneAsync_ShouldReturnTrue_WhenMilestoneUpdatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var milestoneId = _dbContext.Milestones.First().Id;

            var formDto = new MilestoneFromDto
            {
                Id = milestoneId,
                Title = "Updated Milestone Title",
                Description = "Updated description",
                DueDate = DateTime.Now.AddDays(15),
                Status = MilestoneStatus.Completed
            };

            // Act
            var result = await _teamService.UpdateMilestoneAsync(formDto);

            // Assert
            Assert.True(result);
            var updatedMilestone = await _dbContext.Milestones.FindAsync(milestoneId);
            Assert.Equal("Updated Milestone Title", updatedMilestone.Title);
        }

        [Fact]
        public async Task DeleteMilestoneAsync_ShouldReturnTrue_WhenMilestoneDeletedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var milestoneId = _dbContext.Milestones.First().Id;

            // Act
            var result = await _teamService.DeleteMilestoneAsync(milestoneId);

            // Assert
            Assert.True(result);
            Assert.Null(await _dbContext.Milestones.FindAsync(milestoneId));
        }

        [Fact]
        public async Task DeleteMilestoneAsync_ShouldReturnFalse_WhenMilestoneNotFound()
        {
            // Arrange
            InitializeDatabase();
            var milestoneId = Guid.NewGuid(); // Milestone doesn't exist

            // Act
            var result = await _teamService.DeleteMilestoneAsync(milestoneId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTeamMembersByTeamIdAsync_ShouldReturnTeamMembers_WhenTeamMembersExist()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamId = _dbContext.Teams.First().Id;

            // Act
            var result = await _teamService.GetTeamMembersByTeamIdAsync(teamId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("Tech Lead", result.First().TeamRoleName);
        }

        [Fact]
        public async Task GetTeamMembersByTeamIdAsync_ShouldReturnEmptyList_WhenNoTeamMembersExist()
        {
            // Arrange
            InitializeDatabase();
            var teamId = Guid.NewGuid(); // No team members for this team

            // Act
            var result = await _teamService.GetTeamMembersByTeamIdAsync(teamId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTeamMemberFormByIdAsync_ShouldReturnFormDto_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamMemberId = _dbContext.TeamMembers.First().Id;

            // Act
            var result = await _teamService.GetTeamMemberFormByIdAsync(teamMemberId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(teamMemberId, result.Id);
        }

        [Fact]
        public async Task GetTeamMemberFormByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            InitializeDatabase();

            var teamMemberId = Guid.NewGuid(); // Team member doesn't exist

            // Act
            var result = await _teamService.GetTeamMemberFormByIdAsync(teamMemberId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTeamMemberAsync_ShouldReturnTrue_WhenTeamMemberCreatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamMemberFormDto = new TeamMemberFormDto
            {
                UserId = Guid.NewGuid(),
                TeamRoleId = Guid.NewGuid(),
                TeamId = _dbContext.Teams.First().Id
            };

            // Act
            var result = await _teamService.CreateTeamMemberAsync(teamMemberFormDto);

            // Assert
            Assert.True(result);
            Assert.Equal(3, _dbContext.TeamMembers.Count());  // Assuming one member was seeded
        }

        [Fact]
        public async Task UpdateTeamMemberAsync_ShouldReturnTrue_WhenTeamMemberUpdatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamMemberId = _dbContext.TeamMembers.First().Id;
            var teamMemberFormDto = new TeamMemberFormDto
            {
                Id = teamMemberId,
                TeamRoleId = Guid.NewGuid()
            };

            // Act
            var result = await _teamService.UpdateTeamMemberAsync(teamMemberFormDto);

            // Assert
            Assert.True(result);
            var updatedTeamMember = await _dbContext.TeamMembers.FindAsync(teamMemberId);
            Assert.Equal(teamMemberFormDto.TeamRoleId, updatedTeamMember.TeamRoleId);
        }

        [Fact]
        public async Task UpdateTeamMemberAsync_ShouldReturnFalse_WhenTeamMemberNotFound()
        {
            // Arrange
            InitializeDatabase();

            var teamMemberFormDto = new TeamMemberFormDto
            {
                Id = Guid.NewGuid(),  // Team member doesn't exist
                TeamRoleId = Guid.NewGuid()
            };

            // Act
            var result = await _teamService.UpdateTeamMemberAsync(teamMemberFormDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteTeamMemberAsync_ShouldReturnTrue_WhenTeamMemberDeletedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamMemberId = _dbContext.TeamMembers.First().Id;

            // Act
            var result = await _teamService.DeleteTeamMemberAsync(teamMemberId);

            // Assert
            Assert.True(result);
            Assert.Null(await _dbContext.TeamMembers.FindAsync(teamMemberId));
        }

        [Fact]
        public async Task DeleteTeamMemberAsync_ShouldReturnFalse_WhenTeamMemberNotFound()
        {
            // Arrange
            InitializeDatabase();

            var teamMemberId = Guid.NewGuid(); // Team member doesn't exist

            // Act
            var result = await _teamService.DeleteTeamMemberAsync(teamMemberId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTeamRolesByTeamIdAsync_ShouldReturnTeamRoles_WhenRolesExist()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamId = _dbContext.Teams.First().Id;

            // Act
            var result = await _teamService.GetTeamRolesByTeamIdAsync(teamId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("Tech Lead", result.First().Name);
        }

        [Fact]
        public async Task GetTeamRolesByTeamIdAsync_ShouldReturnEmptyList_WhenNoRolesExist()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid(); // No roles for this team

            // Act
            var result = await _teamService.GetTeamRolesByTeamIdAsync(teamId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTeamRoleFormByIdAsync_ShouldReturnFormDto_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var roleId = _dbContext.TeamRoles.First().Id;

            // Act
            var result = await _teamService.GetTeamRoleFormByIdAsync(roleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roleId, result.Id);
        }

        [Fact]
        public async Task GetTeamRoleFormByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            InitializeDatabase();

            var roleId = Guid.NewGuid(); // Role doesn't exist

            // Act
            var result = await _teamService.GetTeamRoleFormByIdAsync(roleId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTeamRoleAsync_ShouldReturnTrue_WhenRoleCreatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var teamRoleFormDto = new TeamRoleFormDto
            {
                Name = "Designer",
                Description = "UI/UX Designer",
                TeamId = _dbContext.Teams.First().Id
            };

            // Act
            var result = await _teamService.CreateTeamRoleAsync(teamRoleFormDto);

            // Assert
            Assert.True(result);
            Assert.Equal(3, _dbContext.TeamRoles.Count());  // Assuming one role was seeded
        }

        [Fact]
        public async Task UpdateTeamRoleAsync_ShouldReturnTrue_WhenRoleUpdatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var roleId = _dbContext.TeamRoles.First().Id;
            var teamRoleFormDto = new TeamRoleFormDto
            {
                Id = roleId,
                Name = "Lead Developer"
            };

            // Act
            var result = await _teamService.UpdateTeamRoleAsync(teamRoleFormDto);

            // Assert
            Assert.True(result);
            var updatedRole = await _dbContext.TeamRoles.FindAsync(roleId);
            Assert.Equal("Lead Developer", updatedRole.Name);
        }

        [Fact]
        public async Task UpdateTeamRoleAsync_ShouldReturnFalse_WhenRoleNotFound()
        {
            // Arrange
            InitializeDatabase();

            var teamRoleFormDto = new TeamRoleFormDto
            {
                Id = Guid.NewGuid(),  // Role doesn't exist
                Name = "Lead Developer"
            };

            // Act
            var result = await _teamService.UpdateTeamRoleAsync(teamRoleFormDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteTeamRoleAsync_ShouldReturnTrue_WhenRoleDeletedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var roleId = _dbContext.TeamRoles.First().Id;

            // Act
            var result = await _teamService.DeleteTeamRoleAsync(roleId);

            // Assert
            Assert.True(result);
            Assert.Null(await _dbContext.TeamRoles.FindAsync(roleId));
        }

        [Fact]
        public async Task DeleteTeamRoleAsync_ShouldReturnFalse_WhenRoleNotFound()
        {
            // Arrange
            InitializeDatabase();
            var roleId = Guid.NewGuid(); // Role doesn't exist

            // Act
            var result = await _teamService.DeleteTeamRoleAsync(roleId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteTeamRoleAsync_ShouldReturnTrue_WhenRoleHasMembers()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var roleId = _dbContext.TeamRoles.First().Id;

            // Try to delete a role that has associated team members
            var result = await _teamService.DeleteTeamRoleAsync(roleId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetTeamsByFounderIdAsync_ShouldReturnTeams_WhenFounderHasTeams()
        {
            // Arrange
            InitializeDatabase();

            var founderId = Guid.NewGuid();
            _dbContext.Teams.Add(new Team { Id = Guid.NewGuid(), UserId = founderId, Name = "Team A", Description = "Team A Description" });
            _dbContext.Teams.Add(new Team { Id = Guid.NewGuid(), UserId = founderId, Name = "Team B", Description = "Team B Description" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _teamService.GetTeamsByFounderIdAsync(founderId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Name == "Team A");
            Assert.Contains(result, t => t.Name == "Team B");
        }

        [Fact]
        public async Task GetTeamsByFounderIdAsync_ShouldReturnEmpty_WhenNoTeamsForFounder()
        {
            // Arrange
            InitializeDatabase();

            var founderId = Guid.NewGuid();

            // Act
            var result = await _teamService.GetTeamsByFounderIdAsync(founderId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTeamsByIndividualIdAsync_ShouldReturnTeams_WhenIndividualIsInTeams()
        {
            // Arrange
            InitializeDatabase();

            var individualId = Guid.NewGuid();
            var team = new Team { Id = Guid.NewGuid(), Name = "Team A", Description = "Team A Description" };
            _dbContext.Teams.Add(team);
            _dbContext.TeamMembers.Add(new TeamMember { TeamId = team.Id, UserId = individualId });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _teamService.GetTeamsByIndividualIdAsync(individualId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Team A", result.First().Name);
        }

        [Fact]
        public async Task GetTeamsByIndividualIdAsync_ShouldReturnEmpty_WhenNoTeamsForIndividual()
        {
            // Arrange
            InitializeDatabase();

            var individualId = Guid.NewGuid();

            // Act
            var result = await _teamService.GetTeamsByIndividualIdAsync(individualId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTeamByIdAsync_ShouldReturnTeamDetail_WhenTeamExists()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid();
            _dbContext.Teams.Add(new Team { Id = teamId, Name = "Team A", Description = "Team A Description", UserId = Guid.NewGuid() });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _teamService.GetTeamByIdAsync(teamId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Team A", result.Name);
        }

        [Fact]
        public async Task GetTeamByIdAsync_ShouldReturnNull_WhenTeamDoesNotExist()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid();

            // Act
            var result = await _teamService.GetTeamByIdAsync(teamId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTeamFormByIdAsync_ShouldReturnTeamForm_WhenTeamExists()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid();
            _dbContext.Teams.Add(new Team { Id = teamId, Name = "Team A", Description = "Team A Description", UserId = Guid.NewGuid() });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _teamService.GetTeamFormByIdAsync(teamId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Team A", result.Name);
        }

        [Fact]
        public async Task GetTeamFormByIdAsync_ShouldReturnNull_WhenTeamDoesNotExist()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid();

            // Act
            var result = await _teamService.GetTeamFormByIdAsync(teamId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTeamAsync_ShouldReturnTrue_WhenTeamCreatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();

            var teamFormDto = new TeamFormDto { Name = "New Team", Description = "New Team Description", UserId = Guid.NewGuid() };

            // Act
            var result = await _teamService.CreateTeamAsync(teamFormDto);

            // Assert
            Assert.True(result);
            var createdTeam = await _dbContext.Teams.FirstOrDefaultAsync(t => t.Name == "New Team");
            Assert.NotNull(createdTeam);
        }

        [Fact]
        public async Task UpdateTeamAsync_ShouldReturnTrue_WhenTeamUpdatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid();
            _dbContext.Teams.Add(new Team { Id = teamId, Name = "Team A", Description = "Team A Description", UserId = Guid.NewGuid() });
            await _dbContext.SaveChangesAsync();

            var teamFormDto = new TeamFormDto { Id = teamId, Name = "Updated Team", Description = "Updated Description" };

            // Act
            var result = await _teamService.UpdateTeamAsync(teamFormDto);

            // Assert
            Assert.True(result);
            var updatedTeam = await _dbContext.Teams.FindAsync(teamId);
            Assert.Equal("Updated Team", updatedTeam.Name);
        }

        [Fact]
        public async Task UpdateTeamAsync_ShouldReturnFalse_WhenTeamNotFound()
        {
            // Arrange
            InitializeDatabase();

            var teamFormDto = new TeamFormDto { Id = Guid.NewGuid(), Name = "Non-Existent Team" };

            // Act
            var result = await _teamService.UpdateTeamAsync(teamFormDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteTeamAsync_ShouldReturnTrue_WhenTeamDeletedSuccessfully()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid();
            _dbContext.Teams.Add(new Team { Id = teamId, Name = "Team A", Description = "Team A Description", UserId = Guid.NewGuid() });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _teamService.DeleteTeamAsync(teamId);

            // Assert
            Assert.True(result);
            Assert.Null(await _dbContext.Teams.FindAsync(teamId));
        }

        [Fact]
        public async Task DeleteTeamAsync_ShouldReturnFalse_WhenTeamNotFound()
        {
            // Arrange
            InitializeDatabase();

            var teamId = Guid.NewGuid(); // Team doesn't exist

            // Act
            var result = await _teamService.DeleteTeamAsync(teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsIndividualMemberOfTeam_ShouldReturnTrue_WhenIndividualIsMemberOfTeam()
        {
            // Arrange
            InitializeDatabase();

            var individualId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            _dbContext.Teams.Add(new Team { Id = teamId, Name = "Team A" });
            _dbContext.TeamMembers.Add(new TeamMember { TeamId = teamId, UserId = individualId });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _teamService.IsIndividualMemberOfTeam(teamId, individualId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsIndividualMemberOfTeam_ShouldReturnFalse_WhenIndividualIsNotMemberOfTeam()
        {
            // Arrange
            InitializeDatabase();

            var individualId = Guid.NewGuid();
            var teamId = Guid.NewGuid();

            // Act
            var result = await _teamService.IsIndividualMemberOfTeam(teamId, individualId);

            // Assert
            Assert.False(result);
        }
    }
}
