using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.JobManagement.Data;
using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Extensions;
using StartupTeam.Module.JobManagement.Models.Enums;
using StartupTeam.Module.MatchingManagement.Utilities;
using StartupTeam.Module.PortfolioManagement.Data;
using StartupTeam.Module.PortfolioManagement.Models;
using StartupTeam.Module.UserManagement.Services;

namespace StartupTeam.Module.MatchingManagement.Services
{
    public class MatchService : IMatchService
    {
        private readonly JobManagementDbContext _jobManagementDbContext;
        private readonly PortfolioManagementDbContext _portfolioManagementDbContext;
        private readonly IUserService _userService;
        private readonly TextSimilarityCalculator _similarityCalculator;

        public MatchService(
            JobManagementDbContext jobManagementDbContext,
            PortfolioManagementDbContext portfolioManagementDbContext,
            IUserService userService,
            TextSimilarityCalculator similarityCalculator)
        {
            _jobManagementDbContext = jobManagementDbContext;
            _portfolioManagementDbContext = portfolioManagementDbContext;
            _userService = userService;
            _similarityCalculator = similarityCalculator;
        }

        public async Task<IEnumerable<JobApplicationDto>> GetTopMatchedApplicantsForJob(Guid jobAdId, int topN)
        {
            var jobAd =
                await _jobManagementDbContext.JobAdvertisements.FindAsync(jobAdId);

            if (jobAd == null)
            {
                return new List<JobApplicationDto>();
            }

            var jobApplications =
                await _jobManagementDbContext.JobApplications
                    .Include(ja => ja.JobAdvertisement)
                    .Where(ja => ja.JobAdvertisement!.UserId == jobAd.UserId)
                    .Where(ja => ja.JobAdvertisementId == jobAd.Id)
                    .ToListAsync();

            var portfolios = new Dictionary<Guid, Portfolio?>();
            var threshold = 0.3;
            var predictions = new List<(JobApplicationDto applicant, double score)>();

            // Calculate similarity for each applicant and fetch portfolio data
            foreach (var jobApplication in jobApplications)
            {
                portfolios[jobApplication.UserId] =
                    await _portfolioManagementDbContext.Portfolios
                        .Include(p => p.PortfolioItems)
                        .FirstOrDefaultAsync(p => p.UserId == jobApplication.UserId);

                var portfolio = portfolios[jobApplication.UserId];

                string portfolioText = string.Empty;

                if (portfolio != null)
                {
                    portfolioText = string.Join(" ",
                        portfolio.PortfolioItems.Select(item => $"{item.Title} {item.Description} {item.Skills} {item.Technologies} {item.Industry}"));
                }

                var jobAppText = $"{jobApplication.CVTextContent} {portfolioText}";

                var jobAdText = $"{jobAd.JobTitle} {jobAd.JobDescription} {jobAd.RequiredSkills} {jobAd.KeyTechnologies} {jobAd.Industry} {jobAd.Experience} {jobAd.Education} {jobAd.StartupDescription}";
                var score = _similarityCalculator.CalculateAverageMatchingScore(jobAppText, jobAdText);

                if (score > threshold)
                {
                    // Fetch the individual for each job application
                    var individual = await _userService.FindByIdAsync(jobApplication.UserId);

                    predictions.Add((new JobApplicationDto
                    {
                        Id = jobApplication.Id,
                        StartupName = jobApplication.JobAdvertisement!.StartupName,
                        JobTitle = jobApplication.JobAdvertisement.JobTitle,
                        JobLocation = jobApplication.JobAdvertisement.JobLocation ?? nameof(JobLocationType.Remote),
                        ApplicationDate = jobApplication.ApplicationDate,
                        Status = jobApplication.Status.ToFriendlyString(),
                        IndividualFullName = individual == null ?
                            string.Empty : $"{individual.FirstName} {individual.LastName}",
                        Score = score
                    }, score));
                }
            }

            // Return top N matched applicants sorted by score
            return predictions
                .OrderByDescending(p => p.score)
                .Take(topN)
                .Select(p => p.applicant);
        }

        public async Task<IEnumerable<JobAdvertisementDto>> GetTopMatchedJobAdsForIndividual(Guid individualId, int topN)
        {
            var portfolio = await _portfolioManagementDbContext.Portfolios
                .Include(p => p.PortfolioItems)
                .FirstOrDefaultAsync(p => p.UserId == individualId);

            if (portfolio == null)
            {
                return new List<JobAdvertisementDto>();
            }

            var jobAds =
                await _jobManagementDbContext.JobAdvertisements.ToListAsync();

            var threshold = 0.3;
            var predictions = new List<(JobAdvertisementDto jobAd, double score)>();

            // Calculate similarity for each job advertisement
            foreach (var jobAd in jobAds)
            {
                var portfolioText = string.Join(" ",
                    portfolio.PortfolioItems.Select(item => $"{item.Title} {item.Description} {item.Skills} {item.Technologies} {item.Industry}"));
                var jobAdText = $"{jobAd.JobTitle} {jobAd.JobDescription} {jobAd.RequiredSkills} {jobAd.KeyTechnologies} {jobAd.Industry} {jobAd.Experience} {jobAd.Education} {jobAd.StartupDescription}";
                var score = _similarityCalculator.CalculateAverageMatchingScore(portfolioText, jobAdText);

                if (score > threshold)
                {
                    predictions.Add((new JobAdvertisementDto
                    {
                        Id = jobAd.Id,
                        StartupName = jobAd.StartupName,
                        JobTitle = jobAd.JobTitle,
                        JobLocation = jobAd.JobLocation ?? nameof(JobLocationType.Remote),
                        ApplicationDeadline = jobAd.ApplicationDeadline,
                        Score = score
                    }, score));
                }
            }

            // Return top N matched job advertisements sorted by score
            return predictions
                .OrderByDescending(p => p.score)
                .Take(topN)
                .Select(p => p.jobAd);
        }
    }
}
