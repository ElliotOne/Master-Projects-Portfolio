using System.ComponentModel.DataAnnotations;

namespace StartupTeam.Module.PortfolioManagement.Dtos
{
    public class PortfolioItemFormDto
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Type { get; set; }

        [MaxLength(256)]
        public string? Technologies { get; set; }

        [MaxLength(256)]
        public string? Skills { get; set; }

        [MaxLength(256)]
        public string? Industry { get; set; }

        [MaxLength(256)]
        public string? Role { get; set; }

        [MaxLength(256)]
        public string? Duration { get; set; }

        [MaxLength(256)]
        public string? Link { get; set; }

        [MaxLength(256)]
        public string? AttachmentUrl { get; set; }

        public IFormFile? AttachmentFile { get; set; }

        [MaxLength(256)]
        public string? Tags { get; set; }

        public Guid UserId { get; set; }
    }
}
