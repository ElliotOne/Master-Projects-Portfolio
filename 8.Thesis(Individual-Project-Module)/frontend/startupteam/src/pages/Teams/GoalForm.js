import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import Form from '../../components/Form';
import SubmitButton from '../../components/SubmitButton';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import BackButton from '../../components/BackButton';

function GoalForm() {
    const { id, teamId } = useParams();
    const navigate = useNavigate();
    const [generalError, setGeneralError] = useState('');
    const {
        register,
        handleSubmit,
        reset,
        setError,
        formState: { errors },
    } = useForm();

    useEffect(() => {
        if (id) {
            const fetchGoalData = async () => {
                try {
                    const response = await fetchData(`/teams/${teamId}/goals/${id}`);

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
                            dueDate: formatDate(response.data.dueDate)
                        });
                    }
                } catch (error) {
                    console.error('Failed to fetch goal details:', error);
                    setGeneralError('An unexpected error occurred.');
                }
            };

            fetchGoalData();
        }
    }, [id, reset, setError]);

    const onSubmit = async (data) => {
        try {

            data.teamId = teamId;

            const response = id
                ? await fetchData(`/teams/${teamId}/goals/${id}`, {
                    method: 'PUT',
                    body: data,
                })
                : await fetchData(`/teams/${teamId}/goals`, {
                    method: 'POST',
                    body: data,
                });

            const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
            if (hasErrors) {
                return;
            }

            navigate(`/teams/${teamId}`);
        } catch (error) {
            console.error('Goal submission failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleDelete = async () => {
        if (!id) return;

        if (window.confirm('Are you sure you want to delete this goal?')) {
            try {
                const response = await fetchData(`/teams/${teamId}/goals/${id}`, {
                    method: 'DELETE'
                });

                const hasErrors = handleApiErrors(response, setError, null, setGeneralError);

                if (hasErrors) return;

                navigate(`/teams/${teamId}`);
            } catch (error) {
                console.error('Delete goal failed:', error);
                setGeneralError('An unexpected error occurred.');
            }
        }
    };

    return (
        <Form title={id ? 'Edit Goal' : 'Create Goal'} onSubmit={handleSubmit(onSubmit)}>
            <div className="form-group">
                <label htmlFor="title" className="form-label">Title <span className="text-danger">*</span></label>
                <input
                    type="text"
                    id="title"
                    className="form-control"
                    placeholder="e.g., Complete UI redesign"
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
                    placeholder="e.g., Detailed description of the goal and its objectives."
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
                <label htmlFor="status" className="form-label">Status <span className="text-danger">*</span></label>
                <select
                    id="status"
                    className="form-control"
                    {...register('status', { required: 'Status is required' })}
                >
                    <option value="NotStarted">Not Started</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                    <option value="OnHold">On Hold</option>
                </select>
                {errors.status && <p className="field-validation-error">{errors.status.message}</p>}
            </div>

            {generalError && <p className="field-validation-error">{generalError}</p>}

            <div className="form-group mt-5">
                <SubmitButton text={id ? 'Update Goal' : 'Create Goal'} />
                {id && (
                    <button type="button" className="btn btn-danger ms-2 float-end" onClick={handleDelete}>
                        Delete Goal
                    </button>
                )}
                <BackButton className="btn btn-secondary ms-3" />
            </div>
        </Form>
    );
}

export default GoalForm;
