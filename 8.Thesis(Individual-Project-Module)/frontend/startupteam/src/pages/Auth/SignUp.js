import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SetPageTitle from '../../components/SetPageTitle';

function SignUp() {
    const [generalError, setGeneralError] = useState('');

    const {
        register,
        handleSubmit,
        setError,
        formState: { errors },
        getValues
    } = useForm();

    const navigate = useNavigate();

    const onSubmit = async (data) => {
        try {
            const response = await fetchData('/auth/signup', {
                method: 'POST',
                body: data
            });

            const hasErrors = handleApiErrors(response, setError, () => getValues(), setGeneralError);
            if (hasErrors) {
                return;
            }

            const userEmail = response.data?.email;

            navigate('/complete-signup', { state: { email: userEmail } });
        } catch (error) {
            console.error('Sign up failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    return (
        <div className="container">
            <SetPageTitle title="SignUp" />
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
                                            {...register('password', {
                                                required: 'Password is required',
                                                minLength: {
                                                    value: 6,
                                                    message: 'Password must be at least 6 characters long'
                                                },
                                                maxLength: {
                                                    value: 32,
                                                    message: 'Password cannot exceed 32 characters'
                                                }
                                            })}
                                        />
                                        {errors.password && <p className="field-validation-error">{errors.password.message}</p>}
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="confirmPassword">Confirm Password <span className="text-danger">*</span></label>
                                        <input
                                            type="password"
                                            id="confirmPassword"
                                            className="form-control"
                                            {...register('confirmPassword', {
                                                required: 'Confirm Password is required',
                                                validate: value => value === getValues('password') || 'Passwords do not match'
                                            })}
                                        />
                                        {errors.confirmPassword && <p className="field-validation-error">{errors.confirmPassword.message}</p>}
                                    </div>

                                    {generalError && <p className="field-validation-error">{generalError}</p>}

                                    <button type="submit" className="btn btn-primary">Sign Up</button>
                                </form>
                                <div className="text-center mt-3">
                                    <p>Already have an account? <Link to="/signin">Sign In</Link></p>
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
            </section>
        </div>
    );
}

export default SignUp;
