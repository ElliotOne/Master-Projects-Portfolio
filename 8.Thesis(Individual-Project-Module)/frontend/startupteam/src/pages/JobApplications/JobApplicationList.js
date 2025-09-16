import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import SetPageTitle from '../../components/SetPageTitle';

function JobApplicationList() {
    const [applications, setApplications] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchJobApplications = async () => {
            try {
                const response = await fetchData('/jobapplications/individual-applications');
                setApplications(response.data || []);
            } catch (error) {
                setError('Failed to fetch job applications.');
                console.error(error);
            } finally {
                setLoading(false);
            }
        };

        fetchJobApplications();
    }, []);

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="job-application-list-section mt-4">
            <SetPageTitle title="Job Applications" />
            <div className="container">
                <div className="row mb-4">
                    <div className="col-12">
                        <div className="d-flex justify-content-between align-items-center">
                            <h1 className="mb-0">Job Applications</h1>
                        </div>
                    </div>
                </div>
                <div className="row">
                    {applications.length === 0 ? (
                        <div className="col-12">
                            <div className="alert alert-warning">
                                No job applications available.
                            </div>
                        </div>
                    ) : (
                        applications.map(application => (
                            <div className="col-md-4 mb-4" key={application.id}>
                                <div className="card job-application-card">
                                    <div className="card-body">
                                        <h5 className="card-title">{application.jobTitle}</h5>
                                        <div className="d-flex align-items-center mb-2">
                                            <span className="material-symbols-outlined me-2">business</span>
                                            <p className="card-text mb-0"><strong>{application.startupName}</strong></p>
                                        </div>
                                        <div className="d-flex align-items-center mb-2">
                                            <span className="material-symbols-outlined me-2">location_on</span>
                                            <p className="card-text mb-0">{application.jobLocation}</p>
                                        </div>
                                        <div className="d-flex align-items-center mb-2">
                                            <span className="material-symbols-outlined me-2">calendar_today</span>
                                            <p className="card-text mb-0">Applied: {new Date(application.applicationDate).toLocaleDateString()}</p>
                                        </div>
                                        <div className="d-flex align-items-center mb-2">
                                            <span className="material-symbols-outlined me-2">assignment_turned_in</span>
                                            <p className="card-text mb-0">Status: {application.status}</p>
                                        </div>
                                        <Link to={`/job-applications/${application.id}`} className="btn btn-primary">More Details</Link>
                                    </div>
                                </div>
                            </div>
                        ))
                    )}
                </div>
            </div>
        </section>
    );
}

export default JobApplicationList;
