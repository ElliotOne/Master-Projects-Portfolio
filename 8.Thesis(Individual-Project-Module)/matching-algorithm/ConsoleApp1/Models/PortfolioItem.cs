namespace ConsoleApp1.Models
{
    public class PortfolioItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Technologies { get; set; }
        public string? Skills { get; set; }
        public string? Industry { get; set; }
    }
}
