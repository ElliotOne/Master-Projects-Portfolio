using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using StartupTeam.Module.JobManagement.Data;
using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Models;
using StartupTeam.Module.JobManagement.Models.Enums;
using StartupTeam.Module.JobManagement.Services;
using StartupTeam.Module.UserManagement.Models;
using StartupTeam.Module.UserManagement.Services;
using StartupTeam.Shared.Services;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.JobManagement.Services
{
    public class JobServiceTests
    {
        private JobService _jobService;
        private JobManagementDbContext _dbContext;
        private Mock<IBlobStorageService> _blobStorageServiceMock;
        private Mock<IUserService> _userServiceMock;

        private void InitializeDatabase()
        {
            // Use a new InMemoryDatabase for each test, ensuring unique database name
            var options = new DbContextOptionsBuilder<JobManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for each test
                .Options;

            _dbContext = new JobManagementDbContext(options);
            _blobStorageServiceMock = new Mock<IBlobStorageService>();
            _userServiceMock = new Mock<IUserService>();

            _jobService = new JobService(_dbContext, _blobStorageServiceMock.Object, _userServiceMock.Object);
        }

        private void SeedData()
        {
            var additionalJobId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Add test data
            _dbContext.JobAdvertisements.AddRange(new List<JobAdvertisement>
            {
                new JobAdvertisement { Id = Guid.NewGuid(), JobTitle = "Active Job", ApplicationDeadline = DateTime.Now.AddDays(10) },
                new JobAdvertisement { Id = Guid.NewGuid(), JobTitle = "Expired Job", ApplicationDeadline = DateTime.Now.AddDays(-1) },
                new JobAdvertisement { Id = additionalJobId, JobTitle = "Additional Job with Application", ApplicationDeadline = DateTime.Now.AddDays(-1), UserId = userId }
            });

            _dbContext.JobApplications.AddRange(new List<JobApplication>
            {
                new JobApplication
                {
                    JobAdvertisementId = additionalJobId,
                    UserId = userId,
                    CVUrl = "fake-cv-url-additional",
                    ApplicationDate = DateTime.Now
                }
            });

            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetJobAdvertisementsAsync_ShouldReturnActiveJobs()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            // Act
            var result = await _jobService.GetJobAdvertisementsAsync();

            // Assert
            Assert.Single(result); // Only one active job should be returned
            Assert.Equal("Active Job", result.First().JobTitle);
        }

        [Fact]
        public async Task GetJobAdvertisementByIdAsync_ShouldReturnDetail_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobId = Guid.NewGuid();
            var individualId = Guid.NewGuid();

            var job = new JobAdvertisement
            {
                Id = jobId,
                JobTitle = "Software Engineer",
                StartupName = "Tech Corp"
            };

            _dbContext.JobAdvertisements.Add(job);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.GetJobAdvertisementByIdAsync(jobId, individualId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Software Engineer", result.JobTitle);
        }


        [Fact]
        public async Task GetJobAdvertisementFormByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobId = Guid.NewGuid();

            // Act
            var result = await _jobService.GetJobAdvertisementFormByIdAsync(jobId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetJobAdvertisementFormByIdAsync_ShouldReturnJobForm_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobId = Guid.NewGuid();
            var job = new JobAdvertisement
            {
                Id = jobId,
                JobTitle = "Software Engineer",
                StartupName = "Tech Corp",
                ApplicationDeadline = DateTime.Now.AddMonths(1),
                UserId = Guid.NewGuid()
            };

            // Add the job advertisement to the InMemory database
            await _dbContext.JobAdvertisements.AddAsync(job);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.GetJobAdvertisementFormByIdAsync(jobId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Software Engineer", result.JobTitle);
            Assert.Equal("Tech Corp", result.StartupName);
        }

        [Fact]
        public async Task CreateJobAdvertisementAsync_ShouldReturnTrue_WhenSuccess()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var userId = Guid.NewGuid();
            var formDto = new JobAdvertisementFormDto
            {
                JobTitle = "Software Engineer",
                StartupName = "Tech Corp",
                ApplicationDeadline = DateTime.Now.AddMonths(1),
                UserId = userId
            };

            // Act
            var result = await _jobService.CreateJobAdvertisementAsync(formDto, userId);

            // Assert
            Assert.True(result);

            // Verify that the job was added to the in-memory database
            var createdJob = await _dbContext.JobAdvertisements.FirstOrDefaultAsync(j => j.JobTitle == "Software Engineer");
            Assert.NotNull(createdJob);
            Assert.Equal("Software Engineer", createdJob.JobTitle);
            Assert.Equal("Tech Corp", createdJob.StartupName);
            Assert.Equal(userId, createdJob.UserId);
        }

        [Fact]
        public async Task UpdateJobAdvertisementAsync_ShouldReturnFalse_WhenJobNotFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var formDto = new JobAdvertisementFormDto
            {
                Id = Guid.NewGuid(), // Non-existing Job ID
                JobTitle = "Updated Title"
            };

            // Act
            var result = await _jobService.UpdateJobAdvertisementAsync(formDto);

            // Assert
            Assert.False(result); // Should return false since the job doesn't exist
        }

        [Fact]
        public async Task UpdateJobAdvertisementAsync_ShouldReturnTrue_WhenUpdatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobId = Guid.NewGuid();
            var job = new JobAdvertisement
            {
                Id = jobId,
                JobTitle = "Original Title",
                StartupName = "Original Startup"
            };

            // Add the job to the in-memory database
            _dbContext.JobAdvertisements.Add(job);
            await _dbContext.SaveChangesAsync();

            var formDto = new JobAdvertisementFormDto
            {
                Id = jobId, // Use the existing Job ID
                JobTitle = "Updated Title",
                StartupName = "Updated Startup"
            };

            // Act
            var result = await _jobService.UpdateJobAdvertisementAsync(formDto);

            // Assert
            Assert.True(result); // Should return true since the job was updated
            var updatedJob = await _dbContext.JobAdvertisements.FindAsync(jobId);
            Assert.Equal("Updated Title", updatedJob?.JobTitle); // Check that the title was updated
            Assert.Equal("Updated Startup", updatedJob?.StartupName); // Check that the startup name was updated
        }

        [Fact]
        public async Task UpdateJobAdvertisementAsync_ShouldReturnTrue_WhenNoChangesMade()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobId = Guid.NewGuid();
            var job = new JobAdvertisement
            {
                Id = jobId,
                JobTitle = "Original Title",
                StartupName = "Original Startup"
            };

            _dbContext.JobAdvertisements.Add(job);
            await _dbContext.SaveChangesAsync();

            var formDto = new JobAdvertisementFormDto
            {
                Id = jobId,
                JobTitle = "Original Title", // No changes
                StartupName = "Original Startup" // No changes
            };

            // Act
            var result = await _jobService.UpdateJobAdvertisementAsync(formDto);

            // Assert
            Assert.True(result); // Should return true, but no changes should have been made
        }

        [Fact]
        public async Task DeleteJobAdvertisementAsync_ShouldReturnFalse_WhenJobNotFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();
            var jobId = Guid.NewGuid(); // Use a non-existing JobId

            // Act
            var result = await _jobService.DeleteJobAdvertisementAsync(jobId);

            // Assert
            Assert.False(result); // Should return false since the job doesn't exist
        }

        [Fact]
        public async Task DeleteJobAdvertisementAsync_ShouldReturnTrue_WhenDeleted()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobId = Guid.NewGuid();
            var job = new JobAdvertisement { Id = jobId, JobTitle = "Test Job" };

            // Add the job to the in-memory database
            _dbContext.JobAdvertisements.Add(job);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.DeleteJobAdvertisementAsync(jobId);

            // Assert
            Assert.True(result); // Should return true since the job was deleted
            Assert.Null(await _dbContext.JobAdvertisements.FindAsync(jobId)); // Job should no longer exist
        }

        [Fact]
        public async Task GetJobApplicationsByIndividualIdAsync_ShouldReturnApplications()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var individualId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                UserId = individualId,
                JobAdvertisement = new JobAdvertisement { Id = jobId, JobTitle = "Test Job" }
            };

            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.GetJobApplicationsByIndividualIdAsync(individualId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Job", result.First().JobTitle);
        }

        [Fact]
        public async Task GetJobApplicationsByFounderIdAsync_ShouldReturnApplications_WhenFiltered()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var founderId = Guid.NewGuid();
            var jobAdvertisementId = Guid.NewGuid();

            var jobAdvertisement = new JobAdvertisement
            {
                Id = jobAdvertisementId,
                UserId = founderId,
                JobTitle = "Test Job"
            };

            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobAdvertisementId = jobAdvertisementId,
                JobAdvertisement = jobAdvertisement
            };

            _dbContext.JobAdvertisements.Add(jobAdvertisement);
            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.GetJobApplicationsByFounderIdAsync(founderId, jobAdvertisementId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Job", result.First().JobTitle);
        }

        [Fact]
        public async Task GetJobApplicationsByFounderIdAsync_ShouldReturnApplications_WhenNoJobAdvertisementIdIsProvided()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var founderId = Guid.NewGuid();
            var jobAdvertisement = new JobAdvertisement
            {
                Id = Guid.NewGuid(),
                UserId = founderId,
                JobTitle = "Test Job"
            };

            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobAdvertisementId = jobAdvertisement.Id,
                JobAdvertisement = jobAdvertisement
            };

            _dbContext.JobAdvertisements.Add(jobAdvertisement);
            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.GetJobApplicationsByFounderIdAsync(founderId, null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Job", result.First().JobTitle);
        }

        [Fact]
        public async Task GetSuccessfulJobApplicantsByJobAdvertisementIdAsync_ShouldReturnApplicants()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobAdvertisementId = Guid.NewGuid();
            var individualId = Guid.NewGuid();

            // Create a job application with status 'OfferAcceptedByIndividual'
            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobAdvertisementId = jobAdvertisementId,
                Status = JobApplicationStatus.OfferAcceptedByIndividual,
                UserId = individualId
            };

            // Create the corresponding user (applicant)
            var individual = new User
            {
                Id = individualId,
                FirstName = "Ali",
                LastName = "Momenzadeh Kholenjani",
                Email = "Ali.Momenzadeh-Kholenjani@city.ac.uk"
            };

            // Add the job application to the in-memory database
            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Mock the _userService to return the individual when looked up
            _userServiceMock.Setup(service => service.FindByIdAsync(individualId))
                .ReturnsAsync(individual);

            // Act
            var result = await _jobService.GetSuccessfulJobApplicantsByJobAdvertisementIdAsync(jobAdvertisementId);

            // Assert
            Assert.Single(result);
            Assert.Equal(individual.Email, result.First().IndividualEmail);
            Assert.Equal("Ali Momenzadeh Kholenjani", result.First().IndividualFullName);
        }

        [Fact]
        public async Task GetJobApplicationFormByIdAsync_ShouldReturnFormDto_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobApplicationId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var individualId = Guid.NewGuid();
            var founderId = Guid.NewGuid();

            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                JobAdvertisementId = jobId,
                JobAdvertisement = new JobAdvertisement { Id = jobId, JobTitle = "Software Engineer", UserId = founderId },
                UserId = individualId
            };

            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Mock the user service to return individual and founder details
            var individual = new User { Id = individualId, UserName = "individualUser", FirstName = "Ali", LastName = "Momenzadeh Kholenjani" };
            var founder = new User { Id = founderId, UserName = "founderUser", FirstName = "Founder", LastName = "Person" };

            _userServiceMock.Setup(service => service.FindByIdAsync(individualId)).ReturnsAsync(individual);
            _userServiceMock.Setup(service => service.FindByIdAsync(founderId)).ReturnsAsync(founder);

            // Act
            var result = await _jobService.GetJobApplicationFormByIdAsync(jobApplicationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobApplicationId, result.Id);
            Assert.Equal("individualUser", result.IndividualUserName);
            Assert.Equal("founderUser", result.FounderUserName);
        }

        [Fact]
        public async Task GetJobApplicationFormByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobApplicationId = Guid.NewGuid(); // This ID won't exist in the DB

            // Act
            var result = await _jobService.GetJobApplicationFormByIdAsync(jobApplicationId);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task SubmitJobApplicationAsync_ShouldReturnTrue_WhenSuccess()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var applicationFormDto = new JobApplicationFormDto
            {
                JobAdvertisementId = Guid.NewGuid(),
                CVFile = new Mock<IFormFile>().Object,
                CoverLetterFile = new Mock<IFormFile>().Object
            };

            _blobStorageServiceMock.Setup(service => service.UploadBlobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync("fake-url");

            // Act
            var result = await _jobService.SubmitJobApplicationAsync(applicationFormDto, Guid.NewGuid());

            // Assert
            Assert.True(result);

            // Verify that the job application was added to the in-memory database
            var jobApplication = await _dbContext.JobApplications.FirstOrDefaultAsync(ja => ja.CVUrl == "fake-url");
            Assert.NotNull(jobApplication);
            Assert.Equal("fake-url", jobApplication.CVUrl); // Blob URL should match
        }

        [Fact]
        public async Task UpdateJobApplicationAsync_ShouldReturnFalse_WhenJobNotFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var updateFormDto = new JobApplicationUpdateFormDto
            {
                Id = Guid.NewGuid(),
                Status = JobApplicationStatus.Submitted
            };

            // No job application added to the database, simulating a not-found scenario

            // Act
            var result = await _jobService.UpdateJobApplicationAsync(updateFormDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateJobApplicationAsync_ShouldReturnTrue_WhenUpdated()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var updateFormDto = new JobApplicationUpdateFormDto
            {
                Id = Guid.NewGuid(),
                Status = JobApplicationStatus.InterviewScheduled
            };

            var jobApplication = new JobApplication
            {
                Id = updateFormDto.Id,
                Status = JobApplicationStatus.Submitted
            };

            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.UpdateJobApplicationAsync(updateFormDto);

            // Assert
            Assert.True(result);

            // Verify that the job application status was updated
            var updatedJobApplication = await _dbContext.JobApplications.FindAsync(updateFormDto.Id);
            Assert.Equal(JobApplicationStatus.InterviewScheduled, updatedJobApplication.Status);
        }

        [Fact]
        public async Task UpdateApplicationStatusByIndividualAsync_ShouldReturnFalse_WhenInvalidStatus()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobApplicationId = Guid.NewGuid();
            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                Status = JobApplicationStatus.ApplicationWithdrawnByIndividual
            };

            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.UpdateApplicationStatusByIndividualAsync(jobApplicationId, JobApplicationStatus.Submitted);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateApplicationStatusByIndividualAsync_ShouldReturnFalse_WhenJobApplicationNotFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobApplicationId = Guid.NewGuid(); // Non-existing application

            // Act
            var result = await _jobService.UpdateApplicationStatusByIndividualAsync(jobApplicationId, JobApplicationStatus.ApplicationWithdrawnByIndividual);

            // Assert
            Assert.False(result); // Job application is not found, so should return false
        }

        [Fact]
        public async Task UpdateApplicationStatusByIndividualAsync_ShouldReturnTrue_WhenStatusUpdatedToWithdrawn()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobApplicationId = Guid.NewGuid();
            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                Status = JobApplicationStatus.Submitted // Status can be updated
            };

            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.UpdateApplicationStatusByIndividualAsync(jobApplicationId, JobApplicationStatus.ApplicationWithdrawnByIndividual);

            // Assert
            Assert.True(result); // The status should be updated successfully
            var updatedJobApplication = await _dbContext.JobApplications.FindAsync(jobApplicationId);
            Assert.Equal(JobApplicationStatus.ApplicationWithdrawnByIndividual, updatedJobApplication.Status); // Verify that the status was updated
        }

        [Fact]
        public async Task UpdateApplicationStatusByIndividualAsync_ShouldReturnFalse_WhenInvalidStatusTransition()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobApplicationId = Guid.NewGuid();
            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                Status = JobApplicationStatus.ApplicationWithdrawnByIndividual // Status cannot be updated from this status
            };

            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.UpdateApplicationStatusByIndividualAsync(jobApplicationId, JobApplicationStatus.Submitted);

            // Assert
            Assert.False(result); // Status cannot be updated from 'ApplicationWithdrawnByIndividual'
            var updatedJobApplication = await _dbContext.JobApplications.FindAsync(jobApplicationId);
            Assert.Equal(JobApplicationStatus.ApplicationWithdrawnByIndividual, updatedJobApplication.Status); // Verify that the status was not updated
        }

        [Fact]
        public async Task UpdateApplicationStatusByIndividualAsync_ShouldReturnTrue_WhenOfferAccepted()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobApplicationId = Guid.NewGuid();
            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                Status = JobApplicationStatus.OfferExtended // Status can be transitioned to OfferAcceptedByIndividual
            };

            _dbContext.JobApplications.Add(jobApplication);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobService.UpdateApplicationStatusByIndividualAsync(jobApplicationId, JobApplicationStatus.OfferAcceptedByIndividual);

            // Assert
            Assert.True(result); // The status should be updated successfully
            var updatedJobApplication = await _dbContext.JobApplications.FindAsync(jobApplicationId);
            Assert.Equal(JobApplicationStatus.OfferAcceptedByIndividual, updatedJobApplication.Status); // Verify that the status was updated
        }

        [Fact]
        public async Task HasUserAlreadyAppliedAsync_ShouldReturnTrue_WhenUserHasApplied()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var jobApplication = _dbContext.JobApplications.First();
            var jobId = jobApplication.JobAdvertisementId;
            var userId = jobApplication.UserId;

            // Act
            var result = await _jobService.HasUserAlreadyAppliedAsync(jobId, userId);

            // Assert
            Assert.True(result);
        }
    }
}
