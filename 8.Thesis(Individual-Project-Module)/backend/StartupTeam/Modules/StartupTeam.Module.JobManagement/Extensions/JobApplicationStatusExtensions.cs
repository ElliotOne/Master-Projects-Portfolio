using StartupTeam.Module.JobManagement.Models.Enums;

namespace StartupTeam.Module.JobManagement.Extensions
{
    public static class JobApplicationStatusExtensions
    {
        public static string ToFriendlyString(this JobApplicationStatus status)
        {
            return status switch
            {
                JobApplicationStatus.Submitted => "Submitted",
                JobApplicationStatus.UnderReview => "Under Review",
                JobApplicationStatus.Shortlisted => "Shortlisted",
                JobApplicationStatus.InterviewScheduled => "Interview Scheduled",
                JobApplicationStatus.Interviewed => "Interviewed",
                JobApplicationStatus.OfferExtended => "Offer Extended",
                JobApplicationStatus.OfferAcceptedByIndividual => "Offer Accepted by Individual",
                JobApplicationStatus.OfferRejectedByIndividual => "Offer Rejected by Individual",
                JobApplicationStatus.ApplicationRejectedByFounder => "Application Rejected by Founder",
                JobApplicationStatus.ApplicationWithdrawnByIndividual => "Application Withdrawn by Individual",
                _ => status.ToString()
            };
        }
    }
}
