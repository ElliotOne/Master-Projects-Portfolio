import React, { useState, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import Form from '../../components/Form';
import SubmitButton from '../../components/SubmitButton';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import BackButton from '../../components/BackButton';

function JobForm() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [generalError, setGeneralError] = useState('');
    const [showJobLocation, setShowJobLocation] = useState(false);
    const [userId, setUserId] = useState(null);

    const {
        register,
        handleSubmit,
        watch,
        setValue,
        setError,
        clearErrors,
        formState: { errors },
        getValues,
        reset
    } = useForm();

    useEffect(() => {
        // Fetch existing job if editing
        if (id) {
            const fetchJobData = async () => {
                try {
                    const response = await fetchData(`/jobadvertisements/${id}`);

                    handleApiErrors(response, setError, () => getValues(), setGeneralError);

                    // Populate form fields with data
                    if (response.data) {

                        // Store userId
                        setUserId(response.data.userId);

                        // Format the date to yyyy-MM-dd
                        const formatDate = (dateString) => {
                            if (!dateString) return '';
                            const [datePart] = dateString.split('T');
                            return datePart;
                        };

                        reset({
                            ...response.data,
                            applicationDeadline: formatDate(response.data.applicationDeadline)
                        });

                        setShowJobLocation(response.data.jobLocationType === 'OnSite' || response.data.jobLocationType === 'Hybrid');
                    }
                } catch (error) {
                    console.error('Failed to fetch job details:', error);
                    setGeneralError('An unexpected error occurred.');
                }
            };

            fetchJobData();
        }
    }, [id, reset, setError, getValues]);

    const onSubmit = async (data) => {
        try {
            if (id) {
                data.userId = userId;
            }

            const response = id
                ? await fetchData(`/jobadvertisements/${id}`, {
                    method: 'PUT',
                    body: data
                })
                : await fetchData('/jobadvertisements', {
                    method: 'POST',
                    body: data
                });

            const hasErrors = handleApiErrors(response, setError, () => getValues(), setGeneralError);
            if (hasErrors) {
                return;
            }

            navigate('/jobs/manage');
        } catch (error) {
            console.error('Job submission failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleDelete = async () => {
        if (!id) return;

        if (window.confirm('Are you sure you want to delete this job?')) {
            try {
                const response = await fetchData(`/jobadvertisements/${id}`, {
                    method: 'DELETE'
                });

                const hasErrors = handleApiErrors(response, setError, () => getValues(), setGeneralError);

                if (hasErrors) return;

                navigate('/jobs/manage');
            } catch (error) {
                console.error('Delete job failed:', error);
                setGeneralError('An unexpected error occurred.');
            }
        }
    };

    const jobLocationType = watch('jobLocationType');
    useEffect(() => {
        if (jobLocationType === 'OnSite' || jobLocationType === 'Hybrid') {
            setShowJobLocation(true);
            // Dynamically set the jobLocation field as required
            setValue('jobLocation', '', { shouldValidate: true }); // Reset value to trigger validation
        } else {
            setShowJobLocation(false);
            // Remove the required validation
            clearErrors('jobLocation');
        }
    }, [jobLocationType, setValue, clearErrors]);

    return (
        <Form title={id ? 'Edit Job' : 'Create Job'} onSubmit={handleSubmit(onSubmit)}>
            <div className="form-subsection">
                <h3>Startup Information</h3>

                <div className="form-group">
                    <label htmlFor="startupName" className="form-label">Startup Name <span className="text-danger">*</span></label>
                    <input
                        type="text"
                        id="startupName"
                        className="form-control"
                        {...register('startupName', { required: 'Startup name is required' })}
                    />
                    {errors.startupName && <p className="field-validation-error">{errors.startupName.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="startupDescription" className="form-label">Startup Description <span className="text-danger">*</span></label>
                    <textarea
                        id="startupDescription"
                        className="form-control"
                        rows="6"
                        {...register('startupDescription', { required: 'Startup description is required' })}
                    ></textarea>
                    {errors.startupDescription && <p className="field-validation-error">{errors.startupDescription.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="startupStage" className="form-label">Startup Stage <span className="text-danger">*</span></label>
                    <select
                        id="startupStage"
                        className="form-control"
                        {...register('startupStage', { required: 'Startup stage is required' })}
                    >
                        <option value="">Select Stage</option>
                        <option value="Idea">Idea</option>
                        <option value="PreSeed">Pre-Seed</option>
                        <option value="Seed">Seed</option>
                        <option value="SeriesA">Series A</option>
                        <option value="SeriesB">Series B</option>
                        <option value="SeriesC">Series C</option>
                        <option value="IPO">IPO</option>
                    </select>
                    {errors.startupStage && <p className="field-validation-error">{errors.startupStage.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="industry" className="form-label">Industry <span className="text-danger">*</span></label>
                    <input
                        type="text"
                        id="industry"
                        className="form-control"
                        {...register('industry', { required: 'Industry is required' })}
                        placeholder="e.g., FinTech, HealthTech"
                    />
                    {errors.industry && <p className="field-validation-error">{errors.industry.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="keyTechnologies" className="form-label">Key Technologies</label>
                    <input
                        type="text"
                        id="keyTechnologies"
                        className="form-control"
                        {...register('keyTechnologies')}
                        placeholder="e.g., React, Node.js"
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="uniqueSellingPoints" className="form-label">Unique Selling Points</label>
                    <textarea
                        id="uniqueSellingPoints"
                        className="form-control"
                        rows="4"
                        {...register('uniqueSellingPoints')}
                        placeholder="Highlight what makes the startup unique compared to other startups."
                    ></textarea>
                </div>

                <div className="form-group">
                    <label htmlFor="missionStatement" className="form-label">Mission Statement</label>
                    <textarea
                        id="missionStatement"
                        className="form-control"
                        rows="2"
                        {...register('missionStatement')}
                    ></textarea>
                </div>

                <div className="form-group">
                    <label htmlFor="foundingYear" className="form-label">Founding Year</label>
                    <input
                        type="number"
                        id="foundingYear"
                        className="form-control"
                        {...register('foundingYear',
                            {
                                min: {
                                    value: 2000,
                                    message: 'Founding year must be 2000 or later'
                                },
                                max: {
                                    value: new Date().getFullYear(),
                                    message: `Founding year cannot be later than ${new Date().getFullYear()}`
                                }
                            })}
                        placeholder="e.g., 2024"
                    />
                    {errors.foundingYear && <p className="field-validation-error">{errors.foundingYear.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="teamSize" className="form-label">Team Size</label>
                    <input
                        type="number"
                        id="teamSize"
                        className="form-control"
                        {...register('teamSize',
                            {
                                min: {
                                    value: 1,
                                    message: 'Team size must be at least 1'
                                },
                                max: {
                                    value: 10000,
                                    message: 'Team size cannot exceed 10000'
                                }
                            })}
                        placeholder="e.g., 10"
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="startupWebsite" className="form-label">Startup Website</label>
                    <input
                        type="text"
                        id="startupWebsite"
                        className="form-control"
                        {...register('startupWebsite')}
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="startupValues" className="form-label">Startup Values</label>
                    <textarea
                        id="startupValues"
                        className="form-control"
                        rows="4"
                        {...register('startupValues')}
                    ></textarea>
                </div>
            </div>

            <div className="form-subsection">
                <h3>Job Details</h3>

                <div className="form-group">
                    <label htmlFor="jobTitle" className="form-label">Job Title <span className="text-danger">*</span></label>
                    <input
                        type="text"
                        id="jobTitle"
                        className="form-control"
                        {...register('jobTitle', { required: 'Job title is required' })}
                    />
                    {errors.jobTitle && <p className="field-validation-error">{errors.jobTitle.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="jobDescription" className="form-label">Job Description <span className="text-danger">*</span></label>
                    <textarea
                        id="jobDescription"
                        className="form-control"
                        rows="6"
                        {...register('jobDescription', { required: 'Job description is required' })}
                    ></textarea>
                    {errors.jobDescription && <p className="field-validation-error">{errors.jobDescription.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="employmentType" className="form-label">Employment Type <span className="text-danger">*</span></label>
                    <select
                        id="employmentType"
                        className="form-control"
                        {...register('employmentType', { required: 'Employment type is required' })}
                    >
                        <option value="">Select Employment Type</option>
                        <option value="FullTime">Full-time</option>
                        <option value="PartTime">Part-time</option>
                        <option value="Contract">Contract</option>
                        <option value="Internship">Internship</option>
                    </select>
                    {errors.employmentType && <p className="field-validation-error">{errors.employmentType.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="requiredSkills" className="form-label">Required Skills</label>
                    <input
                        type="text"
                        id="requiredSkills"
                        className="form-control"
                        {...register('requiredSkills')}
                        placeholder="e.g., React, Node.js"
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="jobResponsibilities" className="form-label">Responsibilities</label>
                    <textarea
                        id="jobResponsibilities"
                        className="form-control"
                        rows="4"
                        {...register('jobResponsibilities')}
                    ></textarea>
                </div>

                <div className="form-group">
                    <label htmlFor="salaryRange" className="form-label">Salary Range</label>
                    <input
                        type="text"
                        id="salaryRange"
                        className="form-control"
                        {...register('salaryRange')}
                        placeholder="e.g., $60,000 - $80,000"
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="jobLocationType" className="form-label">Location Type <span className="text-danger">*</span></label>
                    <select
                        id="jobLocationType"
                        {...register('jobLocationType', { required: 'Location type is required' })}
                        className="form-control"
                    >
                        <option value="">Select Location Type</option>
                        <option value="OnSite">On-site</option>
                        <option value="Remote">Remote</option>
                        <option value="Hybrid">Hybrid (Remote & On-site)</option>
                    </select>
                    {errors.jobLocationType && <p className="field-validation-error">{errors.jobLocationType.message}</p>}
                </div>

                {showJobLocation && (
                    <div className="form-group">
                        <label htmlFor="jobLocation" className="form-label">Job Location <span className="text-danger">*</span></label>
                        <input
                            type="text"
                            id="jobLocation"
                            className="form-control"
                            {...register('jobLocation', {
                                required: jobLocationType === 'OnSite' || jobLocationType === 'Hybrid' ? 'Job location is required' : false
                            })}
                        />
                        {errors.jobLocation && <p className="field-validation-error">{errors.jobLocation.message}</p>}
                    </div>
                )}

                <div className="form-group">
                    <label htmlFor="applicationDeadline" className="form-label">Application Deadline <span className="text-danger">*</span></label>
                    <input
                        type="date"
                        id="applicationDeadline"
                        className="form-control"
                        {...register('applicationDeadline', {
                            required: 'Application deadline is required',
                            validate: {
                                futureDate: (value) => {
                                    const today = new Date().toISOString().split('T')[0];
                                    return (value > today) || 'Application deadline must be a future date';
                                }
                            }
                        })}
                    />
                    {errors.applicationDeadline && <p className="field-validation-error">{errors.applicationDeadline.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="experience" className="form-label">Experience</label>
                    <input
                        type="text"
                        id="experience"
                        className="form-control"
                        {...register('experience')}
                        placeholder="e.g., 3+ years in software development"
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="education" className="form-label">Education</label>
                    <input
                        type="text"
                        id="education"
                        className="form-control"
                        {...register('education')}
                        placeholder="e.g., Bachelor's degree in Computer Science"
                    />
                </div>

                <div className="form-group">
                    <div className="form-check">
                        <input
                            type="checkbox"
                            id="requireCV"
                            {...register('requireCV')}
                            className="form-check-input"
                        />
                        <label htmlFor="requireCV" className="form-check-label">
                            Check this if a <span className="text-primary">CV</span> is required for the application.
                        </label>
                    </div>
                </div>

                <div className="form-group">
                    <div className="form-check">
                        <input
                            type="checkbox"
                            id="requireCoverLetter"
                            {...register('requireCoverLetter')}
                            className="form-check-input"
                        />
                        <label htmlFor="requireCoverLetter" className="form-check-label">
                            Check this if a <span className="text-primary">Cover Letter</span> is required for the application.
                        </label>
                    </div>
                </div>

                {generalError && <p className="field-validation-error">{generalError}</p>}
            </div>

            <div className="form-group mt-5">
                <SubmitButton text={id ? 'Update Job' : 'Create Job'} />

                {id && (
                    <button type="button" className="btn btn-danger ms-2 float-end" onClick={handleDelete}>
                        Delete Job
                    </button>
                )}

                <BackButton className="btn btn-secondary ms-3" />
            </div>
        </Form>
    );
}

export default JobForm;
