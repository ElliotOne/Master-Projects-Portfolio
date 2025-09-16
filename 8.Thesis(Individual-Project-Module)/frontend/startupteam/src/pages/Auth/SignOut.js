import React from 'react';
import { useNavigate } from 'react-router-dom';

function SignOut() {
    const navigate = useNavigate();

    const handleSignOut = async () => {
        try {
            localStorage.removeItem('jwtToken');
            navigate('/signin');
        } catch (error) {
            console.error('Sign out failed:', error);
        }
    };

    return (
        <li>
            <a
                className="dropdown-item"
                href="#"
                onClick={(e) => {
                    e.preventDefault();
                    handleSignOut();
                }}
            >
                <span className="material-symbols-outlined me-2">logout</span>
                Sign Out
            </a>
        </li>
    );
};

export default SignOut;
