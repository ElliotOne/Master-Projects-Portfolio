import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SetPageTitle from '../../components/SetPageTitle';

function JobsManagement() {
    const [jobs, setJobs] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchJobs = async () => {
            try {
                const response = await fetchData('/jobadvertisements/founder-jobs');

                handleApiErrors(response, () => setError('An error occurred while fetching data.'), false, setError);

                setJobs(response.data || []);
            } catch (error) {
                setError('An unexpected error occurred.');
                console.error('Failed to fetch job advertisements:', error);
            }
            finally {
                setLoading(false);
            }
        };

        fetchJobs();
    }, []);

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="jobs-management-section mt-4">
            <SetPageTitle title="My Job Postings" />
            <div className="container">
                <div className="row mb-4">
                    <div className="col-12">
                        <div className="d-flex justify-content-between align-items-center">
                            <h1 className="mb-0">My Job Postings</h1>
                            <Link to="new" className="btn btn-success">
                                <span className="material-symbols-outlined">add</span>
                                Post New Job
                            </Link>
                        </div>
                    </div>
                </div>
                <div className="row">
                    {jobs.length === 0 ? (
                        <div className="col-12">
                            <div className="alert alert-warning">
                                No job postings available.
                            </div>
                        </div>
                    ) : (
                        jobs.map(job => (
                            <div className="col-md-4 mb-4" key={job.id}>
                                <div className="card job-card">
                                    <div className="card-body">
                                        <h5 className="card-title">{job.jobTitle}</h5>
                                        <div className="d-flex align-items-center mb-2">
                                            <span className="material-symbols-outlined me-2">business</span>
                                            <p className="card-text mb-0"><strong>{job.startupName}</strong></p>
                                        </div>
                                        <div className="d-flex align-items-center mb-2">
                                            <span className="material-symbols-outlined me-2">location_on</span>
                                            <p className="card-text mb-0">{job.jobLocation}</p>
                                        </div>
                                        <div className="d-flex align-items-center mb-2">
                                            <span className="material-symbols-outlined me-2">calendar_today</span>
                                            <p className="card-text mb-0">Deadline: {new Date(job.applicationDeadline).toLocaleDateString()}</p>
                                        </div>
                                        <div className="d-flex align-items-center mb-3">
                                            <span className="material-symbols-outlined me-2">{job.status === 'Active' ? 'check_circle' : 'cancel'}</span>
                                            <p className={`card-text mb-0 text-${job.status === 'Active' ? 'success' : 'muted'}`}>{job.status}</p>
                                        </div>
                                        <Link to={`edit/${job.id}`} className="btn btn-secondary">Edit</Link>
                                        <Link to={`/job-applications/manage?jobId=${job.id}`} className="btn btn-primary ms-3">View Applications</Link>
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

export default JobsManagement;
