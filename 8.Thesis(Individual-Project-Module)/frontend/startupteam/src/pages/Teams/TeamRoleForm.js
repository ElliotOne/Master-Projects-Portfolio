import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import Form from '../../components/Form';
import SubmitButton from '../../components/SubmitButton';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import BackButton from '../../components/BackButton';

function TeamRoleForm() {
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
            const fetchRoleData = async () => {
                try {
                    const response = await fetchData(`/teams/${teamId}/roles/${id}`);

                    handleApiErrors(response, setError, null, setGeneralError);

                    if (response.data) {
                        reset(response.data);
                    }
                } catch (error) {
                    console.error('Failed to fetch role details:', error);
                    setGeneralError('An unexpected error occurred.');
                }
            };

            fetchRoleData();
        }
    }, [id, teamId, reset, setError]);

    const onSubmit = async (data) => {
        try {
            data.teamId = teamId;

            const response = id
                ? await fetchData(`/teams/${teamId}/roles/${id}`, {
                    method: 'PUT',
                    body: data,
                })
                : await fetchData(`/teams/${teamId}/roles`, {
                    method: 'POST',
                    body: data,
                });

            const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
            if (hasErrors) return;

            navigate(`/teams/${teamId}`);
        } catch (error) {
            console.error('Role submission failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleDelete = async () => {
        if (!id) return;

        if (window.confirm('Are you sure you want to delete this role?')) {
            try {
                const response = await fetchData(`/teams/${teamId}/roles/${id}`, {
                    method: 'DELETE',
                });

                const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
                if (hasErrors) return;

                navigate(`/teams/${teamId}`);
            } catch (error) {
                console.error('Delete role failed:', error);
                setGeneralError('An unexpected error occurred.');
            }
        }
    };

    return (
        <Form title={id ? 'Edit Team Role' : 'Create Team Role'} onSubmit={handleSubmit(onSubmit)}>
            <div className="form-group">
                <label htmlFor="name" className="form-label">Role Name <span className="text-danger">*</span></label>
                <input
                    type="text"
                    id="name"
                    className="form-control"
                    placeholder="e.g., Developer"
                    {...register('name', { required: 'Role name is required' })}
                />
                {errors.name && <p className="field-validation-error">{errors.name.message}</p>}
            </div>

            <div className="form-group">
                <label htmlFor="description" className="form-label">Description <span className="text-danger">*</span></label>
                <textarea
                    id="description"
                    className="form-control"
                    rows="6"
                    placeholder="e.g., Description of the responsibilities of the role."
                    {...register('description', { required: 'Description is required' })}
                ></textarea>
                {errors.description && <p className="field-validation-error">{errors.description.message}</p>}
            </div>

            {generalError && <p className="field-validation-error">{generalError}</p>}

            <div className="form-group mt-5">
                <SubmitButton text={id ? 'Update Role' : 'Create Role'} />
                {id && (
                    <button type="button" className="btn btn-danger ms-2 float-end" onClick={handleDelete}>
                        Delete Role
                    </button>
                )}
                <BackButton className="btn btn-secondary ms-3" />
            </div>
        </Form>
    );
}

export default TeamRoleForm;
