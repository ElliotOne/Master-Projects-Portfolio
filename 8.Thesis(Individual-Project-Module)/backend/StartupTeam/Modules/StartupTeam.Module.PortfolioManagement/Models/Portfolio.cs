namespace StartupTeam.Module.PortfolioManagement.Models
{
    public class Portfolio
    {
        public Guid Id { get; set; }

        // Skilled Individual Id
        public Guid UserId { get; set; }

        public ICollection<PortfolioItem> PortfolioItems { get; set; }
            = new List<PortfolioItem>();
    }
}
