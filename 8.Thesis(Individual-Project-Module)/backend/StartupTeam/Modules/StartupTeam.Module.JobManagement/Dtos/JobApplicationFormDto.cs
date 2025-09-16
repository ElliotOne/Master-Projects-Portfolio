using StartupTeam.Shared.Validation;

namespace StartupTeam.Module.JobManagement.Dtos
{
    public class JobApplicationFormDto
    {
        public Guid JobAdvertisementId { get; set; }

        [FileExtension(new[] { ".pdf", ".docx" })]
        public IFormFile? CVFile { get; set; }

        [FileExtension(new[] { ".pdf", ".docx" })]
        public IFormFile? CoverLetterFile { get; set; }
    }
}
