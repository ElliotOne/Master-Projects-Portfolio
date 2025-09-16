namespace ConsoleApp1.Models
{
    public class JobAdvertisement
    {
        public Guid Id { get; set; }
        public string StartupDescription { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string? KeyTechnologies { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string? RequiredSkills { get; set; }
        public string? Experience { get; set; }
        public string? Education { get; set; }
    }
}
