import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import Form from '../../components/Form';
import SubmitButton from '../../components/SubmitButton';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import BackButton from '../../components/BackButton';

function PortfolioItemForm() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [generalError, setGeneralError] = useState('');
    const [attachmentFile, setAttachmentFile] = useState(null);
    const [attachmentUrl, setAttachmentUrl] = useState(null);
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
            const fetchPortfolioItemData = async () => {
                try {
                    const response = await fetchData(`/portfolios/${id}`);

                    handleApiErrors(response, setError, null, setGeneralError);

                    // Populate form fields with data
                    if (response.data) {

                        // Store userId
                        setUserId(response.data.userId);

                        reset(response.data);
                        setAttachmentFile(response.data.attachmentFile || null);

                        if (response.data.attachmentUrl) {
                            setAttachmentUrl(response.data.attachmentUrl);
                        }
                    }
                } catch (error) {
                    console.error('Failed to fetch portfolio item details:', error);
                    setGeneralError('An unexpected error occurred.');
                }
            };

            fetchPortfolioItemData();
        }
    }, [id, reset, setError]);

    const onSubmit = async (data) => {
        try {

            if (id) {
                data.userId = userId;
            }

            data.attachmentFile = attachmentFile;

            const response = id
                ? await fetchData(`/portfolios/${id}`, {
                    method: 'PUT',
                    body: data,
                    isMultiPart: true
                })
                : await fetchData('/portfolios', {
                    method: 'POST',
                    body: data,
                    isMultiPart: true
                });

            const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
            if (hasErrors) {
                return;
            }

            navigate('/portfolio/manage');
        } catch (error) {
            console.error('Portfolio item submission failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleDelete = async () => {
        if (!id) return;

        if (window.confirm('Are you sure you want to delete this portfolio item?')) {
            try {
                const response = await fetchData(`/portfolios/${id}`, {
                    method: 'DELETE'
                });

                const hasErrors = handleApiErrors(response, setError, null, setGeneralError);

                if (hasErrors) return;

                navigate('/portfolio/manage');
            } catch (error) {
                console.error('Delete portfolio item failed:', error);
                setGeneralError('An unexpected error occurred.');
            }
        }
    };

    return (
        <Form title={id ? 'Edit Portfolio Item' : 'Create Portfolio Item'} onSubmit={handleSubmit(onSubmit)}>
            <div className="form-group">
                <label htmlFor="title" className="form-label">Title <span className="text-danger">*</span></label>
                <input
                    type="text"
                    id="title"
                    className="form-control"
                    placeholder="e.g., Project Alpha"
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
                    placeholder="e.g., Detailed description of the project, objectives, and outcomes."
                    {...register('description', { required: 'Description is required' })}
                ></textarea>
                {errors.description && <p className="field-validation-error">{errors.description.message}</p>}
            </div>
            <div className="form-group">
                <label htmlFor="type" className="form-label">Type</label>
                <input
                    type="text"
                    id="type"
                    className="form-control"
                    placeholder="e.g., Project, Research, Design"
                    {...register('type')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="technologies" className="form-label">Technologies</label>
                <input
                    type="text"
                    id="technologies"
                    className="form-control"
                    placeholder="e.g., React, Node.js, Python"
                    {...register('technologies')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="skills" className="form-label">Skills</label>
                <input
                    type="text"
                    id="skills"
                    className="form-control"
                    placeholder="e.g., Front-end Development, API Integration"
                    {...register('skills')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="industry" className="form-label">Industry</label>
                <input
                    type="text"
                    id="industry"
                    className="form-control"
                    placeholder="e.g., FinTech, HealthTech"
                    {...register('industry')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="role" className="form-label">Role</label>
                <input
                    type="text"
                    id="role"
                    className="form-control"
                    placeholder="e.g., Lead Developer, Designer"
                    {...register('role')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="duration" className="form-label">Duration</label>
                <input
                    type="text"
                    id="duration"
                    className="form-control"
                    placeholder="e.g., 3 months, 6 weeks"
                    {...register('duration')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="link" className="form-label">Link</label>
                <input
                    type="url"
                    id="link"
                    className="form-control"
                    placeholder="e.g., https://example.com/project-alpha"
                    {...register('link')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="attachmentFile" className="form-label">Attachment</label>
                <input
                    type="file"
                    id="attachmentFile"
                    className="form-control"
                    {...register('attachmentFile')}
                    onChange={(e) => setAttachmentFile(e.target.files[0])}
                />
                <small className="form-text text-muted">Upload a single file (e.g., image, document).</small>

                {attachmentUrl && (
                    <div className="mt-2">
                        <a href={attachmentUrl} target="_blank" rel="noopener noreferrer" className='text-primary'>
                            Download current attachment
                        </a>
                    </div>
                )}
            </div>
            <div className="form-group">
                <label htmlFor="tags" className="form-label">Tags</label>
                <input
                    type="text"
                    id="tags"
                    className="form-control"
                    placeholder="e.g., Web Development, UI/UX"
                    {...register('tags')}
                />
            </div>
            {generalError && <p className="field-validation-error">{generalError}</p>}
            <div className="form-group mt-5">
                <SubmitButton text={id ? 'Update Portfolio Item' : 'Create Portfolio Item'} />
                {id && (
                    <button type="button" className="btn btn-danger ms-2 float-end" onClick={handleDelete}>
                        Delete Portfolio Item
                    </button>
                )}
                <BackButton className="btn btn-secondary ms-3" />
            </div>
        </Form>
    );
}

export default PortfolioItemForm;
