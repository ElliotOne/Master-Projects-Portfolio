import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import ImageWithFallback from '../../components/ImageWithFallback';
import SetPageTitle from '../../components/SetPageTitle';

function UserList() {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await fetchData('/profiles/all-profiles');
                setUsers(response.data || []);
            } catch (error) {
                setError('Failed to fetch user profiles.');
                console.error(error);
            } finally {
                setLoading(false);
            }
        };

        fetchUsers();
    }, []);

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="user-list-section mt-4">
            <SetPageTitle title="Users" />
            <div className="container">
                <div className="row mb-4">
                    <div className="col-12">
                        <div className="d-flex justify-content-between align-items-center">
                            <h1 className="mb-0">All Users</h1>
                        </div>
                    </div>
                </div>
                <div className="row">
                    {users.length === 0 ? (
                        <div className="col-12">
                            <div className="alert alert-warning">
                                No users found.
                            </div>
                        </div>
                    ) : (
                        users.map(user => (
                            <div className="col-md-4 mb-4" key={user.userName}>
                                <div className={`card user-card`}>
                                    <ImageWithFallback
                                        src={user.profilePictureUrl}
                                        className={`card-img-top ${user.roleName.toLowerCase() === 'founder' ? 'border-danger' : 'border-info'}`}
                                        alt={`${user.userName}'s profile`}
                                        fallbackSrc="/images/profile-avatar.png"
                                    />
                                    <div className="card-body">
                                        <h5 className="card-title">{`${user.firstName} ${user.lastName}`}</h5>
                                        <span className={`badge ${user.roleName.toLowerCase() === 'founder' ? 'bg-danger' : 'bg-info'}`}>
                                            {user.roleName}
                                        </span>
                                        <Link to={`${user.userName}`} className={`btn ${user.roleName.toLowerCase() === 'founder' ? 'btn-danger' : 'btn-info'} mt-3`}>View Profile</Link>
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

export default UserList;
