import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import ProfileForm from '../../pages/Users/ProfileForm';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');

describe('ProfileForm Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders the profile form with existing user data', async () => {
        // Mock the fetchData response with user profile data
        fetchData.mockResolvedValueOnce({
            data: {
                userName: 'testuser',
                email: 'test@example.com',
                firstName: 'Ali',
                lastName: 'Momenzadeh Kholenjani',
                phoneNumber: '1234567890',
                roleName: 'SkilledIndividual',
                profilePictureUrl: '/images/profile-avatar.png',
            },
        });

        render(
            <MemoryRouter>
                <ProfileForm />
            </MemoryRouter>
        );

        // Check if the form fields are displayed with the correct default values
        await waitFor(() => {
            expect(screen.getByLabelText(/first name/i)).toHaveValue('Ali');
            expect(screen.getByLabelText(/last name/i)).toHaveValue('Momenzadeh Kholenjani');
            expect(screen.getByLabelText(/username/i)).toHaveValue('testuser');
            expect(screen.getByLabelText(/email/i)).toHaveValue('test@example.com');
            expect(screen.getByLabelText(/phone number/i)).toHaveValue('1234567890');
            expect(screen.getByLabelText(/role/i)).toHaveValue('SkilledIndividual');
        });
    });

    test('handles profile picture upload', async () => {
        fetchData.mockResolvedValueOnce({
            data: {
                userName: 'testuser',
                email: 'test@example.com',
                firstName: 'Ali',
                lastName: 'Momenzadeh Kholenjani',
                profilePictureUrl: '/images/profile-avatar.png',
            },
        });

        const { container } = render(
            <MemoryRouter>
                <ProfileForm />
            </MemoryRouter>
        );

        const file = new File(['profile-pic'], 'profile-pic.png', { type: 'image/png' });

        // Use container.querySelector to select the input by its id
        const input = container.querySelector('#file');

        // Simulate file upload
        fireEvent.change(input, { target: { files: [file] } });

        // Verify that the image preview is updated
        await waitFor(() => {
            const img = screen.getByAltText(/profile/i);
            // Check that the src contains the base64 data URL prefix for PNG images
            expect(img.src).toContain('data:image/png;base64');
        });
    });

    test('displays validation errors when required fields are missing', async () => {
        render(
            <MemoryRouter>
                <ProfileForm />
            </MemoryRouter>
        );

        // Clear the first name field
        fireEvent.change(screen.getByLabelText(/first name/i), { target: { value: '' } });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /update profile/i }));

        // Check for validation errors
        await waitFor(() => {
            expect(screen.getByText(/first name is required/i)).toBeInTheDocument();
            expect(screen.getByText(/last name is required/i)).toBeInTheDocument();
        });
    });

    test('submits the profile form successfully', async () => {
        fetchData.mockResolvedValueOnce({
            data: {
                userName: 'testuser',
                firstName: 'Ali',
                lastName: 'Momenzadeh Kholenjani',
                profilePictureUrl: '/images/profile-avatar.png',
            },
        });
        handleApiErrors.mockReturnValue(false); // No errors

        // Mock the window.alert function
        window.alert = jest.fn();

        render(
            <MemoryRouter>
                <ProfileForm />
            </MemoryRouter>
        );

        // Fill in the form fields
        fireEvent.change(screen.getByLabelText(/first name/i), { target: { value: 'Ali' } });
        fireEvent.change(screen.getByLabelText(/last name/i), { target: { value: 'Momenzadeh Kholenjani' } });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /update profile/i }));

        // Check if alert was called with the success message
        await waitFor(() => {
            expect(window.alert).toHaveBeenCalledWith('Profile updated successfully.');
        });
    });

    test('displays general error message on API failure', async () => {
        // Simulate an API failure
        fetchData.mockRejectedValueOnce(new Error('Failed to update profile'));

        render(
            <MemoryRouter>
                <ProfileForm />
            </MemoryRouter>
        );

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /update profile/i }));

        // Wait for the error message
        await waitFor(() => {
            expect(screen.getByText(/an unexpected error occurred/i)).toBeInTheDocument();
        });
    });
});
