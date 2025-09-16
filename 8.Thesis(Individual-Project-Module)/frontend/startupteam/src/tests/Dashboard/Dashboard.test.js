import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Dashboard from '../../pages/Dashboard/Dashboard';
import { useAuth } from '../../context/AuthContext';

// Mock utilities and hooks
jest.mock('../../context/AuthContext');

describe('Dashboard Component', () => {
    const mockAuthState = {
        userfirstName: 'Ali',
        token: 'mockToken',
        role: 'StartupFounder',
    };

    beforeEach(() => {
        // Reset the mock for each test
        jest.clearAllMocks();

        // Default mock for the useAuth hook
        useAuth.mockReturnValue({
            authState: mockAuthState,
            hasRole: jest.fn((role) => mockAuthState.role === role),
        });
    });

    test('renders the Dashboard with the user name', () => {
        render(
            <MemoryRouter>
                <Dashboard />
            </MemoryRouter>
        );

        // Check if the user name is displayed in the dropdown
        expect(screen.getByText(/Hi Ali/i)).toBeInTheDocument();
    });

    test('renders the navigation for StartupFounder role', () => {
        render(
            <MemoryRouter>
                <Dashboard />
            </MemoryRouter>
        );

        // Check if specific links for StartupFounder role are present
        expect(screen.getByText('Job Listings')).toBeInTheDocument();
        expect(screen.getByText('Manage Jobs')).toBeInTheDocument();
        expect(screen.getByText('Job Applications')).toBeInTheDocument();
        expect(screen.getByText('Teams')).toBeInTheDocument();
    });

    test('renders the navigation for SkilledIndividual role', () => {
        // Mock as SkilledIndividual
        useAuth.mockReturnValue({
            authState: { ...mockAuthState, role: 'SkilledIndividual' },
            hasRole: jest.fn((role) => role === 'SkilledIndividual'),
        });

        render(
            <MemoryRouter>
                <Dashboard />
            </MemoryRouter>
        );

        // Check if specific links for SkilledIndividual role are present
        expect(screen.getByText('Job Listings')).toBeInTheDocument();
        expect(screen.getByText('Portfolio')).toBeInTheDocument();
        expect(screen.getByText('Job Matches')).toBeInTheDocument();
    });

    test('renders the Users link for all roles', () => {
        render(
            <MemoryRouter>
                <Dashboard />
            </MemoryRouter>
        );

        // Check if the Users link is always rendered
        expect(screen.getByText('Users')).toBeInTheDocument();
    });

    test('renders the SignOut component', () => {
        render(
            <MemoryRouter>
                <Dashboard />
            </MemoryRouter>
        );

        // Check if SignOut is present in the dropdown
        const signOutButton = screen.getByRole('button', { name: /Hi Ali/i });
        fireEvent.click(signOutButton); // Open the dropdown
        expect(screen.getByText('Sign Out')).toBeInTheDocument();
    });
});
