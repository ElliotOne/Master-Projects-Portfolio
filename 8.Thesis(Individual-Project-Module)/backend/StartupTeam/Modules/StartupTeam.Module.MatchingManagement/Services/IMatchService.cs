using StartupTeam.Module.JobManagement.Dtos;

namespace StartupTeam.Module.MatchingManagement.Services
{
    public interface IMatchService
    {
        Task<IEnumerable<JobApplicationDto>> GetTopMatchedApplicantsForJob(Guid jobAdId, int topN);
        Task<IEnumerable<JobAdvertisementDto>> GetTopMatchedJobAdsForIndividual(Guid individualId, int topN);
    }
}
