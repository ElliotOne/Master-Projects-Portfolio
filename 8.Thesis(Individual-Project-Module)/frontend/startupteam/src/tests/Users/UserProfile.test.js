import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import UserProfile from '../../pages/Users/UserProfile';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('UserProfile Component', () => {
    const mockUserData = {
        userId: 'user123',
        firstName: 'Ali',
        lastName: 'Momenzadeh Kholenjani',
        email: 'test@example.com',
        phoneNumber: '123-456-7890',
        roleName: 'Founder',
        profilePictureUrl: '/images/profile.jpg',
    };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        fetchData.mockResolvedValueOnce({ data: mockUserData });

        render(
            <MemoryRouter initialEntries={['/users/user123']}>
                <Routes>
                    <Route path="/users/:id" element={<UserProfile />} />
                </Routes>
            </MemoryRouter>
        );

        // Check for the loading text
        expect(screen.getByText(/loading/i)).toBeInTheDocument();
    });

    test('renders user profile details for a founder', async () => {
        fetchData.mockResolvedValueOnce({ data: mockUserData });

        render(
            <MemoryRouter initialEntries={['/users/user123']}>
                <Routes>
                    <Route path="/users/:id" element={<UserProfile />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the user details to appear
        await waitFor(() => {
            expect(screen.getByText('Ali Momenzadeh Kholenjani')).toBeInTheDocument();
            expect(screen.getByText('Founder')).toBeInTheDocument();
            expect(screen.getByText('test@example.com')).toBeInTheDocument();
            expect(screen.getByText('123-456-7890')).toBeInTheDocument();
        });

        // Check that the profile picture is rendered
        expect(screen.getByAltText('Ali Momenzadeh Kholenjani')).toHaveAttribute('src', '/images/profile.jpg');
    });

    test('renders jobs for a founder', async () => {
        const mockJobs = [
            {
                id: 'job1',
                jobTitle: 'Senior Developer',
                startupName: 'Tech Startup',
                jobLocation: 'Remote',
            },
        ];

        fetchData.mockResolvedValueOnce({ data: mockUserData }); // Mock user data fetch
        fetchData.mockResolvedValueOnce({ data: mockJobs }); // Mock jobs fetch

        render(
            <MemoryRouter initialEntries={['/users/user123']}>
                <Routes>
                    <Route path="/users/:id" element={<UserProfile />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the job listings to appear
        await waitFor(() => {
            expect(screen.getByText('Senior Developer')).toBeInTheDocument();
            expect(screen.getByText('Tech Startup')).toBeInTheDocument();
            expect(screen.getByText('Remote')).toBeInTheDocument();
        });
    });

    test('renders portfolio items for an individual', async () => {
        const mockIndividualData = {
            ...mockUserData,
            roleName: 'Individual',
        };

        const mockPortfolioItems = [
            {
                id: 'item1',
                title: 'Project Alpha',
                description: 'A web development project',
            },
        ];

        fetchData.mockResolvedValueOnce({ data: mockIndividualData }); // Mock user data fetch
        fetchData.mockResolvedValueOnce({ data: mockPortfolioItems }); // Mock portfolio fetch

        render(
            <MemoryRouter initialEntries={['/users/user123']}>
                <Routes>
                    <Route path="/users/:id" element={<UserProfile />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the portfolio items to appear
        await waitFor(() => {
            expect(screen.getByText('Project Alpha')).toBeInTheDocument();
            expect(screen.getByText('A web development project')).toBeInTheDocument();
        });
    });

    test('renders error message if user profile fetch fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch user profile'));

        render(
            <MemoryRouter initialEntries={['/users/user123']}>
                <Routes>
                    <Route path="/users/:id" element={<UserProfile />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the error message to appear
        await waitFor(() => {
            expect(screen.getByText(/an unexpected error occurred/i)).toBeInTheDocument();
        });
    });

    test('renders error message if job fetch fails for a founder', async () => {
        fetchData.mockResolvedValueOnce({ data: mockUserData }); // Mock user data fetch
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch jobs'));

        render(
            <MemoryRouter initialEntries={['/users/user123']}>
                <Routes>
                    <Route path="/users/:id" element={<UserProfile />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the jobs error message to appear
        await waitFor(() => {
            expect(screen.getByText(/an unexpected error occurred while fetching job postings/i)).toBeInTheDocument();
        });
    });

    test('renders error message if portfolio fetch fails for an individual', async () => {
        const mockIndividualData = {
            ...mockUserData,
            roleName: 'Individual',
        };

        fetchData.mockResolvedValueOnce({ data: mockIndividualData }); // Mock user data fetch
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch portfolio items'));

        render(
            <MemoryRouter initialEntries={['/users/user123']}>
                <Routes>
                    <Route path="/users/:id" element={<UserProfile />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the portfolio error message to appear
        await waitFor(() => {
            expect(screen.getByText(/an unexpected error occurred while fetching portfolio items/i)).toBeInTheDocument();
        });
    });
});
