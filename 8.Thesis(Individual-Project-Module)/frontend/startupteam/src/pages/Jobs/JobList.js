import React, { useEffect, useState } from 'react';
import { fetchData } from '../../utils/fetchData';
import JobCard from '../../components/JobCard';
import SetPageTitle from '../../components/SetPageTitle';

function JobList() {
    const [jobs, setJobs] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchJobs = async () => {
            try {
                const response = await fetchData('/jobadvertisements/all-jobs');
                setJobs(response.data || []);
            } catch (error) {
                setError('Failed to fetch job listings.');
                console.error(error);
            } finally {
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
        <section className="job-list-section mt-4">
            <SetPageTitle title="Job Listings" />
            <div className="container">
                <div className="row mb-4">
                    <div className="col-12">
                        <div className="d-flex justify-content-between align-items-center">
                            <h1 className="mb-0">Job Listings</h1>
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
                                <JobCard job={job} />
                            </div>
                        ))
                    )}
                </div>
            </div>
        </section>
    );
}

export default JobList;
