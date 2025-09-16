import { useState, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SetPageTitle from '../../components/SetPageTitle';

function CompleteSignUp() {
    const [generalError, setGeneralError] = useState('');

    const { state } = useLocation();
    const email = state?.email || '';
    const externalSignIn = state?.externalSignIn || false;

    const {
        register,
        handleSubmit,
        setError,
        formState: { errors },
        getValues,
        reset
    } = useForm();

    const navigate = useNavigate();

    useEffect(() => {
        if (state) {
            const { firstName, lastName } = state;
            reset({
                firstName: firstName || '',
                lastName: lastName || ''
            });
        }
    }, [state, reset]);

    const onSubmit = async (data) => {
        try {
            const response = await fetchData('/auth/complete-signup', {
                method: 'POST',
                body: {
                    ...data,
                    email: email,
                    externalSignIn: externalSignIn
                },
            });

            const hasErrors = handleApiErrors(response, setError, () => getValues(), setGeneralError);
            if (hasErrors) {
                return;
            }

            const { token } = response.data || {};

            if (token) {
                navigate('/signup-success', {
                    state: {
                        externalSignIn,
                        token
                    }
                });
            } else {
                navigate('/signup-success');
            }

        } catch (error) {
            console.error('Complete sign-up failed:', error);
            setGeneralError('An unexpected error occurred.');
        }
    };

    return (
        <div className="container">
            <SetPageTitle title="Complete SignUp" />
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
                                        <label htmlFor="username">Username <span className="text-danger">*</span></label>
                                        <input
                                            type="text"
                                            id="username"
                                            name="username"
                                            className="form-control"
                                            {...register('username', {
                                                required: 'Username is required',
                                                minLength: {
                                                    value: 6,
                                                    message: 'Username must be at least 6 characters long'
                                                }
                                            })}
                                        />
                                        {errors.username && <p className="field-validation-error">{errors.username.message}</p>}
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="firstName">First Name <span className="text-danger">*</span></label>
                                        <input
                                            type="text"
                                            id="firstName"
                                            name="firstName"
                                            className="form-control"
                                            {...register('firstName', { required: 'First name is required' })}
                                        />
                                        {errors.firstName && <p className="field-validation-error">{errors.firstName.message}</p>}
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="lastName">Last Name <span className="text-danger">*</span></label>
                                        <input
                                            type="text"
                                            id="lastName"
                                            name="lastName"
                                            className="form-control"
                                            {...register('lastName', { required: 'Last name is required' })}
                                        />
                                        {errors.lastName && <p className="field-validation-error">{errors.lastName.message}</p>}
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="roleName">Role <span className="text-danger">*</span></label>
                                        <select
                                            id="roleName"
                                            name="roleName"
                                            className="form-control"
                                            {...register('roleName', { required: 'Role is required' })}
                                        >
                                            <option value="">Select Role</option>
                                            <option value="StartupFounder">Startup Founder</option>
                                            <option value="SkilledIndividual">Skilled Individual</option>
                                        </select>
                                        {errors.roleName && <p className="field-validation-error">{errors.roleName.message}</p>}
                                    </div>

                                    {generalError && <p className="field-validation-error">{generalError}</p>}

                                    <button type="submit" className="btn btn-primary">Complete Sign Up</button>
                                </form>
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

export default CompleteSignUp;
