import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import SetPageTitle from '../../components/SetPageTitle';

function SignUpSuccess() {
    const { state } = useLocation();
    const [error, setError] = useState(null);
    const { externalSignIn, token } = state || {};
    const navigate = useNavigate();

    if (externalSignIn) {
        if (token) {
            localStorage.setItem('jwtToken', token);
        } else {
            setError('No token provided');
        }
    }

    const handleDashboardNavigation = () => {
        navigate('/');
        navigate(0); //Force re-render to ensure correct authorization
    };

    return (
        <div className="container">
            <SetPageTitle title="SignUp Success" />
            <section className="auth-section">
                <div className="row justify-content-center">
                    <div className="col-lg-6 col-md-8 col-sm-12 col-12">
                        <div className="card">
                            <div className="card-header text-center">
                                <img className="logo-img" src="/images/logo-full.svg" alt="StartupTeam Logo" />
                            </div>
                            <div className="card-body text-center">
                                {
                                    error ? (
                                        <p className="text-danger">{error}</p>
                                    ) : (
                                        <>
                                            <h2 className="text-success">Sign-Up Successful!</h2>
                                            <p>Your account has been created successfully.</p>
                                            <p>Please check your email for a confirmation link to verify your account.</p>
                                            <p>If you don't see the email, please check your spam folder.</p>
                                            <div className="mt-3">
                                                {
                                                    externalSignIn ? (
                                                        <button to="/" className="btn btn-primary" onClick={handleDashboardNavigation}>Go Dashboard</button>
                                                    ) :
                                                        (<Link to="/signin" className="btn btn-primary">Go to Signin</Link>)
                                                }
                                            </div>
                                        </>
                                    )
                                }
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

export default SignUpSuccess;
