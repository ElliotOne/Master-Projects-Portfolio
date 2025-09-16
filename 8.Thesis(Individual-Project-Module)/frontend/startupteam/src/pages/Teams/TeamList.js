import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import SetPageTitle from '../../components/SetPageTitle';

function TeamList() {
    const [teams, setTeams] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchTeams = async () => {
            try {
                const response = await fetchData('/teams/individual-teams');
                setTeams(response.data || []);
            } catch (error) {
                setError('Failed to fetch teams.');
                console.error(error);
            } finally {
                setLoading(false);
            }
        };

        fetchTeams();
    }, []);

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="team-list-section mt-4">
            <SetPageTitle title="Teams" />
            <div className="container">
                <div className="row mb-4">
                    <div className="col-12">
                        <div className="d-flex justify-content-between align-items-center">
                            <h1 className="mb-0">Teams</h1>
                        </div>
                    </div>
                </div>
                <div className="row">
                    {teams.length === 0 ? (
                        <div className="col-12">
                            <div className="alert alert-warning">
                                No teams available.
                            </div>
                        </div>
                    ) : (
                        teams.map(team => (
                            <div className="col-md-4 mb-4" key={team.id}>
                                <div className="card team-card">
                                    <div className="card-body">
                                        <h5 className="card-title">{team.name}</h5>
                                        <div className="d-flex align-items-center mb-2">
                                            <p className="card-text mb-3">{team.description}</p>
                                        </div>
                                        <Link to={`/teams/${team.id}`} className="btn btn-primary">View Team</Link>
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

export default TeamList;
