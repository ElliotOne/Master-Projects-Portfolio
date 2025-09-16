using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.JobManagement.Data;
using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Extensions;
using StartupTeam.Module.JobManagement.Models;
using StartupTeam.Module.JobManagement.Models.Enums;
using StartupTeam.Module.UserManagement.Services;
using StartupTeam.Shared.Helpers;
using StartupTeam.Shared.Models.Constants;
using StartupTeam.Shared.Services;
using System.Text;
using UglyToad.PdfPig;

namespace StartupTeam.Module.JobManagement.Services
{
    public class JobService : IJobService
    {
        private readonly JobManagementDbContext _context;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IUserService _userService;

        public JobService(
            JobManagementDbContext context,
            IBlobStorageService blobStorageService,
            IUserService userService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
            _userService = userService;
        }

        public async Task<IEnumerable<JobAdvertisementDto>> GetJobAdvertisementsAsync()
        {
            var jobAdvertisements =
                await _context.JobAdvertisements
                    .Where(ja => ja.ApplicationDeadline > DateTime.Now)
                    .ToListAsync();

            var jobAdvertisementDtos = jobAdvertisements.Select(ja => new JobAdvertisementDto
            {
                Id = ja.Id,
                StartupName = ja.StartupName,
                JobTitle = ja.JobTitle,
                JobLocation = ja.JobLocation ?? nameof(JobLocationType.Remote),
                ApplicationDeadline = ja.ApplicationDeadline
            }).ToList();

            return jobAdvertisementDtos;
        }

        public async Task<IEnumerable<JobAdvertisementDto>> GetJobAdvertisementsForUserAsync(Guid userId)
        {
            var jobAdvertisements =
                await _context.JobAdvertisements
                    .Where(ja =>
                        ja.ApplicationDeadline > DateTime.Now &&
                        ja.UserId == userId)
                    .ToListAsync();

            var jobAdvertisementDtos = jobAdvertisements.Select(ja => new JobAdvertisementDto
            {
                Id = ja.Id,
                StartupName = ja.StartupName,
                JobTitle = ja.JobTitle,
                JobLocation = ja.JobLocation ?? nameof(JobLocationType.Remote),
                ApplicationDeadline = ja.ApplicationDeadline
            }).ToList();

            return jobAdvertisementDtos;
        }

        public async Task<IEnumerable<JobAdvertisementDto>> GetJobAdvertisementsByUserIdAsync(Guid userId)
        {
            var jobAdvertisements = await _context.JobAdvertisements
                .Where(ja => ja.UserId == userId)
                .ToListAsync();

            var jobAdvertisementDtos = jobAdvertisements.Select(ja => new JobAdvertisementDto
            {
                Id = ja.Id,
                StartupName = ja.StartupName,
                JobTitle = ja.JobTitle,
                JobLocation = ja.JobLocation ?? nameof(JobLocationType.Remote),
                ApplicationDeadline = ja.ApplicationDeadline,
                Status = ja.ApplicationDeadline > DateTime.Now ? "Active" : "Expired"
            }).ToList();

            return jobAdvertisementDtos;
        }

        public async Task<JobAdvertisementDetailDto?> GetJobAdvertisementByIdAsync(Guid id, Guid individualId)
        {
            var jobAdvertisement = await _context.JobAdvertisements.FindAsync(id);

            if (jobAdvertisement == null)
            {
                return null;
            }

            var advertisementDetailDto = new JobAdvertisementDetailDto
            {
                Id = jobAdvertisement.Id,
                StartupName = jobAdvertisement.StartupName,
                StartupDescription = jobAdvertisement.StartupDescription,
                StartupStage = jobAdvertisement.StartupStage,
                Industry = jobAdvertisement.Industry,
                KeyTechnologies = jobAdvertisement.KeyTechnologies,
                UniqueSellingPoints = jobAdvertisement.UniqueSellingPoints,
                MissionStatement = jobAdvertisement.MissionStatement,
                FoundingYear = jobAdvertisement.FoundingYear,
                TeamSize = jobAdvertisement.TeamSize,
                StartupWebsite = jobAdvertisement.StartupWebsite,
                StartupValues = jobAdvertisement.StartupValues,
                JobTitle = jobAdvertisement.JobTitle,
                JobDescription = jobAdvertisement.JobDescription,
                EmploymentType = jobAdvertisement.EmploymentType,
                RequiredSkills = jobAdvertisement.RequiredSkills,
                JobResponsibilities = jobAdvertisement.JobResponsibilities,
                SalaryRange = jobAdvertisement.SalaryRange,
                JobLocationType = jobAdvertisement.JobLocationType,
                JobLocation = jobAdvertisement.JobLocation,
                ApplicationDeadline = jobAdvertisement.ApplicationDeadline,
                Experience = jobAdvertisement.Experience,
                Education = jobAdvertisement.Education,
                RequireCV = jobAdvertisement.RequireCV,
                RequireCoverLetter = jobAdvertisement.RequireCoverLetter,
                HasApplied = await HasUserAlreadyAppliedAsync(id, individualId)
            };

            return advertisementDetailDto;
        }

