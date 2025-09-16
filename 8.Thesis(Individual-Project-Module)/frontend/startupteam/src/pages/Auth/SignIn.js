import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import { GoogleOAuthProvider, GoogleLogin } from '@react-oauth/google';
import config from '../../config';
import SetPageTitle from '../../components/SetPageTitle';

function SignIn() {
    const [generalError, setGeneralError] = useState('');
    const navigate = useNavigate();

    const {
        register,
        handleSubmit,
        setError,
        formState: { errors },
        getValues
    } = useForm();

    const onSubmit = async (data) => {
        try {
            const response = await fetchData('/auth/signin', {
                method: 'POST',
                body: data
            });

            const hasErrors = handleApiErrors(response, setError, () => getValues(), setGeneralError);
            if (hasErrors) {
                return;
            }

            const token = response.data?.token;

            if (token) {
                localStorage.setItem('jwtToken', token);
                navigate('/');
                navigate(0); //Force re-render to ensure correct authorization
            } else {
                setGeneralError('An unexpected error occurred.');
            }
        } catch (error) {
            console.error('Sign in failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    const handleGoogleSuccess = async (callback) => {
        try {
            const response = await fetchData('/auth/external-signin', {
                method: 'POST',
                body: {
                    credential: callback.credential
                }
            });

            const hasErrors = handleApiErrors(response, setError, () => getValues(), setGeneralError);
            if (hasErrors) {
                return;
            }

            const { signUp, externalSignIn, email, firstName, lastName } = response.data || {};

            if (signUp) {
                navigate('/complete-signup', {
                    state: {
                        externalSignIn,
                        email,
                        firstName,
                        lastName
                    }
                });
            }
            else {
                const token = response.data?.token;

                if (token) {
                    localStorage.setItem('jwtToken', token);
                    navigate('/');
                    navigate(0); //Force re-render to ensure correct authorization
                } else {
                    setGeneralError('An unexpected error occurred.');
                }
            }
        } catch (error) {
            console.error('Sign in failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    return (
        <div className="container">
            <SetPageTitle title="SignIn" />
            <section className="auth-section">
                <div className="row justify-content-center">
                    <div className="col-lg-4 col-md-8 col-sm-12 col-12">
                        <div className="card">
                            <div className="card-header text-center">
                                <img className="logo-img" src="/images/logo-full.svg" alt="StartupTeam Logo" />
                            </div>
                            <div className="card-body">
                                <form onSubmit={handleSubmit(onSubmit)}>
                                    <div className="form-group">
                                        <label htmlFor="email">Email <span className="text-danger">*</span></label>
                                        <input
                                            type="email"
                                            id="email"
                                            className="form-control"
                                            {...register('email', { required: 'Email is required' })}
                                        />
                                        {errors.email && <p className="field-validation-error">{errors.email.message}</p>}
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="password">Password <span className="text-danger">*</span></label>
                                        <input
                                            type="password"
                                            id="password"
                                            className="form-control"
                                            {...register('password', { required: 'Password is required' })}
                                        />
                                        {errors.password && <p className="field-validation-error">{errors.password.message}</p>}
                                    </div>

                                    {generalError && <p className="field-validation-error">{generalError}</p>}

                                    <button type="submit" className="btn btn-primary">Sign In</button>
                                </form>
                                <div className="text-center mt-3">
                                    <p>Don't have an account? <Link to="/signup">Sign Up</Link></p>
                                </div>
                                <div className="text-end mt-3">
                                    <GoogleOAuthProvider clientId={config.googleClientId}>
                                        <GoogleLogin
                                            onSuccess={handleGoogleSuccess}
                                            onFailure={() => setGeneralError('Google login failed')}
                                        />
                                    </GoogleOAuthProvider>
                                </div>
                            </div>
                            <div className="card-footer bg-white p-0">
                                <div className="card-footer-item card-footer-item-bordered">
                                    <div className="d-flex justify-content-center">
                                        <Link to="/terms-of-service" className="footer-link me-3">Terms of Service</Link>
                                        <Link to="/privacy-policy" className="footer-link">Privacy Policy</Link>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section >
        </div>
    );
}

export default SignIn;
