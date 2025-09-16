import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import SetPageTitle from '../../components/SetPageTitle';

function DashboardOverview() {
    const { hasRole } = useAuth();

    return (
        <section className="dashaboard-overview-section">
            <SetPageTitle title="Dashboard" />
            <div className="container">
                <div className="row">
                    <div className="col-12">
                        <h1 className="mb-4">Dashboard Overview</h1>
                        <p>Welcome! Choose an option below.</p>
                    </div>
                    <div className="row text-center mt-5">

                        <div className="col-md-3 col-sm-6 mb-4">
                            <Link to="jobs" className="btn btn-secondary btn-dashboard">
                                <span className="material-symbols-outlined ico me-2">work_outline</span>
                                <span className="text">View Job Listings</span>
                            </Link>
                        </div>

                        {hasRole('StartupFounder') && (
                            <>
                                <div className="col-md-3 col-sm-6 mb-4">
                                    <Link to="jobs/manage" className="btn btn-secondary btn-dashboard">
                                        <span className="material-symbols-outlined ico me-2">folder_open</span>
                                        <span className="text">View My Job Postings</span>
                                    </Link>
                                </div>
                                <div className="col-md-3 col-sm-6 mb-4">
                                    <Link to="job-applications/manage" className="btn btn-secondary btn-dashboard">
                                        <span className="material-symbols-outlined ico me-2">description</span>
                                        <span className="text">View Job Applications</span>
                                    </Link>
                                </div>
                                <div className="col-md-3 col-sm-6 mb-4">
                                    <Link to="teams/manage" className="btn btn-secondary btn-dashboard">
                                        <span className="material-symbols-outlined ico me-2">groups</span>
                                        <span className="text">View Teams</span>
                                    </Link>
                                </div>
                            </>
                        )}

                        {hasRole('SkilledIndividual') && (
                            <>
                                <div className="col-md-3 col-sm-6 mb-4">
                                    <Link to="job-applications" className="btn btn-secondary btn-dashboard">
                                        <span className="material-symbols-outlined ico me-2">description</span>
                                        <span className="text">View Job Applications</span>
                                    </Link>
                                </div>
                                <div className="col-md-3 col-sm-6 mb-4">
                                    <Link to="portfolio/manage" className="btn btn-secondary btn-dashboard">
                                        <span className="material-symbols-outlined ico me-2">folder</span>
                                        <span className="text">View Portfolio</span>
                                    </Link>
                                </div>
                                <div className="col-md-3 col-sm-6 mb-4">
                                    <Link to="job-matches" className="btn btn-secondary btn-dashboard">
                                        <span className="material-symbols-outlined ico me-2">search</span>
                                        <span className="text">View Job Matches</span>
                                    </Link>
                                </div>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </section>
    );
}

export default DashboardOverview;
