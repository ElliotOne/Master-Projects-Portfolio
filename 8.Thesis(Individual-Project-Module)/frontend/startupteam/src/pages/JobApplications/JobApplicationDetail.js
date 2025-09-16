import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SetPageTitle from '../../components/SetPageTitle';
import BackButton from '../../components/BackButton';

function JobApplicationDetail() {
    const { id } = useParams();
    const [job, setJob] = useState(null);
    const [jobApplication, setJobApplication] = useState(null);
    const [loading, setLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState('');
    const [generalError, setGeneralError] = useState('');

    useEffect(() => {
        const fetchJobAndApplicationData = async () => {
            try {
                // Fetch job application details
                const applicationResponse = await fetchData(`/jobapplications/${id}`);

                handleApiErrors(applicationResponse, () => setErrorMessage('An error occurred while fetching data.'), false, setGeneralError);

                if (applicationResponse.data) {
                    setJobApplication(applicationResponse.data);

                    // Fetch job details using the jobAdvertisementId from the job application
                    const jobResponse = await fetchData(`/jobadvertisements/details/${applicationResponse.data.jobAdvertisementId}`);

                    handleApiErrors(jobResponse, () => setErrorMessage('An error occurred while fetching data.'), false, setGeneralError);

                    if (jobResponse.data) {
                        setJob(jobResponse.data);
                    }
                }

            } catch (error) {
                setErrorMessage('Failed to load data.');
                console.error(error);
            } finally {
                setLoading(false);
            }
        };

        fetchJobAndApplicationData();
    }, [id]);

    const updateApplicationStatus = async (status) => {
        try {
            const response = await fetchData(`/jobapplications/${id}/update-status`, {
                method: 'PUT',
                body: { status },
            });

            const hasErrors = handleApiErrors(response, setError => setGeneralError(setError), false, setGeneralError);
            if (hasErrors) {
                return;
            }

            alert(`Application status updated successfully!`);
            window.location.reload();
        } catch (error) {
            console.error('Failed to update job application:', error);
            setGeneralError('An unexpected error occurred while updating the job application.');
        }
    };

    const renderActionButtons = () => {
        const { status } = jobApplication;

        if (status === 'Submitted' ||
            status === 'UnderReview' ||
            status === 'Shortlisted' ||
            status === 'InterviewScheduled') {
            return (
                <div className="form-group">
                    <button className="btn btn-warning" onClick={() => updateApplicationStatus('ApplicationWithdrawnByIndividual')}>
                        Withdraw Application
                    </button>
                </div>
            );
        } else if (status === 'OfferExtended') {
            return (
                <div className="form-group">
                    <button className="btn btn-success me-2" onClick={() => updateApplicationStatus('OfferAcceptedByIndividual')}>
                        Accept Offer
                    </button>
                    <button className="btn btn-danger" onClick={() => updateApplicationStatus('OfferRejectedByIndividual')}>
                        Reject Offer
                    </button>
                </div>
            );
        }

        return null;
    };

    const shouldShowUpdateSection = () => {
        const { status } = jobApplication;
        return (
            status === 'Submitted' ||
            status === 'UnderReview' ||
            status === 'Shortlisted' ||
            status === 'InterviewScheduled' ||
            status === 'OfferExtended'
        );
    };

    // Format the date to yyyy-MM-dd
    const formatDate = (dateString) => {
        if (!dateString) return '';
        const [datePart] = dateString.split('T');
        return datePart;
    };

    // Format the date to yyyy-MM-dd HH:mm
    const formatDateTime = (dateString) => {
        if (!dateString) return '';
        const [datePart, timePart] = dateString.split('T');
        const [hours, minutes] = timePart.split(':');
        return `${datePart} ${hours}:${minutes}`;
    };

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (errorMessage) {
        return <div className="text-center mt-5">{errorMessage}</div>;
    }

    return (
        <section className="job-application-section py-5">
            <SetPageTitle title="Job Application Detail" />
            <div className="container">
                <div className="row">
                    <div className="col-12 mb-5">
                        <div className="section-title text-center">
                            <h2>Job Application Detail</h2>
                        </div>
                    </div>
                    <div className="col-lg-8 col-md-10 mx-auto">
                        {job && (
                            <div className="job-details-card">
                                <h3 className="section-title">Job Details</h3>

                                <div className="info-group">
                                    <h5 className="info-label">Job Title</h5>
                                    <p className="info-text">{job.jobTitle}</p>
                                </div>

                                {job.jobDescription && (
                                    <div className="info-group">
                                        <h5 className="info-label">Job Description</h5>
                                        <p className="info-text">{job.jobDescription}</p>
                                    </div>
                                )}

                                {job.requiredSkills && (
                                    <div className="info-group">
                                        <h5 className="info-label">Required Skills</h5>
                                        <p className="info-text">{job.requiredSkills}</p>
                                    </div>
                                )}

                                {job.jobResponsibilities && (
                                    <div className="info-group">
                                        <h5 className="info-label">Responsibilities</h5>
                                        <p className="info-text">{job.jobResponsibilities}</p>
                                    </div>
                                )}

                                {job.salaryRange && (
                                    <div className="info-group">
                                        <h5 className="info-label">Salary Range</h5>
                                        <p className="info-text">{job.salaryRange}</p>
                                    </div>
                                )}

                                {job.employmentType && (
                                    <div className="info-group">
                                        <h5 className="info-label">Employment Type</h5>
                                        <p className="info-text">{job.employmentType}</p>
                                    </div>
                                )}

                                {job.jobLocationType && (
                                    <div className="info-group">
                                        <h5 className="info-label">Location Type</h5>
                                        <p className="info-text">{job.jobLocationType}</p>
                                    </div>
                                )}

                                {job.jobLocation && (
                                    <div className="info-group">
                                        <h5 className="info-label">Job Location</h5>
                                        <p className="info-text">{job.jobLocation}</p>
                                    </div>
                                )}

                                {job.applicationDeadline && (
                                    <div className="info-group">
                                        <h5 className="info-label">Application Deadline</h5>
                                        <p className="info-text">{job.applicationDeadline}</p>
                                    </div>
                                )}

                                {job.experience && (
                                    <div className="info-group">
                                        <h5 className="info-label">Experience</h5>
                                        <p className="info-text">{job.experience}</p>
                                    </div>
                                )}

                                {job.education && (
                                    <div className="info-group">
                                        <h5 className="info-label">Education</h5>
                                        <p className="info-text">{job.education}</p>
                                    </div>
                                )}

                                <h3 className="section-title">Startup Information</h3>

                                {job.startupName && (
                                    <div className="info-group">
                                        <h5 className="info-label">Startup Name</h5>
                                        <p className="info-text">{job.startupName}</p>
                                    </div>
                                )}

                                {job.startupDescription && (
                                    <div className="info-group">
                                        <h5 className="info-label">Startup Description</h5>
                                        <p className="info-text">{job.startupDescription}</p>
                                    </div>
                                )}

                                {job.startupStage && (
                                    <div className="info-group">
                                        <h5 className="info-label">Startup Stage</h5>
                                        <p className="info-text">{job.startupStage}</p>
                                    </div>
                                )}

                                {job.industry && (
                                    <div className="info-group">
                                        <h5 className="info-label">Industry</h5>
                                        <p className="info-text">{job.industry}</p>
                                    </div>
                                )}

                                {job.keyTechnologies && (
                                    <div className="info-group">
                                        <h5 className="info-label">Key Technologies</h5>
                                        <p className="info-text">{job.keyTechnologies}</p>
                                    </div>
                                )}

                                {job.uniqueSellingPoints && (
                                    <div className="info-group">
                                        <h5 className="info-label">Unique Selling Points</h5>
                                        <p className="info-text">{job.uniqueSellingPoints}</p>
                                    </div>
                                )}

                                {job.missionStatement && (
                                    <div className="info-group">
                                        <h5 className="info-label">Mission Statement</h5>
                                        <p className="info-text">{job.missionStatement}</p>
                                    </div>
                                )}

                                {job.foundingYear && (
                                    <div className="info-group">
                                        <h5 className="info-label">Founding Year</h5>
                                        <p className="info-text">{job.foundingYear}</p>
                                    </div>
                                )}

                                {job.teamSize && (
                                    <div className="info-group">
                                        <h5 className="info-label">Team Size</h5>
                                        <p className="info-text">{job.teamSize}</p>
                                    </div>
                                )}

                                {job.startupWebsites && (
                                    <div className="info-group">
                                        <h5 className="info-label">Startup Website</h5>
                                        <a href={job.startupWebsite} target="_blank" rel="noopener noreferrer" className="info-link">{job.startupWebsite}</a>
                                    </div>
                                )}

                                {job.startupValues && (
                                    <div className="info-group">
                                        <h5 className="info-label">Startup Values</h5>
                                        <p className="info-text">{job.startupValues}</p>
                                    </div>
                                )}
                            </div>
                        )}

                        <div className="founder-details-card">
                            <h3 className="section-title">Founder</h3>
                            <div className="info-group">
                                <h5 className="info-label">Full Name</h5>
                                <p className="info-text">{jobApplication.founderFullName}</p>

                                <Link to={`/users/${jobApplication.founderUserName}`} className="btn btn-info">View Profile</Link>
                                <Link to={`/jobs/${job.id}`} className="btn btn-primary ms-3">View Job</Link>
                            </div>
                        </div>

                        <div className="application-materials-card">
                            <h3 className="section-title">Application Materials</h3>
                            <div className="form-group">
                                {jobApplication?.cvUrl ? (
                                    <a href={jobApplication.cvUrl} target="_blank" rel="noopener noreferrer" className="form-control-link">
                                        View CV
                                    </a>
                                ) : (
                                    <p>No CV provided</p>
                                )}
                            </div>
                            <div className="form-group">
                                {jobApplication?.coverLetterUrl ? (
                                    <a href={jobApplication.coverLetterUrl} target="_blank" rel="noopener noreferrer" className="form-control-link">
                                        View Cover Letter
                                    </a>
                                ) : (
                                    <p>No Cover Letter provided</p>
                                )}
                            </div>
                        </div>

                        <div className="application-form-card">
                            <div className="info-group">
                                <h5 className="info-label">Application Staus</h5>
                                <p className="info-text">{jobApplication.statusText}</p>
                            </div>

                            <div className="info-group">
                                <h5 className="info-label">Application Date</h5>
                                <p className="info-text">{formatDateTime(jobApplication.applicationDate)}</p>
                            </div>

                            {jobApplication.interviewDate && (
                                <div className="info-group">
                                    <h5 className="info-label">Interview Date</h5>
                                    <p className="info-text">
                                        {formatDate(jobApplication.interviewDate)}
                                    </p>
                                </div>
                            )}

                            {shouldShowUpdateSection() && (
                                <>
                                    <h3 className="section-title">Update Application</h3>
                                    {renderActionButtons()}
                                </>
                            )}

                            {generalError && <p className="field-validation-error">{generalError}</p>}
                        </div>

                    </div>
                    <div className="col-12 text-center mt-5">
                        <BackButton className="btn btn-secondary" />
                    </div>
                </div>
            </div>
        </section >
    );
}

export default JobApplicationDetail;
