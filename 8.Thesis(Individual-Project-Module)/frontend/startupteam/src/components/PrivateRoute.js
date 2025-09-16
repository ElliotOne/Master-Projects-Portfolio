import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const PrivateRoute = ({ element: Component, requiredRole, ...rest }) => {
    const { authState, hasRole, loading } = useAuth();
    const location = useLocation();

    if (!loading) {
        if (!authState.token) {
            return <Navigate to="/signin" state={{ from: location }} />;
        }

        if (requiredRole && !hasRole(requiredRole)) {
            return <Navigate to="/unauthorized" state={{ from: location }} />;
        }

        return <Component {...rest} />;
    }
};

export default PrivateRoute;