        public async Task<JobAdvertisementFormDto?> GetJobAdvertisementFormByIdAsync(Guid id)
        {
            var jobAdvertisement = await _context.JobAdvertisements.FindAsync(id);

            if (jobAdvertisement == null)
            {
                return null;
            }

            var advertisementFormDto = new JobAdvertisementFormDto
            {
                Id = jobAdvertisement.Id,
                StartupName = jobAdvertisement.StartupName,
                StartupDescription = jobAdvertisement.StartupDescription,
                StartupStage = jobAdvertisement.StartupStage,
                Industry = jobAdvertisement.Industry,
                KeyTechnologies = jobAdvertisement.KeyTechnologies,
                UniqueSellingPoints = jobAdvertisement.UniqueSellingPoints,
                MissionStatement = jobAdvertisement.MissionStatement,
                FoundingYear = jobAdvertisement.FoundingYear,
                TeamSize = jobAdvertisement.TeamSize,
                StartupWebsite = jobAdvertisement.StartupWebsite,
                StartupValues = jobAdvertisement.StartupValues,
                JobTitle = jobAdvertisement.JobTitle,
                JobDescription = jobAdvertisement.JobDescription,
                EmploymentType = jobAdvertisement.EmploymentType,
                RequiredSkills = jobAdvertisement.RequiredSkills,
                JobResponsibilities = jobAdvertisement.JobResponsibilities,
                SalaryRange = jobAdvertisement.SalaryRange,
                JobLocationType = jobAdvertisement.JobLocationType,
                JobLocation = jobAdvertisement.JobLocation,
                ApplicationDeadline = jobAdvertisement.ApplicationDeadline,
                Experience = jobAdvertisement.Experience,
                Education = jobAdvertisement.Education,
                RequireCV = jobAdvertisement.RequireCV,
                RequireCoverLetter = jobAdvertisement.RequireCoverLetter,
                UserId = jobAdvertisement.UserId
            };

            return advertisementFormDto;
        }

