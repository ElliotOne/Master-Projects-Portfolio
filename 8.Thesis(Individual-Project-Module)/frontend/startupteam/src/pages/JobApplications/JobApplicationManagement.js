import React, { useEffect, useState } from 'react';
import { Link, useSearchParams, useNavigate } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SetPageTitle from '../../components/SetPageTitle';

function JobApplicationManagement() {
    const [applications, setApplications] = useState([]);
    const [jobAdvertisements, setJobAdvertisements] = useState([]);
    const [selectedJobId, setSelectedJobId] = useState('');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [searchParams] = useSearchParams();  // Get the query params
    const navigate = useNavigate();

    const jobIdFromQuery = searchParams.get('jobId');  // Get jobId from query params

    // Fetch all job applications (optionally by job ID)
    const fetchApplications = async (jobId = '') => {
        try {
            const endpoint = jobId ? `/jobapplications/founder-applications?jobId=${jobId}` : '/jobapplications/founder-applications';
            const response = await fetchData(endpoint);

            handleApiErrors(response, () => setError('An error occurred while fetching data.'), false, setError);

            setApplications(response.data || []);
        } catch (error) {
            setError('An unexpected error occurred.');
            console.error('Failed to fetch job applications:', error);
        } finally {
            setLoading(false);
        }
    };

    // Fetch job advertisements for the founder
    const fetchJobAdvertisements = async () => {
        try {
            const response = await fetchData('/jobadvertisements/founder-jobs');
            handleApiErrors(response, () => setError('Failed to load job advertisements.'), false, setError);

            setJobAdvertisements(response.data || []);
        } catch (error) {
            console.error('Failed to fetch job advertisements:', error);
            setError('An unexpected error occurred.');
        }
    };

    useEffect(() => {
        fetchJobAdvertisements();
        // If jobIdFromQuery is present, fetch applications based on that; otherwise, fetch all.
        fetchApplications(jobIdFromQuery || '');
        // Set the initial selected job based on query param
        setSelectedJobId(jobIdFromQuery || '');
    }, [jobIdFromQuery]);

    // Handle job selection change
    const handleJobChange = (e) => {
        const jobId = e.target.value;
        setSelectedJobId(jobId);
        fetchApplications(jobId);
    };

    // Handle redirect to Applicant Matches
    const handleRedirectToMatches = () => {
        if (selectedJobId) {
            navigate(`/applicant-matches?jobAdId=${selectedJobId}`);
        } else {
            alert('Please select a job advertisement before viewing matched applicants.');
        }
    };

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="job-applications-management-section mt-4">
            <SetPageTitle title="Job Applications" />
            <div className="container">
                <div className="row mb-4">
                    <div className="col-12">
                        <h1 className="mb-0">Job Applications</h1>
                    </div>
                </div>
                {/* Select Job Advertisement */}
                <div className="row mb-4">
                    <div className="col-md-6">
                        <select
                            className="form-control"
                            value={selectedJobId}
                            onChange={handleJobChange}
                        >
                            <option value="">All Job Advertisements</option>
                            {jobAdvertisements.map((job) => (
                                <option key={job.id} value={job.id}>
                                    {job.jobTitle}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="col-md-6 text-right">
                        <button className="btn btn-primary" onClick={handleRedirectToMatches}>
                            View Matched Applicants
                        </button>
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
                                        <Link to={`edit/${application.id}`} className="btn btn-secondary">Edit</Link>
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

export default JobApplicationManagement;
