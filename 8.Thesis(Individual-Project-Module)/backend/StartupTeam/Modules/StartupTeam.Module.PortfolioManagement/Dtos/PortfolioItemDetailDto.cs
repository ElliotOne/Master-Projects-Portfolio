namespace StartupTeam.Module.PortfolioManagement.Dtos
{
    public class PortfolioItemDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? Technologies { get; set; }
        public string? Skills { get; set; }
        public string? Industry { get; set; }
        public string? Role { get; set; }
        public string? Duration { get; set; }
        public string? Link { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? Tags { get; set; }
    }
}
