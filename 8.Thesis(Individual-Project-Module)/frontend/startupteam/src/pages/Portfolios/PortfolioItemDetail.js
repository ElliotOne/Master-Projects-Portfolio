import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SetPageTitle from '../../components/SetPageTitle';
import BackButton from '../../components/BackButton';

function PortfolioItemDetail() {
    const { id } = useParams();
    const [portfolioItem, setPortfolioItem] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchPortfolioItemData = async () => {
            try {
                const response = await fetchData(`/portfolios/details/${id}`);

                handleApiErrors(response, () => setError('An error occurred while fetching data.'), false, setError);

                setPortfolioItem(response.data || {});
            } catch (error) {
                setError('An unexpected error occurred.');
                console.error('Failed to fetch portfolio item details:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchPortfolioItemData();
    }, [id]);

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="portfolio-item-detail-section py-5">
            <SetPageTitle title="Portfolio Item Detail" />
            <div className="container">
                <div className="row">

                    {portfolioItem.title && (
                        <div className="col-12 mb-4 text-center">
                            <h1 className="portfolio-item-title">{portfolioItem.title}</h1>
                        </div>
                    )}

                    <div className="col-lg-8 col-md-10 mx-auto">
                        <div className="portfolio-item-summary">

                            {portfolioItem.description && (
                                <div className="summary-item">
                                    <h5 className="info-label">Description</h5>
                                    <p className="info-text">{portfolioItem.description}</p>
                                </div>
                            )}

                            {portfolioItem.type && (
                                <div className="summary-item">
                                    <h5 className="info-label">Type</h5>
                                    <p className="info-text">{portfolioItem.type}</p>
                                </div>
                            )}

                            {portfolioItem.technologies && (
                                <div className="summary-item">
                                    <h5 className="info-label">Technologies</h5>
                                    <p className="info-text">{portfolioItem.technologies}</p>
                                </div>
                            )}

                            {portfolioItem.skills && (
                                <div className="summary-item">
                                    <h5 className="info-label">Skills</h5>
                                    <p className="info-text">{portfolioItem.skills}</p>
                                </div>
                            )}

                            {portfolioItem.industry && (
                                <div className="summary-item">
                                    <h5 className="info-label">Industry</h5>
                                    <p className="info-text">{portfolioItem.industry}</p>
                                </div>
                            )}

                            {portfolioItem.role && (
                                <div className="summary-item">
                                    <h5 className="info-label">Role</h5>
                                    <p className="info-text">{portfolioItem.role}</p>
                                </div>
                            )}

                            {portfolioItem.duration && (
                                <div className="summary-item">
                                    <h5 className="info-label">Duration</h5>
                                    <p className="info-text">{portfolioItem.duration}</p>
                                </div>
                            )}

                            {portfolioItem.link && (
                                <div className="summary-item">
                                    <h5 className="info-label">Link</h5>
                                    <a href={portfolioItem.link} target="_blank" rel="noopener noreferrer" className="info-link">
                                        {portfolioItem.link}
                                    </a>
                                </div>
                            )}

                            {portfolioItem.attachmentUrl && (
                                <div className="summary-item">
                                    <h5 className="info-label">Attachment</h5>
                                    <a href={portfolioItem.attachmentUrl} target="_blank" rel="noopener noreferrer" className="info-link">
                                        Download Attachment
                                    </a>
                                </div>
                            )}

                            {portfolioItem.tags && (
                                <div className="summary-item">
                                    <h5 className="info-label">Tags</h5>
                                    <p className="info-text">{portfolioItem.tags}</p>
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

export default PortfolioItemDetail;
