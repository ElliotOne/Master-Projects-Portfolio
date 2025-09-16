using StartupTeam.Module.JobManagement.Models.Enums;
using StartupTeam.Module.JobManagement.Validation;
using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.JobManagement.Dtos
{
    public class JobApplicationUpdateFormDto
    {
        public Guid Id { get; set; }

        public Guid JobAdvertisementId { get; set; }

        public string? CVUrl { get; set; }

        public string? CoverLetterUrl { get; set; }

        public DateTime ApplicationDate { get; set; }

        [Required]
        public JobApplicationStatus Status { get; set; }

        public string StatusText { get; set; } = string.Empty;

        [JobApplicationInterviewDateValidation]
        public DateTime? InterviewDate { get; set; }

        public string IndividualUserName { get; set; } = string.Empty;

        public string IndividualFullName { get; set; } = string.Empty;

        public string FounderUserName { get; set; } = string.Empty;

        public string FounderFullName { get; set; } = string.Empty;
    }
}
