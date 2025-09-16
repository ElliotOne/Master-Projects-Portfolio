import React from 'react';
import { useNavigate } from 'react-router-dom';

const BackButton = ({ text = 'Back', className = 'btn btn-secondary' }) => {
    const navigate = useNavigate();

    return (
        <button
            type="button"
            className={className}
            onClick={() => navigate(-1)}
        >
            {text}
        </button>
    );
};

export default BackButton;
