using StartupTeam.Module.JobManagement.Models.Enums;

namespace StartupTeam.Module.JobManagement.Dtos
{
    public class JobAdvertisementDetailDto
    {
        public Guid Id { get; set; }

        // Startup Information
        public string StartupName { get; set; } = string.Empty;

        public string StartupDescription { get; set; } = string.Empty;

        public StartupStage StartupStage { get; set; }

        public string Industry { get; set; } = string.Empty;

        public string? KeyTechnologies { get; set; }

        public string? UniqueSellingPoints { get; set; }

        public string? MissionStatement { get; set; }

        public int? FoundingYear { get; set; }

        public int? TeamSize { get; set; }

        public string? StartupWebsite { get; set; }

        public string? StartupValues { get; set; }

        // Job Details
        public string JobTitle { get; set; } = string.Empty;

        public string JobDescription { get; set; } = string.Empty;

        public EmploymentType EmploymentType { get; set; }

        public string? RequiredSkills { get; set; }

        public string? JobResponsibilities { get; set; }

        public string? SalaryRange { get; set; }

        public JobLocationType JobLocationType { get; set; }

        public string? JobLocation { get; set; } = string.Empty;

        public DateTime ApplicationDeadline { get; set; }

        public string? Experience { get; set; }

        public string? Education { get; set; }

        public bool RequireCV { get; set; }

        public bool RequireCoverLetter { get; set; }

        public bool HasApplied { get; set; }
    }
}
