using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Models.Enums;

namespace StartupTeam.Module.JobManagement.Services
{
    public interface IJobService
    {
        Task<IEnumerable<JobAdvertisementDto>> GetJobAdvertisementsAsync();
        Task<IEnumerable<JobAdvertisementDto>> GetJobAdvertisementsForUserAsync(Guid userId);
        Task<IEnumerable<JobAdvertisementDto>> GetJobAdvertisementsByUserIdAsync(Guid userId);
        Task<JobAdvertisementDetailDto?> GetJobAdvertisementByIdAsync(Guid id, Guid individualId);
        Task<JobAdvertisementFormDto?> GetJobAdvertisementFormByIdAsync(Guid id);
        Task<bool> CreateJobAdvertisementAsync(JobAdvertisementFormDto advertisementFormDto, Guid userId);
        Task<bool> UpdateJobAdvertisementAsync(JobAdvertisementFormDto advertisementFormDto);
        Task<bool> DeleteJobAdvertisementAsync(Guid id);
        Task<IEnumerable<JobApplicationDto>> GetJobApplicationsByIndividualIdAsync(Guid individualId);
        Task<IEnumerable<JobApplicationDto>> GetJobApplicationsByFounderIdAsync(Guid founderId, Guid? jobAdvertisementId);
        Task<IEnumerable<JobApplicantDto>> GetSuccessfulJobApplicantsByJobAdvertisementIdAsync(Guid jobAdvertisementId);
        Task<JobApplicationDetailDto?> GetJobApplicationByIdAsync(Guid id);
        Task<JobApplicationUpdateFormDto?> GetJobApplicationFormByIdAsync(Guid id);
        Task<bool> SubmitJobApplicationAsync(JobApplicationFormDto applicationFormDto, Guid individualId);
        Task<bool> UpdateJobApplicationAsync(JobApplicationUpdateFormDto jobApplicationUpdateFormDto);
        Task<bool> UpdateApplicationStatusByIndividualAsync(Guid id, JobApplicationStatus status);
        Task<bool> HasUserAlreadyAppliedAsync(Guid id, Guid individualId);
    }
}
