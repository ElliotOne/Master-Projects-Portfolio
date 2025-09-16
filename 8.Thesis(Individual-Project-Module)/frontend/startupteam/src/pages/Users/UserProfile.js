import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useParams } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import ImageWithFallback from '../../components/ImageWithFallback';
import JobCard from '../../components/JobCard';
import SetPageTitle from '../../components/SetPageTitle';
import BackButton from '../../components/BackButton';

function UserProfile() {
    const { id } = useParams();
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const [jobs, setJobs] = useState([]);
    const [loadingJobs, setLoadingJobs] = useState(false);
    const [jobsError, setJobsError] = useState('');

    const [portfolioItems, setPortfolioItems] = useState([]);
    const [loadingPortfolio, setLoadingPortfolio] = useState(false);
    const [portfolioError, setPortfolioError] = useState('');

    useEffect(() => {
        const fetchUserProfile = async () => {
            try {
                const response = await fetchData(`/profiles/details/${id}`);

                handleApiErrors(response, () => setError('An error occurred while fetching the profile details.'), false, setError);

                setUser(response.data || null);

                // If the user is a founder, fetch their job postings
                if (response.data?.roleName?.toLowerCase() === 'founder') {
                    setLoadingJobs(true);
                    fetchUserJobs(response.data.userId);
                }
                // If the user is an individual, fetch thier portfolio items
                else if (response.data?.roleName?.toLowerCase() === 'individual') {
                    setLoadingPortfolio(true);
                    fetchUserPortfolio(response.data.userId);
                }

            } catch (error) {
                setError('An unexpected error occurred.');
                console.error('Failed to fetch user profile:', error);
            } finally {
                setLoading(false);
            }
        };

        const fetchUserJobs = async (userId) => {
            try {
                const response = await fetchData(`/jobadvertisements/founder-jobs/${userId}`);
                setJobs(response.data || []);
            } catch (error) {
                setJobsError('An unexpected error occurred while fetching job postings.');
                console.error('Failed to fetch job postings:', error);
            } finally {
                setLoadingJobs(false);
            }
        };

        const fetchUserPortfolio = async (userId) => {
            try {
                const response = await fetchData(`/portfolios/individual-portfolio/${userId}`);
                setPortfolioItems(response.data || []);
            } catch (error) {
                setPortfolioError('An unexpected error occurred while fetching portfolio items.');
                console.error('Failed to fetch portfolio items:', error);
            } finally {
                setLoadingPortfolio(false);
            }
        };

        fetchUserProfile();
    }, [id]);

    if (loading || loadingJobs || loadingPortfolio) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="user-profile-section py-5">
            <SetPageTitle title="User Profile" />
            <div className="container">
                <div className="row">
                    <div className="col-12 mb-4 text-center">

                        {user.firstName && user.lastName && (
                            <h1 className="user-name">{`${user.firstName} ${user.lastName}`}</h1>
                        )}

                        {user.roleName && (
                            <span className={`badge ${user.roleName.toLowerCase() === 'founder' ? 'bg-danger' : 'bg-info'}`}>{user.roleName}</span>
                        )}
                    </div>
                    <div className="col-lg-8 col-md-10 mx-auto">
                        <div className="user-details-card">
                            <div className="profile-image-container">
                                <ImageWithFallback
                                    src={user.profilePictureUrl}
                                    alt={`${user.firstName} ${user.lastName}`}
                                    className={`user-profile-img ${user.roleName.toLowerCase() === 'founder' ? 'border-danger' : 'border-info'}`}
                                    fallbackSrc="/images/profile-avatar.png"
                                />
                            </div>
                            <h3 className="section-title mt-4">User Information</h3>

                            {user.email && (
                                <div className="info-group">
                                    <h5 className="info-label">Email</h5>
                                    <p className="info-text">{user.email}</p>
                                </div>
                            )}

                            {user.phoneNumber && (
                                <div className="info-group">
                                    <h5 className="info-label">Phone Number</h5>
                                    <p className="info-text">{user.phoneNumber}</p>
                                </div>
                            )}

                            {user.roleName.toLowerCase() === 'founder' && (
                                <div className="mt-5">
                                    <h3 className="section-title">Job Postings</h3>

                                    {jobsError && <p className="text-danger">{jobsError}</p>}

                                    {jobs.length === 0 ? (
                                        <div className="alert alert-warning">No job postings available.</div>
                                    ) : (
                                        <div className="row">
                                            {jobs.map(job => (
                                                <div className="col-md-6 mb-4" key={job.id}>
                                                    <JobCard job={job} />
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            )}

                            {user.roleName.toLowerCase() === 'individual' && (
                                <div className="mt-5">
                                    <h3 className="section-title">Portfolio</h3>

                                    {portfolioError && <p className="text-danger">{portfolioError}</p>}

                                    {portfolioItems.length === 0 ? (
                                        <div className="alert alert-warning">No portfolio items available.</div>
                                    ) : (
                                        <div className="row">
                                            {portfolioItems.map(item => (
                                                <div className="col-md-6 mb-4" key={item.id}>
                                                    <div className="card portfolio-card">
                                                        <div className="card-body">
                                                            <h5 className="card-title">{item.title}</h5>
                                                            <div className="d-flex align-items-center mb-2">
                                                                <p className="card-text mb-0">{item.description}</p>
                                                            </div>
                                                            <Link to={`/portfolio/${item.id}`} className="btn btn-primary">More Details</Link>
                                                        </div>
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            )}
                        </div>
                    </div>
                    <div className="col-12 text-center mt-5">
                        <BackButton className="btn btn-secondary" />
                    </div>
                </div>
            </div>
        </section>
    );
}

export default UserProfile;
