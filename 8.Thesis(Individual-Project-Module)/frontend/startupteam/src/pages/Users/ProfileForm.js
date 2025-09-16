import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import Form from '../../components/Form';
import ImageWithFallback from '../../components/ImageWithFallback';
import SubmitButton from '../../components/SubmitButton';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import BackButton from '../../components/BackButton';

function ProfileForm() {
    const navigate = useNavigate();
    const [generalError, setGeneralError] = useState('');
    const [profileImage, setProfileImage] = useState(null);
    const [profilePictureFile, setProfilePictureFile] = useState(null);

    const {
        register,
        handleSubmit,
        reset,
        setError,
        formState: { errors },
    } = useForm();

    useEffect(() => {
        const fetchProfileData = async () => {
            try {
                const response = await fetchData('/profiles');

                // Populate form fields with data
                if (response.data) {

                    reset(response.data);

                    if (response.data.profilePictureUrl) {
                        setProfileImage(response.data.profilePictureUrl);
                    }
                }
            } catch (error) {
                console.error('Failed to fetch profile data:', error);
                setGeneralError('An unexpected error occurred.');
            }
        };

        fetchProfileData();
    }, [reset]);

    const onSubmit = async (data) => {
        try {
            const userName = data.userName;
            data.profilePictureFile = profilePictureFile;

            const response = await fetchData(`/profiles/${userName}`, {
                method: 'PUT',
                body: data,
                isMultiPart: true
            });

            const hasErrors = handleApiErrors(response, setError, null, setGeneralError);
            if (hasErrors) {
                return;
            }

            alert('Profile updated successfully.');
            navigate('/profile');
        } catch (error) {
            console.error('Profile update failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            setProfilePictureFile(file);
            const reader = new FileReader();
            reader.onloadend = () => {
                setProfileImage(reader.result);
            };
            reader.readAsDataURL(file);
        }
    };

    return (
        <Form title="Profile" onSubmit={handleSubmit(onSubmit)}>
            <div className="form-group profile-image-container text-center">
                <ImageWithFallback
                    className="img"
                    src={profileImage}
                    alt="Profile"
                    fallbackSrc="/images/profile-avatar.png"
                />
            </div>
            <div className="form-group">
                <input
                    id="file"
                    type="file"
                    className="form-control mt-3"
                    {...register('profilePictureFile')}
                    accept=".jpeg, .jpg, .png"
                    onChange={handleFileChange}
                />
                <small className="form-text text-muted">Upload a new profile picture.</small>
                {errors.profilePictureFile && <p className="field-validation-error">{errors.profilePictureFile.message}</p>}
            </div>
            <div className="form-group">
                <label htmlFor="userName" className="form-label">Username</label>
                <input
                    id="userName"
                    name="userName"
                    className="form-control"
                    disabled
                    {...register('userName')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="email" className="form-label">Email</label>
                <input
                    id="email"
                    name="email"
                    className="form-control"
                    disabled
                    {...register('email')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="phoneNumber" className="form-label">Phone Number</label>
                <input
                    id="phoneNumber"
                    name="phoneNumber"
                    className="form-control"
                    {...register('phoneNumber')}
                />
            </div>
            <div className="form-group">
                <label htmlFor="firstName" className="form-label">First Name <span className="text-danger">*</span></label>
                <input
                    id="firstName"
                    name="firstName"
                    className="form-control"
                    {...register('firstName', { required: 'First Name is required' })}
                />
                {errors.firstName && <p className="field-validation-error">{errors.firstName.message}</p>}
            </div>
            <div className="form-group">
                <label htmlFor="lastName" className="form-label">Last Name <span className="text-danger">*</span></label>
                <input
                    id="lastName"
                    name="lastName"
                    className="form-control"
                    {...register('lastName', { required: 'Last Name is required' })}
                />
                {errors.lastName && <p className="field-validation-error">{errors.lastName.message}</p>}
            </div>
            <div className="form-group">
                <label htmlFor="roleName" className="form-label">Role</label>
                <input
                    id="roleName"
                    name="roleName"
                    className="form-control"
                    disabled
                    {...register('roleName')}
                />
            </div>
            {generalError && <p className="field-validation-error">{generalError}</p>}
            <div className="form-group mt-5">
                <SubmitButton text="Update Profile" />
                <BackButton className="btn btn-secondary ms-3" />
            </div>
        </Form>
    );
}

export default ProfileForm;
