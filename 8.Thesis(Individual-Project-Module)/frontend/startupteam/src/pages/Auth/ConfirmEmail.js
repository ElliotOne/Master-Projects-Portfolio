import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import SetPageTitle from '../../components/SetPageTitle';

const ConfirmEmail = () => {
    const { search } = useLocation();
    const navigate = useNavigate();
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const queryParams = new URLSearchParams(search);
        const token = queryParams.get('token');

        if (token) {
            localStorage.setItem('jwtToken', token);
            setLoading(false);
            navigate('/');
            navigate(0); //Force re-render to ensure correct authorization
        } else {
            setError('No token provided');
            setLoading(false);
        }
    }, [search, navigate]);

    return (
        <div>
            <SetPageTitle title="Confirm Email" />
            {loading ? (
                <p>Loading...</p>
            ) : error ? (
                <p>{error}</p>
            ) : (
                <p>Email confirmed successfully! Redirecting...</p>
            )}
        </div>
    );
};

export default ConfirmEmail;
