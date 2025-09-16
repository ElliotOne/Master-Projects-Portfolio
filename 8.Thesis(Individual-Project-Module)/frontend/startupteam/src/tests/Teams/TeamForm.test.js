import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes, useNavigate } from 'react-router-dom';
import TeamForm from '../../pages/Teams/TeamForm';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn(),  // Mock useNavigate
}));

describe('TeamForm Component', () => {
    const mockNavigate = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
        useNavigate.mockReturnValue(mockNavigate);
    });

    test('renders the Create Team form by default', () => {
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<TeamForm />} />
                </Routes>
            </MemoryRouter>
        );

        expect(screen.getByLabelText(/Team Name/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Description/i)).toBeInTheDocument();
    });

    test('renders the Edit Team form when team ID is provided', async () => {
        // Mock fetchData to return existing team data
        fetchData.mockResolvedValueOnce({
            data: {
                name: 'Team Alpha',
                description: 'A team dedicated to building core platform.',
                userId: 1,
            },
        });
        handleApiErrors.mockReturnValue(false);

        render(
            <MemoryRouter initialEntries={[`/teams/manage/edit/1s`]}>
                <Routes>
                    <Route path="/teams/manage/edit/:id" element={<TeamForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the data to be loaded into the form
        await waitFor(() => {
            expect(screen.getByDisplayValue(/Team Alpha/i)).toBeInTheDocument();
            expect(screen.getByDisplayValue(/A team dedicated to building core platform./i)).toBeInTheDocument();
        });
    });

    test('submits form data to create a new team', async () => {
        fetchData.mockResolvedValueOnce({ data: {} });
        handleApiErrors.mockReturnValue(false);

        render(
            <MemoryRouter initialEntries={['/teams/manage/new']}>
                <Routes>
                    <Route path="/teams/manage/new" element={<TeamForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.input(screen.getByLabelText(/Team Name/i), {
            target: { value: 'Complete UI Redesign' },
        });

        fireEvent.input(screen.getByLabelText(/Description/i), {
            target: { value: 'A team dedicated to building core platform.' },
        });

        fireEvent.click(screen.getByRole('button', { name: /Create Team/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/manage`);
        });
    });

    test('submits form data to update an existing team', async () => {
        fetchData.mockResolvedValueOnce({
            data: {
                name: 'Team Alpha',
                description: 'A team dedicated to building core platform.',
                userId: 1,
            },
        });
        handleApiErrors.mockReturnValue(false);

        render(
            <MemoryRouter initialEntries={[`/teams/manage/edit/1`]}>
                <Routes>
                    <Route path="/teams/manage/edit/:id" element={<TeamForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the data to be loaded into the form
        await waitFor(() => {
            expect(screen.getByDisplayValue(/Team Alpha/i)).toBeInTheDocument();
        });

        fireEvent.input(screen.getByLabelText(/Team Name/i), { target: { value: 'Team Alpha Updated' } });
        fireEvent.input(screen.getByLabelText(/Description/i), { target: { value: 'Updated description.' } });

        fireEvent.click(screen.getByRole('button', { name: /Update Team/i }));

        await waitFor(() => {
            expect(fetchData).toHaveBeenCalledWith(`/teams/1`, expect.objectContaining({
                method: 'PUT',
                body: {
                    name: 'Team Alpha Updated',
                    description: 'Updated description.',
                    userId: 1,
                },
            }));
        });
    });

    test('displays validation errors when fields are empty', async () => {
        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<TeamForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.click(screen.getByRole('button', { name: /Create Team/i }));

        await waitFor(() => {
            expect(screen.getByText(/Team name is required/i)).toBeInTheDocument();
            expect(screen.getByText(/Description is required/i)).toBeInTheDocument();
        });
    });

    test('handles team deletion', async () => {
        window.confirm = jest.fn(() => true); // Simulate the confirm dialog
        fetchData.mockResolvedValueOnce({ data: {} });

        render(
            <MemoryRouter initialEntries={['/teams/manage/edit/1']}>
                <Routes>
                    <Route path="/teams/manage/edit/:id" element={<TeamForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.click(screen.getByRole('button', { name: /Delete Team/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/manage`);
        });
    });

    test('displays error message on submission failure', async () => {
        // Mock fetchData to reject with an error
        fetchData.mockRejectedValueOnce(new Error('API Error'));

        // Mock handleApiErrors to return true, indicating an error occurred
        handleApiErrors.mockReturnValue(true);

        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<TeamForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.input(screen.getByLabelText(/Team Name/i), {
            target: { value: 'Complete UI Redesign' },
        });

        fireEvent.input(screen.getByLabelText(/Description/i), {
            target: { value: 'A team dedicated to building core platform.' },
        });

        fireEvent.click(screen.getByRole('button', { name: /Create Team/i }));

        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });
});