        public async Task<bool> CreateJobAdvertisementAsync(JobAdvertisementFormDto advertisementFormDto, Guid userId)
        {
            var jobAdvertisement = new JobAdvertisement()
            {
                StartupName = advertisementFormDto.StartupName,
                StartupDescription = advertisementFormDto.StartupDescription,
                StartupStage = advertisementFormDto.StartupStage,
                Industry = advertisementFormDto.Industry,
                KeyTechnologies = advertisementFormDto.KeyTechnologies,
                UniqueSellingPoints = advertisementFormDto.UniqueSellingPoints,
                MissionStatement = advertisementFormDto.MissionStatement,
                FoundingYear = advertisementFormDto.FoundingYear,
                TeamSize = advertisementFormDto.TeamSize,
                StartupWebsite = advertisementFormDto.StartupWebsite,
                StartupValues = advertisementFormDto.StartupValues,
                JobTitle = advertisementFormDto.JobTitle,
                JobDescription = advertisementFormDto.JobDescription,
                EmploymentType = advertisementFormDto.EmploymentType,
                RequiredSkills = advertisementFormDto.RequiredSkills,
                JobResponsibilities = advertisementFormDto.JobResponsibilities,
                SalaryRange = advertisementFormDto.SalaryRange,
                JobLocationType = advertisementFormDto.JobLocationType,
                JobLocation = advertisementFormDto.JobLocation,
                ApplicationDeadline = advertisementFormDto.ApplicationDeadline,
                Experience = advertisementFormDto.Experience,
                Education = advertisementFormDto.Education,
                RequireCV = advertisementFormDto.RequireCV,
                RequireCoverLetter = advertisementFormDto.RequireCoverLetter,
                UserId = userId
            };

            _context.JobAdvertisements.Add(jobAdvertisement);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateJobAdvertisementAsync(JobAdvertisementFormDto advertisementFormDto)
        {
            var jobAdvertisement = await _context.JobAdvertisements
                .FirstOrDefaultAsync(ja => ja.Id == advertisementFormDto.Id);

            if (jobAdvertisement == null)
            {
                return false;
            }

            jobAdvertisement.StartupName = advertisementFormDto.StartupName;
            jobAdvertisement.StartupDescription = advertisementFormDto.StartupDescription;
            jobAdvertisement.StartupStage = advertisementFormDto.StartupStage;
            jobAdvertisement.Industry = advertisementFormDto.Industry;
            jobAdvertisement.KeyTechnologies = advertisementFormDto.KeyTechnologies;
            jobAdvertisement.UniqueSellingPoints = advertisementFormDto.UniqueSellingPoints;
            jobAdvertisement.MissionStatement = advertisementFormDto.MissionStatement;
            jobAdvertisement.FoundingYear = advertisementFormDto.FoundingYear;
            jobAdvertisement.TeamSize = advertisementFormDto.TeamSize;
            jobAdvertisement.StartupWebsite = advertisementFormDto.StartupWebsite;
            jobAdvertisement.StartupValues = advertisementFormDto.StartupValues;
            jobAdvertisement.JobTitle = advertisementFormDto.JobTitle;
            jobAdvertisement.JobDescription = advertisementFormDto.JobDescription;
            jobAdvertisement.EmploymentType = advertisementFormDto.EmploymentType;
            jobAdvertisement.RequiredSkills = advertisementFormDto.RequiredSkills;
            jobAdvertisement.JobResponsibilities = advertisementFormDto.JobResponsibilities;
            jobAdvertisement.SalaryRange = advertisementFormDto.SalaryRange;
            jobAdvertisement.JobLocationType = advertisementFormDto.JobLocationType;
            jobAdvertisement.JobLocation = advertisementFormDto.JobLocation;
            jobAdvertisement.ApplicationDeadline = advertisementFormDto.ApplicationDeadline;
            jobAdvertisement.Experience = advertisementFormDto.Experience;
            jobAdvertisement.Education = advertisementFormDto.Education;
            jobAdvertisement.RequireCV = advertisementFormDto.RequireCV;
            jobAdvertisement.RequireCoverLetter = advertisementFormDto.RequireCoverLetter;

            _context.Entry(jobAdvertisement).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteJobAdvertisementAsync(Guid id)
        {
            var jobAdvertisement = await _context.JobAdvertisements.FindAsync(id);

            if (jobAdvertisement == null)
            {
                return false;
            }

            _context.JobAdvertisements.Remove(jobAdvertisement);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<JobApplicationDto>> GetJobApplicationsByIndividualIdAsync(Guid individualId)
        {
            var jobApplications = _context.JobApplications
                .Where(ja => ja.UserId == individualId)
                .Include(ja => ja.JobAdvertisement)
                .AsQueryable();

            var jobApplicationDtos =
                await jobApplications.Select(
                    ja => new JobApplicationDto
                    {
                        Id = ja.Id,
                        StartupName = ja.JobAdvertisement!.StartupName,
                        JobTitle = ja.JobAdvertisement.JobTitle,
                        JobLocation = ja.JobAdvertisement.JobLocation ?? nameof(JobLocationType.Remote),
                        ApplicationDate = ja.ApplicationDate,
                        Status = ja.Status.ToFriendlyString()
                    }).ToListAsync();

            return jobApplicationDtos;
        }

        public async Task<IEnumerable<JobApplicationDto>> GetJobApplicationsByFounderIdAsync(Guid founderId, Guid? jobAdvertisementId)
        {
            var jobApplicationsQuery = _context.JobApplications
                .Include(ja => ja.JobAdvertisement)
                .Where(ja => ja.JobAdvertisement!.UserId == founderId)
                .AsQueryable();

            // If jobAdvertisementId is provided, filter the query further
            if (jobAdvertisementId.HasValue)
            {
                jobApplicationsQuery = jobApplicationsQuery.Where(ja => ja.JobAdvertisementId == jobAdvertisementId.Value);
            }

            var jobApplications = await jobApplicationsQuery.ToListAsync();

            var jobApplicationDtos = new List<JobApplicationDto>();

            foreach (var ja in jobApplications)
            {
                // Fetch the individual for each job application
                var individual = await _userService.FindByIdAsync(ja.UserId);

                var jobApplicationDto = new JobApplicationDto
                {
                    Id = ja.Id,
                    StartupName = ja.JobAdvertisement!.StartupName,
                    JobTitle = ja.JobAdvertisement.JobTitle,
                    JobLocation = ja.JobAdvertisement.JobLocation ?? nameof(JobLocationType.Remote),
                    ApplicationDate = ja.ApplicationDate,
                    Status = ja.Status.ToFriendlyString(),
                    IndividualFullName = individual == null ?
                        string.Empty : $"{individual.FirstName} {individual.LastName}"
                };

                jobApplicationDtos.Add(jobApplicationDto);
            }

            return jobApplicationDtos;
        }

        public async Task<IEnumerable<JobApplicantDto>>
            GetSuccessfulJobApplicantsByJobAdvertisementIdAsync(Guid jobAdvertisementId)
        {
            var jobApplications = await _context.JobApplications
                .Where(ja =>
                    ja.JobAdvertisementId == jobAdvertisementId &&
                    ja.Status == JobApplicationStatus.OfferAcceptedByIndividual)
                .ToListAsync();

            var jobApplicantDtos = new List<JobApplicantDto>();

            foreach (var ja in jobApplications)
            {
                // Fetch the individual for each job application
                var individual = await _userService.FindByIdAsync(ja.UserId);

                if (individual != null)
                {
                    var jobApplicantDto = new JobApplicantDto()
                    {
                        UserId = individual.Id,
                        IndividualEmail = individual.Email ?? string.Empty,
                        IndividualFullName = $"{individual.FirstName} {individual.LastName}"
                    };

                    jobApplicantDtos.Add(jobApplicantDto);
                }
            }

            return jobApplicantDtos;
        }

        public async Task<JobApplicationDetailDto?> GetJobApplicationByIdAsync(Guid id)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);

            if (jobApplication == null)
            {
                return null;
            }


            var jobApplicationDetailDto = new JobApplicationDetailDto()
            {
                Id = jobApplication.Id,
                UserId = jobApplication.UserId
            };

            return jobApplicationDetailDto;
        }

        public async Task<JobApplicationUpdateFormDto?> GetJobApplicationFormByIdAsync(Guid id)
        {
            var jobApplication = await _context.JobApplications
                .Include(ja => ja.JobAdvertisement)
                .FirstOrDefaultAsync(ja => ja.Id == id);

            if (jobApplication == null ||
                jobApplication.JobAdvertisement == null)
            {
                return null;
            }

            var individual = await _userService.FindByIdAsync(jobApplication.UserId);
            var founder = await _userService.FindByIdAsync(jobApplication.JobAdvertisement.UserId);

            var jobApplicationUpdateFormDto = new JobApplicationUpdateFormDto
            {
                Id = jobApplication.Id,
                JobAdvertisementId = jobApplication.JobAdvertisementId,
                CVUrl = jobApplication.CVUrl,
                CoverLetterUrl = jobApplication.CoverLetterUrl,
                ApplicationDate = jobApplication.ApplicationDate,
                Status = jobApplication.Status,
                StatusText = jobApplication.Status.ToFriendlyString(),
                InterviewDate = jobApplication.InterviewDate,
                IndividualUserName = individual?.UserName ?? string.Empty,
                IndividualFullName = individual == null ?
                    string.Empty : $"{individual.FirstName} {individual.LastName}",
                FounderUserName = founder?.UserName ?? string.Empty,
                FounderFullName = founder == null ?
                    string.Empty : $"{founder.FirstName} {founder.LastName}",
            };

            return jobApplicationUpdateFormDto;
        }

        public async Task<bool> SubmitJobApplicationAsync(JobApplicationFormDto applicationFormDto, Guid individualId)
        {
            string? content = null;
            string? cVUrl = null;
            string? coverLetterUrl = null;

            if (applicationFormDto.CVFile != null && applicationFormDto.CVFile.Length > 0)
            {
                content = await ReadFileContentAsync(applicationFormDto.CVFile);

                if (content == null)
                {
                    return false;
                }
            }

            if (applicationFormDto.CVFile != null)
            {
                await using var stream = applicationFormDto.CVFile.OpenReadStream();
                cVUrl = await _blobStorageService.UploadBlobAsync(
                    BlobStorageConstants.JobApplicationAttachmentsContainerName,
                    FormFileHelper.GenerateNewFileName(applicationFormDto.CVFile),
                    stream);
            }

            if (applicationFormDto.CoverLetterFile != null)
            {
                await using var stream = applicationFormDto.CoverLetterFile.OpenReadStream();
                coverLetterUrl = await _blobStorageService.UploadBlobAsync(
                    BlobStorageConstants.JobApplicationAttachmentsContainerName,
                    FormFileHelper.GenerateNewFileName(applicationFormDto.CoverLetterFile),
                    stream);
            }

            var jobApplication = new JobApplication
            {
                JobAdvertisementId = applicationFormDto.JobAdvertisementId,
                CVUrl = cVUrl,
                CVTextContent = content,
                CoverLetterUrl = coverLetterUrl,
                ApplicationDate = DateTime.UtcNow,
                UserId = individualId,
                Status = JobApplicationStatus.Submitted
            };

            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateJobApplicationAsync(JobApplicationUpdateFormDto jobApplicationUpdateFormDto)
        {
            var jobApplication = await _context.JobApplications
                .FirstOrDefaultAsync(ja => ja.Id == jobApplicationUpdateFormDto.Id);

            if (jobApplication == null)
            {
                return false;
            }

            switch (jobApplication.Status)
            {
                case JobApplicationStatus.OfferAcceptedByIndividual:
                case JobApplicationStatus.OfferRejectedByIndividual:
                case JobApplicationStatus.ApplicationWithdrawnByIndividual:
                case JobApplicationStatus.ApplicationRejectedByFounder:
                    return false;
            }

            jobApplication.Status = jobApplicationUpdateFormDto.Status;
            jobApplication.InterviewDate = jobApplicationUpdateFormDto.InterviewDate;

            _context.Entry(jobApplication).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateApplicationStatusByIndividualAsync(Guid id, JobApplicationStatus status)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);

            if (jobApplication == null)
            {
                return false;
            }

            switch (jobApplication.Status)
            {
                case JobApplicationStatus.Submitted:
                case JobApplicationStatus.UnderReview:
                case JobApplicationStatus.Shortlisted:
                case JobApplicationStatus.InterviewScheduled:
                    {
                        if (status == JobApplicationStatus.ApplicationWithdrawnByIndividual)
                        {
                            jobApplication.Status = status;
                            _context.Entry(jobApplication).State = EntityState.Modified;
                            return await _context.SaveChangesAsync() > 0;
                        }

                        break;
                    }

                case JobApplicationStatus.OfferExtended when status == JobApplicationStatus.OfferAcceptedByIndividual ||
                                                             status == JobApplicationStatus.OfferRejectedByIndividual:
                    jobApplication.Status = status;
                    _context.Entry(jobApplication).State = EntityState.Modified;
                    return await _context.SaveChangesAsync() > 0;

                case JobApplicationStatus.Interviewed:
                case JobApplicationStatus.OfferAcceptedByIndividual:
                case JobApplicationStatus.OfferRejectedByIndividual:
                case JobApplicationStatus.ApplicationRejectedByFounder:
                case JobApplicationStatus.ApplicationWithdrawnByIndividual:
                    return false;
            }

            return false;
        }

        public async Task<bool> HasUserAlreadyAppliedAsync(Guid id, Guid individualId)
        {
            return await _context.JobApplications
                .AnyAsync(ja => ja.JobAdvertisementId == id && ja.UserId == individualId);
        }

        private async Task<string?> ReadFileContentAsync(IFormFile file)
        {
            string? content;

            if (file.ContentType == "application/pdf")
            {
                content = await ReadPdfFileAsync(file);
            }
            else if (file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                content = await ReadWordFileAsync(file);
            }
            else
            {
                // Handle unsupported file types
                throw new NotSupportedException("Unsupported file type.");
            }

            return content;
        }

        private async Task<string?> ReadPdfFileAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            var text = new StringBuilder();

            // Using PdfPig to extract text from the PDF document
            using (var document = PdfDocument.Open(stream))
            {
                foreach (var page in document.GetPages())
                {
                    text.Append(page.Text);
                }
            }

            return text.ToString();
        }

        private async Task<string?> ReadWordFileAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();

            // Using Open XML SDK to extract text from the Word document
            using var doc = WordprocessingDocument.Open(stream, false);
            return doc.MainDocumentPart?.Document.InnerText ?? string.Empty;
        }
    }
}
