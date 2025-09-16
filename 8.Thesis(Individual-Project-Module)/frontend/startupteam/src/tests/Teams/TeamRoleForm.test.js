import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes, useNavigate } from 'react-router-dom';
import TeamRoleForm from '../../pages/Teams/TeamRoleForm';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn()
}));

describe('TeamRoleForm Component', () => {
    const mockNavigate = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
        useNavigate.mockReturnValue(mockNavigate);
    });

    test('renders Create Team Role form by default', () => {
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<TeamRoleForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Check if the form elements are rendered
        expect(screen.getByLabelText(/Role Name/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Description/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /Create Role/i })).toBeInTheDocument();
    });

    test('renders Edit Team Role form by default', async () => {
        // Mock the fetchData response for editing a role
        fetchData.mockResolvedValueOnce({
            data: {
                id: '1',
                name: 'Developer',
                description: 'Develop software applications',
            },
        });

        render(
            <MemoryRouter initialEntries={['/teams/team1/roles/1/edit']}>
                <Routes>
                    <Route path="/teams/:teamId/roles/:id/edit" element={<TeamRoleForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the form to be populated with fetched data
        await waitFor(() => {
            expect(screen.getByLabelText(/Role Name/i)).toHaveValue('Developer');
            expect(screen.getByLabelText(/Description/i)).toHaveValue('Develop software applications');
            expect(screen.getByRole('button', { name: /Update Role/i })).toBeInTheDocument();
        });
    });

    test('displays validation errors when required fields are missing', async () => {
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<TeamRoleForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Submit the form without filling in any fields
        fireEvent.click(screen.getByRole('button', { name: /Create Role/i }));

        // Wait for the validation errors to be displayed
        await waitFor(() => {
            expect(screen.getByText(/role name is required/i)).toBeInTheDocument();
            expect(screen.getByText(/description is required/i)).toBeInTheDocument();
        });
    });

    test('handles successful team role submission', async () => {
        fetchData.mockResolvedValueOnce({ data: {} });
        handleApiErrors.mockReturnValue(false); // No errors

        render(
            <MemoryRouter initialEntries={['/teams/team1/roles/new']}>
                <Routes>
                    <Route path="/teams/:teamId/roles/new" element={<TeamRoleForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Fill in the form fields
        fireEvent.input(screen.getByLabelText(/Role Name/i), {
            target: { value: 'Developer' },
        });
        fireEvent.input(screen.getByLabelText(/Description/i), {
            target: { value: 'Develop software applications' },
        });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /create role/i }));

        // Wait for the form submission and navigation
        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith('/teams/team1');
        });
    });

    test('handles team role deletion', async () => {
        // Mock the confirmation dialog to always return true
        window.confirm = jest.fn(() => true);

        // Mock the API call that handles the deletion
        fetchData.mockResolvedValueOnce({});

        // Simulate editing an existing goal with teamId and goalId
        render(
            <MemoryRouter initialEntries={['/teams/team1/roles/1/edit']}>
                <Routes>
                    <Route path="/teams/:teamId/roles/:id/edit" element={<TeamRoleForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Click the delete button
        fireEvent.click(screen.getByRole('button', { name: /Delete Role/i }));

        // Wait for the deletion and navigation
        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith('/teams/team1');
        });
    });

    test('handles API errors gracefully', async () => {
        // Mock fetchData to reject with an error
        fetchData.mockRejectedValueOnce(new Error('API Error'));

        // Mock handleApiErrors to return true, indicating an error occurred
        handleApiErrors.mockReturnValue(true);

        // Render the GoalForm component
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<TeamRoleForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.input(screen.getByLabelText(/Role Name/i), {
            target: { value: 'Developer' },
        });
        fireEvent.input(screen.getByLabelText(/Description/i), {
            target: { value: 'Develop software applications' },
        });

        // Click the submit button (assumed to trigger form submission)
        fireEvent.click(screen.getByRole('button', { name: /Create Role/i }));

        // Wait for the error message to be displayed
        await waitFor(() => {
            // Check if the error message is rendered
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });
});
