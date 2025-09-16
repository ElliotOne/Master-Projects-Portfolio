import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import { useForm } from 'react-hook-form';
import SubmitButton from '../../components/SubmitButton';
import { useAuth } from '../../context/AuthContext';
import SetPageTitle from '../../components/SetPageTitle';
import BackButton from '../../components/BackButton';

function JobDetail() {
    const { id } = useParams();
    const [job, setJob] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showApplicationForm, setShowApplicationForm] = useState(false);
    const [generalError, setGeneralError] = useState('');
    const [cvFile, setCvFile] = useState(null);
    const [coverLetterFile, setCoverLetterFile] = useState(null);
    const { hasRole } = useAuth();

    const {
        register,
        handleSubmit,
        formState: { errors },
        getValues,
    } = useForm();

    useEffect(() => {
        const fetchJobData = async () => {
            try {
                const response = await fetchData(`/jobadvertisements/details/${id}`);

                handleApiErrors(response, () => setError('An error occurred while fetching data.'), false, setError);

                setJob(response.data || []);
            } catch (error) {
                setError('An unexpected error occurred.');
                console.error('Failed to fetch job advertisements:', error);
            }
            finally {
                setLoading(false);
            }
        };

        fetchJobData();
    }, [id]);

    const handleApplyClick = () => {
        setShowApplicationForm(true);
    };

    const onSubmit = async (data) => {

        data.cVFile = cvFile;
        data.coverLetterFile = coverLetterFile;
        data.jobAdvertisementId = job.id;

        try {
            const response = await fetchData('/jobapplications', {
                method: 'POST',
                body: data,
                isMultiPart: true
            });

            const hasErrors = handleApiErrors(response, setError, () => getValues(), setGeneralError);
            if (hasErrors) {
                return;
            }

            alert('Application submitted successfully!');
            window.location.reload();
        } catch (error) {
            console.error('Application submission failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="job-detail-section py-5">
            <SetPageTitle title="Job Detail" />
            <div className="container">
                <div className="row">
                    <div className="col-12 mb-4 text-center">
                        <h1 className="job-title">{job.jobTitle}</h1>
                    </div>
                    <div className="col-lg-8 col-md-10 mx-auto">
                        <div className="job-summary">

                            {job.startupName && (
                                <div className="summary-item">
                                    <span className="material-symbols-outlined me-2">business</span>
                                    <p className="summary-text">{job.startupName}</p>
                                </div>
                            )}

                            {job.jobLocation && (
                                <div className="summary-item">
                                    <span className="material-symbols-outlined me-2">location_on</span>
                                    <p className="summary-text">{job.jobLocation}</p>
                                </div>
                            )}

                            {job.employmentType && (
                                <div className="summary-item">
                                    <span className="material-symbols-outlined me-2">work</span>
                                    <p className="summary-text">{job.employmentType}</p>
                                </div>
                            )}

                        </div>
                    </div>
                    <div className="col-lg-8 col-md-10 mx-auto">
                        <div className="job-details-card">
                            <h3 className="section-title">Startup Information</h3>

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

                            <h3 className="section-title">Job Details</h3>

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

                            <div className="info-group">
                                <h5 className="info-label">Application Materials</h5>
                                <div className="application-requirements">
                                    <div className="requirement">
                                        <span className={`material-symbols-outlined me-2 ${job.requireCV ? 'required' : 'not-required'}`}>
                                            description
                                        </span>
                                        <span className="requirement-text">{job.requireCV ? 'CV Required' : 'No CV Required'}</span>
                                    </div>
                                    <div className="requirement">
                                        <span className={`material-symbols-outlined me-2 ${job.requireCoverLetter ? 'required' : 'not-required'}`}>
                                            edit_note
                                        </span>
                                        <span className="requirement-text">{job.requireCoverLetter ? 'Cover Letter Required' : 'No Cover Letter Required'}</span>
                                    </div>
                                </div>
                            </div>

                            {hasRole('SkilledIndividual') &&
                                (
                                    <>
                                        {!job.hasApplied && (
                                            <form onSubmit={handleSubmit(onSubmit)} className="mt-5">

                                                {showApplicationForm && (
                                                    <>
                                                        <div className="form-subsection">
                                                            <h3>Upload Documents</h3>

                                                            <div className="form-group">
                                                                <label htmlFor="cv" className="form-label">CV {job?.requireCV && (<span className="text-danger">*</span>)}</label>
                                                                <input
                                                                    type="file"
                                                                    id="cv"
                                                                    className="form-control"
                                                                    accept=".pdf, .docx"
                                                                    {...register('cv', { required: job?.requireCV ? 'CV is required' : false })}
                                                                    onChange={(e) => setCvFile(e.target.files[0])}
                                                                />
                                                                {errors.cv && <p className="field-validation-error">{errors.cv.message}</p>}
                                                            </div>

                                                            <div className="form-group">
                                                                <label htmlFor="coverLetter" className="form-label">Cover Letter {job?.requireCoverLetter && <span className="text-danger">*</span>}</label>
                                                                <input
                                                                    type="file"
                                                                    id="coverLetter"
                                                                    className="form-control"
                                                                    accept=".pdf, .docx"
                                                                    {...register('coverLetter', { required: job?.requireCoverLetter ? 'Cover letter is required' : false })}
                                                                    onChange={(e) => setCoverLetterFile(e.target.files[0])}
                                                                />
                                                                {errors.coverLetter && <p className="field-validation-error">{errors.coverLetter.message}</p>}
                                                            </div>
                                                        </div>
                                                        {generalError && <p className="field-validation-error">{generalError}</p>}
                                                    </>
                                                )}

                                                <div className="form-group mt-5">
                                                    <div className="text-center mt-4">

                                                        {showApplicationForm ? (
                                                            <SubmitButton text="Submit Application" />
                                                        ) : (
                                                            <button
                                                                onClick={handleApplyClick}
                                                                className="btn btn-primary btn-lg"
                                                            >
                                                                Apply Now
                                                            </button>
                                                        )}

                                                    </div>
                                                </div>
                                            </form>
                                        )}

                                        {job.hasApplied && (
                                            <div className="text-center mt-5">
                                                <p className="text-muted">You have already applied for this job.</p>
                                            </div>
                                        )}
                                    </>
                                )
                            }

                        </div>

                        <div className="text-center mt-5">
                            <BackButton className="btn btn-secondary" />
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
}

export default JobDetail;
