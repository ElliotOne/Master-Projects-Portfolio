import React, { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SetPageTitle from '../../components/SetPageTitle';

function ApplicantMatches() {
    const [applications, setApplications] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [searchParams] = useSearchParams();
    const jobIdFromQuery = searchParams.get('jobAdId'); // Get jobAdId from query params

    // Fetch matched applicants
    const fetchMatchedApplicants = async (jobAdId = '') => {
        try {
            const endpoint = `/matches/founder/matched-applicants?jobAdId=${jobAdId}`;
            const response = await fetchData(endpoint);

            handleApiErrors(response, () => setError('An error occurred while fetching matched applicants.'), false, setError);

            setApplications(response.data || []);
        } catch (error) {
            setError('An unexpected error occurred.');
            console.error('Failed to fetch matched applicants:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (jobIdFromQuery) {
            fetchMatchedApplicants(jobIdFromQuery);
        }
    }, [jobIdFromQuery]);

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="matches-section mt-4">
            <SetPageTitle title="Relevant Applicants" />
            <div className="row mb-4">
                <div className="col-12">
                    <div className="d-flex justify-content-between align-items-center">
                        <h1 className="mb-0">Top Applicants for Your Job Posting</h1>
                    </div>
                </div>
            </div>
            <div className="row">
                {applications.length === 0 ? (
                    <div className="col-12">
                        <div className="alert alert-warning">
                            No matched applicants available.
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
                                        <span className="material-symbols-outlined me-2">person</span>
                                        <p className="card-text mb-0">{application.individualFullName}</p>
                                    </div>
                                    <div className="d-flex align-items-center mb-2">
                                        <span className="material-symbols-outlined me-2">calendar_today</span>
                                        <p className="card-text mb-0">Applied: {new Date(application.applicationDate).toLocaleDateString()}</p>
                                    </div>
                                    <div className="d-flex align-items-center mb-2">
                                        <span className="material-symbols-outlined me-2">assignment_turned_in</span>
                                        <p className="card-text mb-0">Status: {application.status}</p>
                                    </div>
                                    <div className="d-flex align-items-center mb-2 float-end">
                                        <span className="material-symbols-outlined me-2 text-warning">grade</span>
                                        <p className="card-text mb-0">Score: {application.score.toFixed(2)}</p>
                                    </div>
                                    <Link to={`/job-applications/manage/edit/${application.id}`} className="btn btn-secondary">Edit</Link>
                                </div>
                            </div>
                        </div>
                    ))
                )}
            </div>
        </section>
    );
}

export default ApplicantMatches;
