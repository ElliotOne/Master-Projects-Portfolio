import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import Form from '../../components/Form';
import SubmitButton from '../../components/SubmitButton';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import BackButton from '../../components/BackButton';

function TeamForm() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [generalError, setGeneralError] = useState('');
    const [userId, setUserId] = useState(null);

    const {
        register,
        handleSubmit,
        reset,
        setError,
        formState: { errors },
    } = useForm();

    useEffect(() => {
        if (id) {
            const fetchTeamData = async () => {
                try {
                    const response = await fetchData(`/teams/${id}`);

                    handleApiErrors(response, setError, null, setGeneralError);

                    if (response.data) {
                        setUserId(response.data.userId);
                        reset(response.data);
                    }
                } catch (error) {
                    console.error('Failed to fetch team details:', error);
                    setGeneralError('An unexpected error occurred.');
                }
            };

            fetchTeamData();
        }
    }, [id, reset, setError]);

    const onSubmit = async (data) => {
        try {
            if (id) {
                data.userId = userId;
            }

            const response = id
                ? await fetchData(`/teams/${id}`, {
                    method: 'PUT',
                    body: data
                })
                : await fetchData('/teams', {
                    method: 'POST',
                    body: data
                });

            const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
            if (hasErrors) {
                return;
            }

            navigate('/teams/manage');
        } catch (error) {
            console.error('Team submission failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleDelete = async () => {
        if (!id) return;

        if (window.confirm('Are you sure you want to delete this team?')) {
            try {
                const response = await fetchData(`/teams/${id}`, {
                    method: 'DELETE'
                });

                const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
                if (hasErrors) return;

                navigate('/teams/manage');
            } catch (error) {
                console.error('Delete team failed:', error);
                setGeneralError('An unexpected error occurred.');
            }
        }
    };

    return (
        <Form title={id ? 'Edit Team' : 'Create Team'} onSubmit={handleSubmit(onSubmit)}>
            <div className="form-group">
                <label htmlFor="name" className="form-label">Team Name <span className="text-danger">*</span></label>
                <input
                    type="text"
                    id="name"
                    className="form-control"
                    placeholder="e.g., Development Team Alpha"
                    {...register('name', { required: 'Team name is required' })}
                />
                {errors.name && <p className="field-validation-error">{errors.name.message}</p>}
            </div>
            <div className="form-group">
                <label htmlFor="description" className="form-label">Description <span className="text-danger">*</span></label>
                <textarea
                    id="description"
                    className="form-control"
                    rows="6"
                    placeholder="e.g., A team dedicated to building our core platform."
                    {...register('description', { required: 'Description is required' })}
                ></textarea>
                {errors.description && <p className="field-validation-error">{errors.description.message}</p>}
            </div>

            {generalError && <p className="field-validation-error">{generalError}</p>}
            <div className="form-group mt-5">
                <SubmitButton text={id ? 'Update Team' : 'Create Team'} />
                {id && (
                    <button type="button" className="btn btn-danger ms-2 float-end" onClick={handleDelete}>
                        Delete Team
                    </button>
                )}
                <BackButton className="btn btn-secondary ms-3" />
            </div>
        </Form>
    );
}

export default TeamForm;
