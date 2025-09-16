import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import CompleteSignUp from '../../pages/Auth/CompleteSignUp';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import { useLocation, useNavigate } from 'react-router-dom';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn(),
    useLocation: jest.fn(),
}));

describe('CompleteSignUp Component', () => {
    const mockNavigate = jest.fn();

    beforeEach(() => {
        // Mock the location and navigate hooks
        useLocation.mockReturnValue({
            state: {
                email: 'test@example.com',
                firstName: 'Ali',
                lastName: 'Momenzadeh Kholenjani',
                externalSignIn: false,
            },
        });
        useNavigate.mockReturnValue(mockNavigate);
        jest.clearAllMocks(); // Clear any previous mock data
    });

    test('renders CompleteSignUp form with default values', () => {
        render(
            <MemoryRouter>
                <CompleteSignUp />
            </MemoryRouter>
        );

        // Check if form fields are present with default values
        expect(screen.getByLabelText(/Username/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/First Name/i)).toHaveValue('Ali');
        expect(screen.getByLabelText(/Last Name/i)).toHaveValue('Momenzadeh Kholenjani');
        expect(screen.getByLabelText(/Role/i)).toBeInTheDocument();
    });

    test('should display validation errors when fields are empty', async () => {
        useLocation.mockReturnValue({
            state: {
                email: 'test@example.com',  // Keep email, but reset firstName and lastName to empty
                firstName: '',
                lastName: '',
                externalSignIn: false,
            },
        });

        render(
            <MemoryRouter>
                <CompleteSignUp />
            </MemoryRouter>
        );

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /complete sign up/i }));

        expect(await screen.findByText(/Username is required/i)).toBeInTheDocument();
        expect(await screen.findByText(/First name is required/i)).toBeInTheDocument();
        expect(await screen.findByText(/Last name is required/i)).toBeInTheDocument();
        expect(await screen.findByText(/Role is required/i)).toBeInTheDocument();
    });

    test('handles successful form submission and redirects', async () => {
        fetchData.mockResolvedValueOnce({
            data: { token: 'mockToken' },
        });
        handleApiErrors.mockReturnValue(false); // No errors

        render(
            <MemoryRouter>
                <CompleteSignUp />
            </MemoryRouter>
        );

        // Fill in the form
        fireEvent.input(screen.getByLabelText(/Username/i), {
            target: { value: 'testuser' },
        });
        fireEvent.input(screen.getByLabelText(/First Name/i), {
            target: { value: 'Ali' },
        });
        fireEvent.input(screen.getByLabelText(/Last Name/i), {
            target: { value: 'Momenzadeh Kholenjani' },
        });
        fireEvent.change(screen.getByLabelText(/Role/i), {
            target: { value: 'StartupFounder' },
        });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /complete sign up/i }));

        // Wait for the navigation after successful submission
        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith('/signup-success', {
                state: { externalSignIn: false, token: 'mockToken' },
            });
        });
    });

    test('handles API errors gracefully', async () => {
        // Mock the API call to fail
        fetchData.mockRejectedValueOnce(new Error('API Error'));

        render(
            <MemoryRouter>
                <CompleteSignUp />
            </MemoryRouter>
        );

        // Fill in the form fields
        fireEvent.input(screen.getByLabelText(/Username/i), {
            target: { value: 'testuser' },
        });
        fireEvent.input(screen.getByLabelText(/First Name/i), {
            target: { value: 'Ali' },
        });
        fireEvent.input(screen.getByLabelText(/Last Name/i), {
            target: { value: 'Momenzadeh Kholenjani' },
        });
        fireEvent.change(screen.getByLabelText(/Role/i), {
            target: { value: 'SkilledIndividual' },
        });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /complete sign up/i }));

        // Wait for the error message to be displayed
        await waitFor(() => {
            expect(screen.getByText('An unexpected error occurred.')).toBeInTheDocument();
        });
    });
});
