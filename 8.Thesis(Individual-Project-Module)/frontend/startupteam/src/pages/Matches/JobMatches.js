import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import SetPageTitle from '../../components/SetPageTitle';

function JobMatches() {
    const [jobs, setJobs] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchMatchedJobs = async () => {
            try {
                const response = await fetchData('/matches/individuals/matched-job-ads');
                setJobs(response.data || []);
            } catch (error) {
                setError('Failed to fetch matched job ads.');
                console.error(error);
            } finally {
                setLoading(false);
            }
        };

        fetchMatchedJobs();
    }, []);

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="matches-section mt-4">
            <SetPageTitle title="Recommended Jobs" />
            <div className="container">
                <div className="row mb-4">
                    <div className="col-12">
                        <div className="d-flex justify-content-between align-items-center">
                            <h1 className="mb-0">Recommended Jobs for You</h1>
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
                                        <div className="d-flex align-items-center mb-2 float-end">
                                            <span className="material-symbols-outlined me-2 text-warning">grade</span>
                                            <p className="card-text mb-0">Score: {job.score.toFixed(2)}</p>
                                        </div>
                                        <Link to={`/jobs/${job.id}`} className="btn btn-primary">More Details</Link>
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

export default JobMatches;
