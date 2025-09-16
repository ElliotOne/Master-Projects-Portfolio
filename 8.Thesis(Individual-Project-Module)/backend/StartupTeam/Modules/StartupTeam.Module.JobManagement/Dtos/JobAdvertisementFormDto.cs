using StartupTeam.Module.JobManagement.Models.Enums;
using StartupTeam.Module.JobManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.JobManagement.Dtos
{
    public class JobAdvertisementFormDto
    {
        public Guid? Id { get; set; }

        // Startup Information
        [Required]
        [MaxLength(256)]
        public string StartupName { get; set; } = string.Empty;

        [Required]
        public string StartupDescription { get; set; } = string.Empty;

        [Required]
        public StartupStage StartupStage { get; set; }

        [Required]
        [MaxLength(256)]
        public string Industry { get; set; } = string.Empty;

        public string? KeyTechnologies { get; set; }

        public string? UniqueSellingPoints { get; set; }

        public string? MissionStatement { get; set; }

        [FoundingYearValidation]
        public int? FoundingYear { get; set; }

        [TeamSizeValidation]
        public int? TeamSize { get; set; }

        [MaxLength(256)]
        public string? StartupWebsite { get; set; }

        public string? StartupValues { get; set; }

        // Job Details
        [Required]
        [MaxLength(256)]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public string JobDescription { get; set; } = string.Empty;

        [Required]
        public EmploymentType EmploymentType { get; set; }

        public string? RequiredSkills { get; set; }

        public string? JobResponsibilities { get; set; }

        public string? SalaryRange { get; set; }

        [Required]
        public JobLocationType JobLocationType { get; set; }

        [JobLocationValidation]
        public string? JobLocation { get; set; }

        [Required]
        [JobApplicationDateValidation]
        public DateTime ApplicationDeadline { get; set; }

        public string? Experience { get; set; }

        public string? Education { get; set; }

        public bool RequireCV { get; set; }

        public bool RequireCoverLetter { get; set; }

        public Guid UserId { get; set; }
    }
}
