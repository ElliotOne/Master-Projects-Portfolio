namespace StartupTeam.Module.JobManagement.Dtos
{
    public class JobApplicationDto
    {
        public Guid Id { get; set; }
        public string StartupName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string JobLocation { get; set; } = string.Empty;
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string IndividualFullName { get; set; } = string.Empty;
        public double Score { get; set; }
    }
}
