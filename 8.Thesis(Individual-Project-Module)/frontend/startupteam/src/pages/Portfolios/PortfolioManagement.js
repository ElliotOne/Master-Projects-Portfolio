import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import SetPageTitle from '../../components/SetPageTitle';

function PortfolioManagement() {
    const [portfolioItems, setPortfolioItems] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchPortfolioItems = async () => {
            try {
                const response = await fetchData('/portfolios');
                handleApiErrors(response, () => setError('An error occurred while fetching data.'), false, setError);
                setPortfolioItems(response.data || []);
            } catch (error) {
                setError('An unexpected error occurred.');
                console.error('Failed to fetch portfolio items:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchPortfolioItems();
    }, []);

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="portfolio-management-section mt-4">
            <SetPageTitle title="My Portfolio" />
            <div className="container">
                <div className="row mb-4">
                    <div className="col-12">
                        <div className="d-flex justify-content-between align-items-center">
                            <h1 className="mb-0">My Portfolio</h1>
                            <Link to="new" className="btn btn-success">
                                <span className="material-symbols-outlined">
                                    add
                                </span>
                                Add New Item
                            </Link>
                        </div>
                    </div>
                </div>
                <div className="row">
                    {portfolioItems.length === 0 ? (
                        <div className="col-12">
                            <div className="alert alert-warning">
                                No portfolio items available.
                            </div>
                        </div>
                    ) : (
                        portfolioItems.map(item => (
                            <div className="col-md-4 mb-4" key={item.id}>
                                <div className="card portfolio-card">
                                    <div className="card-body">
                                        <h5 className="card-title">{item.title}</h5>
                                        <div className="d-flex align-items-center mb-2">
                                            <p className="card-text mb-0">{item.description}</p>
                                        </div>
                                        <Link to={`edit/${item.id}`} className="btn btn-secondary">Edit</Link>
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

export default PortfolioManagement;
