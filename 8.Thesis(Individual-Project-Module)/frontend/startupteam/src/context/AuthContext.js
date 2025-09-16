import React, { createContext, useContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [authState, setAuthState] = useState({ token: null, role: '', userFirstName: '' });
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem('jwtToken');
        if (token) {
            try {
                const decodedToken = jwtDecode(token);
                const role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
                const userfirstName = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname'] || '';
                setAuthState({ token, role, userfirstName });
            } catch (error) {
                console.error('Invalid token', error);
            }
        }

        setLoading(false);
    }, []);

    const hasRole = (roleToCheck) => authState.role === roleToCheck;

    return (
        <AuthContext.Provider value={{ authState, hasRole, loading }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
