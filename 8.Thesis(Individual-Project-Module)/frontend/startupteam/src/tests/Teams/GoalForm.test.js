import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes, useNavigate } from 'react-router-dom';
import GoalForm from '../../pages/Teams/GoalForm';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn(),  // Mock useNavigate
}));

describe('GoalForm Component', () => {
    const mockNavigate = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
        useNavigate.mockReturnValue(mockNavigate);
    });

    test('renders Create Goal form by default', () => {
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<GoalForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Check if the form elements are rendered
        expect(screen.getByLabelText(/Title/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Description/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Due Date/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Status/i)).toBeInTheDocument();
    });

    test('displays validation errors when fields are empty', async () => {
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<GoalForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Click the submit button without filling in fields
        fireEvent.click(screen.getByRole('button', { name: /Create Goal/i }));

        await waitFor(() => {
            expect(screen.getByText(/Title is required/i)).toBeInTheDocument();
            expect(screen.getByText(/Description is required/i)).toBeInTheDocument();
            expect(screen.getByText(/Due date is required/i)).toBeInTheDocument();
        });
    });

    test('handles successful goal submission', async () => {
        fetchData.mockResolvedValueOnce({ data: {} });
        handleApiErrors.mockReturnValue(false);

        render(
            <MemoryRouter initialEntries={['/teams/team1/goals/new']}>
                <Routes>
                    <Route path="/teams/:teamId/goals/new" element={<GoalForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.input(screen.getByLabelText(/Title/i), {
            target: { value: 'Complete UI Redesign' },
        });
        fireEvent.input(screen.getByLabelText(/Description/i), {
            target: { value: 'Redesign the user interface for better usability' },
        });
        fireEvent.input(screen.getByLabelText(/Due Date/i), {
            target: { value: '2024-12-01' },
        });
        fireEvent.change(screen.getByLabelText(/Status/i), {
            target: { value: 'InProgress' },
        });

        fireEvent.click(screen.getByRole('button', { name: /Create Goal/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
        });
    });

    test('handles goal update when editing an existing goal', async () => {
        fetchData.mockResolvedValueOnce({
            data: {
                id: '1',
                title: 'Update API',
                description: 'Update API to support new features',
                dueDate: '2025-10-15',
                status: 'InProgress',
            },
        });

        handleApiErrors.mockReturnValue(false);

        // Simulate the route for editing an existing goal with teamId and goalId
        render(
            <MemoryRouter initialEntries={['/teams/team1/goals/1/edit']}>
                <Routes>
                    <Route path="/teams/:teamId/goals/:id/edit" element={<GoalForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the form to be populated with the existing goal data
        await waitFor(() => {
            expect(screen.getByLabelText(/Title/i)).toHaveValue('Update API');
            expect(screen.getByLabelText(/Description/i)).toHaveValue('Update API to support new features');
            expect(screen.getByLabelText(/Due Date/i)).toHaveValue('2025-10-15');
            expect(screen.getByLabelText(/Status/i)).toHaveValue('InProgress');
        });

        // Simulate form submission by clicking the "Update Goal" button
        fireEvent.click(screen.getByRole('button', { name: /Update Goal/i }));

        // Check if navigation occurred after successful submission
        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
        });
    });

    test('handles goal deletion', async () => {
        // Mock the confirmation dialog to always return true
        window.confirm = jest.fn(() => true);

        // Mock the API call that handles the deletion
        fetchData.mockResolvedValueOnce({});

        // Simulate editing an existing goal with teamId and goalId
        render(
            <MemoryRouter initialEntries={['/teams/team1/goals/1/edit']}>
                <Routes>
                    <Route path="/teams/:teamId/goals/:id/edit" element={<GoalForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Click the Delete Goal button
        fireEvent.click(screen.getByRole('button', { name: /Delete Goal/i }));

        // Check if the navigation occurred after deletion
        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
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
                    <Route path="*" element={<GoalForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.input(screen.getByLabelText(/Title/i), {
            target: { value: 'Complete UI Redesign' },
        });
        fireEvent.input(screen.getByLabelText(/Description/i), {
            target: { value: 'Redesign the user interface for better usability' },
        });
        fireEvent.input(screen.getByLabelText(/Due Date/i), {
            target: { value: '2024-12-01' },
        });
        fireEvent.change(screen.getByLabelText(/Status/i), {
            target: { value: 'InProgress' },
        });

        // Click the submit button (assumed to trigger form submission)
        fireEvent.click(screen.getByRole('button', { name: /Create Goal/i }));

        // Wait for the error message to be displayed
        await waitFor(() => {
            // Check if the error message is rendered
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });
});
