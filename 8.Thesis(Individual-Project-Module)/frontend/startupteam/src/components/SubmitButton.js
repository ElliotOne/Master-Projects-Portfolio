import React from 'react';

const SubmitButton = ({ text }) => {
    return (
        <input type="submit" value={text} className="btn btn-success" />
    );
};

export default SubmitButton;
