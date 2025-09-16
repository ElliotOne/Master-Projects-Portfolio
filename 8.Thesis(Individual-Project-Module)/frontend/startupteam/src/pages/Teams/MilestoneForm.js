import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import Form from '../../components/Form';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SubmitButton from '../../components/SubmitButton';
import BackButton from '../../components/BackButton';

function MilestoneForm() {
    const { id, teamId } = useParams();
    const navigate = useNavigate();
    const [generalError, setGeneralError] = useState('');
    const [goals, setGoals] = useState([]);

    const {
        register,
        handleSubmit,
        reset,
        setError,
        formState: { errors },
    } = useForm();

    useEffect(() => {
        if (id) {
            const fetchMilestoneData = async () => {
                try {
                    const response = await fetchData(`/teams/${teamId}/milestones/${id}`);

                    handleApiErrors(response, setError, null, setGeneralError);

                    if (response.data) {

                        // Format the due date to yyyy-MM-dd
                        const formatDate = (dateString) => {
                            if (!dateString) return '';
                            const [datePart] = dateString.split('T');
                            return datePart;
                        };

                        reset({
                            ...response.data,
                            dueDate: formatDate(response.data.dueDate),
                        });
                    }
                } catch (error) {
                    console.error('Failed to fetch milestone details:', error);
                    setGeneralError('An unexpected error occurred.');
                }
            };

            fetchMilestoneData();
        }

        // Fetch available goals for the team
        const fetchGoals = async () => {
            try {
                const response = await fetchData(`/teams/${teamId}/goals`);

                handleApiErrors(response, setError, null, setGeneralError);

                if (Array.isArray(response.data)) {
                    setGoals(response.data);
                } else {
                    setGoals([]);
                }
            } catch (error) {
                console.error('Failed to fetch goals:', error);
                setGeneralError('An unexpected error occurred.');
            }
        };

        fetchGoals();
    }, [id, reset, setError, teamId]);

    const onSubmit = async (data) => {
        try {
            data.teamId = teamId;

            const response = id
                ? await fetchData(`/teams/${teamId}/milestones/${id}`, {
                    method: 'PUT',
                    body: data,
                })
                : await fetchData(`/teams/${teamId}/milestones`, {
                    method: 'POST',
                    body: data,
                });

            const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
            if (hasErrors) {
                return;
            }

            navigate(`/teams/${teamId}`);
        } catch (error) {
            console.error('Milestone submission failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleDelete = async () => {
        if (!id) return;

        if (window.confirm('Are you sure you want to delete this milestone?')) {
            try {
                const response = await fetchData(`/teams/${teamId}/milestones/${id}`, {
                    method: 'DELETE',
                });

                const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
                if (hasErrors) return;

                navigate(`/teams/${teamId}`);
            } catch (error) {
                console.error('Delete milestone failed:', error);
                setGeneralError('An unexpected error occurred.');
            }
        }
    };

    return (
        <Form title={id ? 'Edit Milestone' : 'Create Milestone'} onSubmit={handleSubmit(onSubmit)}>
            <div className="form-group">
                <label htmlFor="title" className="form-label">Title <span className="text-danger">*</span></label>
                <input
                    type="text"
                    id="title"
                    className="form-control"
                    placeholder="e.g., Complete first feature"
                    {...register('title', { required: 'Title is required' })}
                />
                {errors.title && <p className="field-validation-error">{errors.title.message}</p>}
            </div>

            <div className="form-group">
                <label htmlFor="description" className="form-label">Description <span className="text-danger">*</span></label>
                <textarea
                    id="description"
                    className="form-control"
                    rows="6"
                    placeholder="e.g., Detailed description of the milestone."
                    {...register('description', { required: 'Description is required' })}
                ></textarea>
                {errors.description && <p className="field-validation-error">{errors.description.message}</p>}
            </div>

            <div className="form-group">
                <label htmlFor="dueDate" className="form-label">Due Date <span className="text-danger">*</span></label>
                <input
                    type="date"
                    id="dueDate"
                    className="form-control"
                    {...register('dueDate', {
                        required: 'Due date is required',
                        validate: {
                            futureDate: (value) => {
                                const today = new Date().toISOString().split('T')[0];
                                return (value > today) || 'Due date must be a future date';
                            }
                        }
                    })}
                />
                {errors.dueDate && <p className="field-validation-error">{errors.dueDate.message}</p>}
            </div>

            <div className="form-group">
                <label htmlFor="goalId" className="form-label">Goal (Optional)</label>
                <select
                    id="goalId"
                    className="form-control"
                    {...register('goalId')}
                >
                    <option value="">Select a Goal (Optional)</option>
                    {goals.map((goal) => (
                        <option key={goal.id} value={goal.id}>
                            {goal.title}
                        </option>
                    ))}
                </select>
            </div>

            <div className="form-group">
                <label htmlFor="status" className="form-label">Status <span className="text-danger">*</span></label>
                <select
                    id="status"
                    className="form-control"
                    {...register('status', { required: 'Status is required' })}
                >
                    <option value="Pending">Pending</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                    <option value="Overdue">Overdue</option>
                </select>
                {errors.status && <p className="field-validation-error">{errors.status.message}</p>}
            </div>

            {generalError && <p className="field-validation-error">{generalError}</p>}

            <div className="form-group mt-5">
                <SubmitButton text={id ? 'Update Milestone' : 'Create Milestone'} />
                {id && (
                    <button type="button" className="btn btn-danger ms-2 float-end" onClick={handleDelete}>
                        Delete Milestone
                    </button>
                )}
                <BackButton className="btn btn-secondary ms-3" />
            </div>
        </Form>
    );
}

export default MilestoneForm;
