namespace ConsoleApp1.Models
{
    public class Portfolio
    {
        public Guid Id { get; set; }
        public ICollection<PortfolioItem> PortfolioItems { get; set; }
            = new List<PortfolioItem>();
    }
}
