namespace ConsoleApp1.Models
{
    public class JobApplication
    {
        public Guid Id { get; set; }
        public Guid JobAdvertisementId { get; set; }

        // Store the extracted text of the CV
        public string? CVTextContent { get; set; }
    }
}
