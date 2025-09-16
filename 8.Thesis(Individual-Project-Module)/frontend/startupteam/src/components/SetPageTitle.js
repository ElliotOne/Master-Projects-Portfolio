import React from 'react';

const SetPageTitle = ({ title }) => {
    React.useEffect(() => {
        document.title = 'StartupTeam | ' + title;
    }, [title]);

    return null;
};

export default SetPageTitle;