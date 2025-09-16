using StartupTeam.Module.JobManagement.Models.Enums;

namespace StartupTeam.Module.JobManagement.Models
{
    public class JobApplication
    {
        public Guid Id { get; set; }

        public Guid JobAdvertisementId { get; set; }

        public string? CVUrl { get; set; }

        // Store the extracted text of the CV
        public string? CVTextContent { get; set; }

        public string? CoverLetterUrl { get; set; }

        public DateTime ApplicationDate { get; set; }

        public JobApplicationStatus Status { get; set; }

        public DateTime? InterviewDate { get; set; }

        // Skilled Individual Id
        public Guid UserId { get; set; }

        public JobAdvertisement? JobAdvertisement { get; set; }
    }
}
