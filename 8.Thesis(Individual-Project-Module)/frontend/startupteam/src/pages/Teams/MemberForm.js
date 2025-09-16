import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import Form from '../../components/Form';
import SubmitButton from '../../components/SubmitButton';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import BackButton from '../../components/BackButton';

function MilestoneForm() {
    const { id, teamId } = useParams();
    const navigate = useNavigate();
    const [jobs, setJobs] = useState([]);
    const [applicants, setApplicants] = useState([]);
    const [roles, setRoles] = useState([]);
    const [generalError, setGeneralError] = useState('');
    const [isEditMode, setIsEditMode] = useState(false);
    const [memberData, setMemberData] = useState(null); // Store member data for display in edit mode

    const {
        register,
        handleSubmit,
        reset,
        setError,
        setValue,
        formState: { errors },
        watch,
    } = useForm();

    const selectedJob = watch('jobId'); // Watch for job selection

    // Fetch jobs posted by the founder
    const fetchJobs = async () => {
        try {
            const response = await fetchData(`/jobadvertisements/founder-jobs`);
            handleApiErrors(response, setError, null, setGeneralError);

            if (Array.isArray(response.data)) {
                setJobs(response.data);
            } else {
                setJobs([]);
            }
        } catch (error) {
            console.error('Failed to fetch jobs:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    // Fetch available roles for the team
    const fetchRoles = async () => {
        try {
            const response = await fetchData(`/teams/${teamId}/roles`);
            handleApiErrors(response, setError, null, setGeneralError);

            if (Array.isArray(response.data)) {
                setRoles(response.data);
            } else {
                setRoles([]);
            }
        } catch (error) {
            console.error('Failed to fetch roles:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    // Fetch applicants for the selected job
    const fetchApplicants = async (jobId) => {
        if (!jobId) {
            setApplicants([]);
            return;
        }

        try {
            const response = await fetchData(`/jobapplications/${jobId}/successful-applicants`);
            handleApiErrors(response, setError, null, setGeneralError);

            if (Array.isArray(response.data)) {
                setApplicants(response.data);
            } else {
                setApplicants([]);
            }
        } catch (error) {
            console.error('Failed to fetch applicants:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    // Fetch member data in edit mode
    const fetchMemberData = async () => {
        try {
            const response = await fetchData(`/teams/${teamId}/members/${id}`);
            handleApiErrors(response, setError, null, setGeneralError);
            if (response.data) {
                const memberData = response.data;

                // Set specific values manually
                setMemberData(memberData);
                setValue('teamRoleId', memberData.teamRoleId);

                reset(memberData); // Reset form with fetched data
            }
        } catch (error) {
            console.error('Failed to fetch member data:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    useEffect(() => {
        fetchRoles();

        // If `id` is present, we are in edit mode
        if (id) {
            setIsEditMode(true);
            fetchMemberData();
        } else {
            fetchJobs(); // Fetch jobs only in add mode
        }
    }, [id, teamId, reset, setError]);

    useEffect(() => {
        if (selectedJob && !isEditMode) {
            fetchApplicants(selectedJob);
        }
    }, [selectedJob, setError, isEditMode]);

    const onSubmit = async (data) => {
        try {
            data.teamId = teamId;

            const url = id
                ? `/teams/${teamId}/members/${id}`
                : `/teams/${teamId}/members`;

            const response = await fetchData(url, {
                method: id ? 'PUT' : 'POST',
                body: data,
            });

            const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
            if (hasErrors) {
                return;
            }

            navigate(`/teams/${teamId}`);
        } catch (error) {
            console.error('Member submission failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleDelete = async () => {
        if (!id) return;

        if (window.confirm('Are you sure you want to delete this team member?')) {
            try {
                const response = await fetchData(`/teams/${teamId}/members/${id}`, { method: 'DELETE' });

                const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
                if (hasErrors) return;

                navigate(`/teams/${teamId}`);
            } catch (error) {
                setGeneralError('Failed to delete team member.');
                console.error(error);
            }
        }
    };

    return (
        <Form title={id ? 'Edit Team Member' : 'Add Team Member'} onSubmit={handleSubmit(onSubmit)}>
            {/* Show the job selection only if not in edit mode */}
            {!isEditMode && (
                <>
                    <div className="form-group">
                        <label htmlFor="jobId" className="form-label">Select Job <span className="text-danger">*</span></label>
                        <select
                            id="jobId"
                            className="form-control"
                            {...register('jobId', { required: 'Please select a job' })}
                        >
                            <option value="">Select a Job</option>
                            {jobs.map((job) => (
                                <option key={job.id} value={job.id}>
                                    {job.jobTitle}
                                </option>
                            ))}
                        </select>
                        {errors.jobId && <p className="field-validation-error">{errors.jobId.message}</p>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="userId" className="form-label">Select Applicant <span className="text-danger">*</span></label>
                        <select
                            id="userId"
                            className="form-control"
                            {...register('userId', { required: 'Please select an applicant' })}
                            disabled={applicants.length === 0}
                        >
                            <option value="">Select an Applicant</option>
                            {applicants.map((applicant) => (
                                <option key={applicant.userId} value={applicant.userId}>
                                    {applicant.individualFullName} ({applicant.individualEmail})
                                </option>
                            ))}
                        </select>
                        {errors.userId && <p className="field-validation-error">{errors.userId.message}</p>}
                    </div>
                </>
            )}

            {/* In edit mode, display a disabled input with the individual's name */}
            {isEditMode && memberData && (
                <div className="form-group">
                    <label htmlFor="individualFullName" className="form-label">Member Name</label>
                    <input
                        type="text"
                        id="individualFullName"
                        className="form-control"
                        value={memberData.individualFullName}
                        disabled
                    />
                </div>
            )}

            <div className="form-group">
                <label htmlFor="teamRoleId" className="form-label">Select Role <span className="text-danger">*</span></label>
                <select
                    id="teamRoleId"
                    className="form-control"
                    {...register('teamRoleId', { required: 'Please select a role' })}
                >
                    <option value="">Select a Role</option>
                    {roles.map((role) => (
                        <option key={role.id} value={role.id}>
                            {role.name}
                        </option>
                    ))}
                </select>
                {errors.teamRoleId && <p className="field-validation-error">{errors.teamRoleId.message}</p>}
            </div>

            {generalError && <p className="field-validation-error">{generalError}</p>}

            <div className="form-group mt-5">
                <SubmitButton text={id ? 'Update Member' : 'Add Member'} />

                {id && (
                    <button type="button" className="btn btn-danger ms-2 float-end" onClick={handleDelete}>
                        Delete Team Member
                    </button>
                )}

                <BackButton className="btn btn-secondary ms-3" />
            </div>
        </Form>
    );
}

export default MilestoneForm;
