namespace StartupTeam.Shared.Services
{
    public interface IMailService
    {
        Task<bool> SendEmail(string emailAddress, string subject, string body);
    }
}
