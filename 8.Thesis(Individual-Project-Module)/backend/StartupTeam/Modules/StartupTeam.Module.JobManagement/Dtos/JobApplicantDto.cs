namespace StartupTeam.Module.JobManagement.Dtos
{
    public class JobApplicantDto
    {
        public Guid UserId { get; set; }
        public string IndividualEmail { get; set; } = string.Empty;
        public string IndividualFullName { get; set; } = string.Empty;
    }
}
