import React from 'react';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import DashboardOverview from '../../pages/Dashboard/DashboardOverview';
import { useAuth } from '../../context/AuthContext';

// Mock utilities and hooks
jest.mock('../../context/AuthContext', () => ({
    useAuth: jest.fn(),
}));

describe('DashboardOverview Component', () => {
    test('renders basic elements', () => {
        useAuth.mockReturnValue({ hasRole: () => false }); // No specific role

        render(
            <MemoryRouter>
                <DashboardOverview />
            </MemoryRouter>
        );

        // Check for common title and welcome message
        expect(screen.getByText(/Dashboard Overview/i)).toBeInTheDocument();
        expect(screen.getByText(/Welcome! Choose an option below./i)).toBeInTheDocument();

        // Check for the common "View Job Listings" link
        expect(screen.getByText(/View Job Listings/i)).toBeInTheDocument();
    });

    test('renders links for StartupFounder role', () => {
        useAuth.mockReturnValue({ hasRole: (role) => role === 'StartupFounder' });

        render(
            <MemoryRouter>
                <DashboardOverview />
            </MemoryRouter>
        );

        // Check for role-specific links for StartupFounder
        expect(screen.getByText(/View My Job Postings/i)).toBeInTheDocument();
        expect(screen.getByText(/View Job Applications/i)).toBeInTheDocument();
        expect(screen.getByText(/View Teams/i)).toBeInTheDocument();
    });

    test('renders links for SkilledIndividual role', () => {
        useAuth.mockReturnValue({ hasRole: (role) => role === 'SkilledIndividual' });

        render(
            <MemoryRouter>
                <DashboardOverview />
            </MemoryRouter>
        );

        // Check for role-specific links for SkilledIndividual
        expect(screen.getByText(/View Job Applications/i)).toBeInTheDocument();
        expect(screen.getByText(/View Portfolio/i)).toBeInTheDocument();
        expect(screen.getByText(/View Job Matches/i)).toBeInTheDocument();
    });
});
