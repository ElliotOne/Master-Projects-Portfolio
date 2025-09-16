import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import SignIn from '../../pages/Auth/SignIn';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import { useNavigate } from 'react-router-dom';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn(),
}));

jest.mock('@react-oauth/google', () => {
    // Import React within the module factory
    const React = require('react');
    return {
        GoogleOAuthProvider: ({ children }) => React.createElement('div', null, children),
        GoogleLogin: ({ onSuccess }) =>
            React.createElement(
                'button',
                { onClick: () => onSuccess({ credential: 'mockGoogleToken' }) },
                'Sign in with Google'
            ),
    };
});

describe('SignIn with Google OAuth', () => {
    const mockNavigate = jest.fn();

    beforeEach(() => {
        useNavigate.mockReturnValue(mockNavigate);
        jest.clearAllMocks(); // Clear any previous mock data
    });

    afterEach(() => {
        jest.resetAllMocks(); // Ensure all mocks are reset after each test
    });

    test('handles Google sign-in successfully and redirects', async () => {
        fetchData.mockResolvedValueOnce({
            data: {
                token: 'mockGoogleToken',
                externalSignIn: true,
            },
        });
        handleApiErrors.mockReturnValue(false); // No errors

        render(
            <MemoryRouter>
                <SignIn />
            </MemoryRouter>
        );

        // Simulate successful Google login by clicking the mocked Google Sign-In button
        const googleLoginButton = screen.getByRole('button', { name: /sign in with google/i });
        fireEvent.click(googleLoginButton);

        // Wait for the navigation after Google login
        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith('/');
        });
        expect(localStorage.getItem('jwtToken')).toBe('mockGoogleToken');
    });

    test('displays error when Google login fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('Google login failed'));
        handleApiErrors.mockReturnValue(true);

        render(
            <MemoryRouter>
                <SignIn />
            </MemoryRouter>
        );

        // Simulate failed Google login
        const googleLoginButton = screen.getByRole('button', { name: /sign in with google/i });
        fireEvent.click(googleLoginButton);

        // Wait for the error message to be displayed
        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });
});
