import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useParams } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import { useForm } from 'react-hook-form';
import SubmitButton from '../../components/SubmitButton';
import BackButton from '../../components/BackButton';
import SetPageTitle from '../../components/SetPageTitle';

function JobApplicationForm() {
    const { id } = useParams();
    const [job, setJob] = useState(null);
    const [jobApplication, setJobApplication] = useState(null);
    const [loading, setLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState('');
    const [generalError, setGeneralError] = useState('');
    const [showInterviewDate, setShowInterviewDate] = useState(false);

    const {
        register,
        handleSubmit,
        watch,
        setValue,
        setError,
        formState: { errors },
        getValues,
        reset
    } = useForm();

    const status = watch('status');  // Watch for status changes

    useEffect(() => {
        const fetchJobAndApplicationData = async () => {
            try {
                // Fetch job application details
                const applicationResponse = await fetchData(`/jobapplications/${id}`);

                handleApiErrors(applicationResponse, () => setErrorMessage('An error occurred while fetching data.'), false, setGeneralError);

                if (applicationResponse.data) {
                    setJobApplication(applicationResponse.data);

                    reset({
                        ...applicationResponse.data,
                        interviewDate: formatDate(applicationResponse.data.interviewDate)
                    });

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
    }, [id, setValue]);

    // Disable update if the application is in a concluded state
    const isUpdateAllowed = ![
        'OfferAcceptedByIndividual',
        'OfferRejectedByIndividual',
        'ApplicationRejectedByFounder',
        'ApplicationWithdrawnByIndividual',
    ].includes(jobApplication?.status);

    useEffect(() => {
        if (status === 'InterviewScheduled') {
            setShowInterviewDate(true);
            setValue('interviewDate', '', { shouldValidate: true });  // Reset and validate interviewDate field when required
        } else {
            setShowInterviewDate(false);
            setValue('interviewDate', null);  // Clear interviewDate when not required
        }
    }, [status, setValue]);

    const onSubmit = async (data) => {
        try {
            const response = await fetchData(`/jobapplications/${id}`, {
                method: 'PUT',
                body: data,
            });

            const hasErrors = handleApiErrors(response, setError, () => getValues(), setGeneralError);
            if (hasErrors) {
                return;
            }

            alert('Job application updated successfully!');
            window.location.reload();
        } catch (error) {
            console.error('Failed to update job application:', error);
            setGeneralError('An unexpected error occurred while updating the job application.');
        }
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
            <SetPageTitle title="Edit Job Application" />
            <div className="container">
                <div className="row">
                    <div className="col-12 mb-5">
                        <div className="section-title text-center">
                            <h2>Edit Job Application</h2>
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

                                {job.jobLocation && (
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

                        <div className="applicant-details-card">
                            <h3 className="section-title">Applicant</h3>
                            <div className="info-group">
                                <h5 className="info-label">Full Name</h5>
                                <p className="info-text">{jobApplication.individualFullName}</p>

                                <Link to={`/users/${jobApplication.individualUserName}`} className="btn btn-info">View Profile</Link>
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

                            {isUpdateAllowed ? (
                                <>
                                    <h3 className="section-title">Update Application</h3>
                                    <form onSubmit={handleSubmit(onSubmit)}>
                                        <div className="form-group">
                                            <label htmlFor="status" className="form-label">New Application Status <span className="text-danger">*</span></label>
                                            <select id="status" className="form-control" {...register('status', { required: 'Status is required' })}>
                                                <option value="Submitted">Submitted</option>
                                                <option value="UnderReview">Under Review</option>
                                                <option value="Shortlisted">Shortlisted</option>
                                                <option value="InterviewScheduled">Interview Scheduled</option>
                                                <option value="Interviewed">Interviewed</option>
                                                <option value="OfferExtended">Offer Extended</option>
                                                <option value="ApplicationRejectedByFounder">Rejected</option>
                                            </select>
                                            {errors.status && <p className="field-validation-error">{errors.status.message}</p>}
                                        </div>

                                        {showInterviewDate && (
                                            <div className="form-group">
                                                <label htmlFor="interviewDate" className="form-label">Interview Date <span className="text-danger">*</span></label>
                                                <input
                                                    type="date"
                                                    id="interviewDate"
                                                    className="form-control"
                                                    {...register('interviewDate', {
                                                        required: status === 'InterviewScheduled' ? 'Interview date is required when the status is Interview Scheduled' : false,
                                                        validate: {
                                                            futureDate: (value) => {
                                                                if (!value) return true;

                                                                const today = new Date().toISOString().split('T')[0];
                                                                return (value > today) || 'Interview date must be a future date';
                                                            }
                                                        }
                                                    })}
                                                />
                                                {errors.interviewDate && <p className="field-validation-error">{errors.interviewDate.message}</p>}
                                            </div>
                                        )}

                                        {generalError && <p className="field-validation-error">{generalError}</p>}

                                        <div className="form-group mt-5">
                                            <SubmitButton text="Update Application" />
                                            <BackButton className="btn btn-secondary ms-3" />
                                        </div>
                                    </form>
                                </>
                            ) : (
                                <>
                                    <div className="alert alert-warning">
                                        Updates are not allowed for applications that have been accepted, rejected, or withdrawn.
                                    </div>
                                    <div className="text-center">
                                        <BackButton className="btn btn-secondary ms-3" />
                                    </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
}

export default JobApplicationForm;
