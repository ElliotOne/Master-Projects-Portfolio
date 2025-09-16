import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes, useNavigate } from 'react-router-dom';
import MilestoneForm from '../../pages/Teams/MilestoneForm';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn(),  // Mock useNavigate
}));

describe('MilestoneForm Component', () => {
    const mockNavigate = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
        useNavigate.mockReturnValue(mockNavigate);
    });

    test('renders Create Milestone form by default', () => {
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<MilestoneForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Check if form elements are rendered
        expect(screen.getByLabelText(/Title/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Description/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Due Date/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Status/i)).toBeInTheDocument();
    });

    test('displays validation errors when fields are empty', async () => {
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<MilestoneForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Click the submit button without filling in fields
        fireEvent.click(screen.getByRole('button', { name: /Create Milestone/i }));

        await waitFor(() => {
            expect(screen.getByText(/Title is required/i)).toBeInTheDocument();
            expect(screen.getByText(/Description is required/i)).toBeInTheDocument();
            expect(screen.getByText(/Due date is required/i)).toBeInTheDocument();
        });
    });

    test('handles successful milestone submission', async () => {
        fetchData.mockResolvedValueOnce({ data: {} });
        handleApiErrors.mockReturnValue(false);

        render(
            <MemoryRouter initialEntries={['/teams/team1/milestones/new']}>
                <Routes>
                    <Route path="/teams/:teamId/milestones/new" element={<MilestoneForm />} />
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

        fireEvent.click(screen.getByRole('button', { name: /Create Milestone/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
        });
    });

    test('handles milestone update when editing an existing milestone', async () => {
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

        render(
            <MemoryRouter initialEntries={['/teams/team1/milestones/1/edit']}>
                <Routes>
                    <Route path="/teams/:teamId/milestones/:id/edit" element={<MilestoneForm />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByLabelText(/Title/i)).toHaveValue('Update API');
            expect(screen.getByLabelText(/Description/i)).toHaveValue('Update API to support new features');
            expect(screen.getByLabelText(/Due Date/i)).toHaveValue('2025-10-15');
            expect(screen.getByLabelText(/Status/i)).toHaveValue('InProgress');
        });

        fireEvent.click(screen.getByRole('button', { name: /Update Milestone/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
        });
    });

    test('handles milestone deletion', async () => {
        window.confirm = jest.fn(() => true); // Mock the confirmation dialog
        fetchData.mockResolvedValueOnce({});

        render(
            <MemoryRouter initialEntries={['/teams/team1/milestones/1/edit']}>
                <Routes>
                    <Route path="/teams/:teamId/milestones/:id/edit" element={<MilestoneForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.click(screen.getByRole('button', { name: /Delete Milestone/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
        });
    });

    test('handles API errors gracefully', async () => {
        fetchData.mockRejectedValueOnce(new Error('API Error'));
        handleApiErrors.mockReturnValue(true);

        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<MilestoneForm />} />
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

        fireEvent.click(screen.getByRole('button', { name: /Create Milestone/i }));

        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });
});
