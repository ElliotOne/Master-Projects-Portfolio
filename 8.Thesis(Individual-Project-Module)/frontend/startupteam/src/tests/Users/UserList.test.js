import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import UserList from '../../pages/Users/UserList';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('UserList Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <UserList />
            </MemoryRouter>
        );

        // Check for the loading text
        expect(screen.getByText(/loading/i)).toBeInTheDocument();
    });

    test('renders an error message when fetch fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch user profiles'));

        render(
            <MemoryRouter>
                <UserList />
            </MemoryRouter>
        );

        // Wait for the error message to appear
        await waitFor(() => {
            expect(screen.getByText(/failed to fetch user profiles/i)).toBeInTheDocument();
        });
    });

    test('renders a "no users found" message when there are no users', async () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <UserList />
            </MemoryRouter>
        );

        // Wait for the "No users found" message to appear
        await waitFor(() => {
            expect(screen.getByText(/no users found/i)).toBeInTheDocument();
        });
    });

    test('renders users correctly with their profile picture and role-specific styling', async () => {
        const mockUsers = [
            {
                userName: 'founder123',
                firstName: 'Ali',
                lastName: 'Momenzadeh Kholenjani',
                profilePictureUrl: '/images/ali.jpg',
                roleName: 'Founder',
            },
            {
                userName: 'skilled_individual',
                firstName: 'Bob',
                lastName: 'Smith',
                profilePictureUrl: '/images/bob.jpg',
                roleName: 'SkilledIndividual',
            },
        ];

        fetchData.mockResolvedValueOnce({ data: mockUsers });

        render(
            <MemoryRouter>
                <UserList />
            </MemoryRouter>
        );

        // Check that the correct user names are rendered
        await waitFor(() => {
            expect(screen.getByText('Ali Momenzadeh Kholenjani')).toBeInTheDocument();
            expect(screen.getByText('Bob Smith')).toBeInTheDocument();
        });

        // Check that the profile picture is rendered correctly
        expect(screen.getByAltText("founder123's profile")).toHaveAttribute('src', '/images/ali.jpg');
        expect(screen.getByAltText("skilled_individual's profile")).toHaveAttribute('src', '/images/bob.jpg');

        // Check for role-specific styling (Founder should have a 'btn-danger' and 'border-danger', SkilledIndividual should have 'btn-info')
        expect(screen.getByText('View Profile', { selector: 'a.btn-danger' })).toBeInTheDocument();
        expect(screen.getByText('View Profile', { selector: 'a.btn-info' })).toBeInTheDocument();

        // Check that the correct role badge is applied
        expect(screen.getByText('Founder')).toHaveClass('bg-danger');
        expect(screen.getByText('SkilledIndividual')).toHaveClass('bg-info');
    });

    test('renders the fallback profile picture if no profilePictureUrl is provided', async () => {
        const mockUsers = [
            {
                userName: 'user_without_image',
                firstName: 'Charlie',
                lastName: 'Brown',
                profilePictureUrl: '',
                roleName: 'SkilledIndividual',
            },
        ];

        fetchData.mockResolvedValueOnce({ data: mockUsers });

        render(
            <MemoryRouter>
                <UserList />
            </MemoryRouter>
        );

        // Wait for the fallback profile picture to be rendered
        const img = await screen.findByAltText("user_without_image's profile");

        // Ensure the fallback src is used
        await waitFor(() => {
            expect(img).toHaveAttribute('src', '/images/profile-avatar.png');
        });
    });
});
