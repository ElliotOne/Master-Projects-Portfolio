import React from 'react';
import { Link } from 'react-router-dom';
import SetPageTitle from '../../components/SetPageTitle';

function Unauthorized() {
    return (
        <div className="container">
            <SetPageTitle title="Unauthorized" />
            <section className="auth-section">
                <div className="row justify-content-center">
                    <div className="col-lg-6 col-md-8 col-sm-12 col-12">
                        <div className="card">
                            <div className="card-header text-center">
                                <img className="logo-img" src="/images/logo-full.svg" alt="StartupTeam Logo" />
                            </div>
                            <div className="card-body text-center">
                                <h2 className="text-danger">Unauthorized Access</h2>
                                <p>You do not have permission this page.</p>
                                <div className="mt-3">
                                    <Link to="/" className="btn btn-primary">Go to Homepage</Link>
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

export default Unauthorized;
