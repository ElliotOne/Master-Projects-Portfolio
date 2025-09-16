import React from 'react';
import { render, screen, waitFor, within } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import TeamDetail from '../../pages/Teams/TeamDetail';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import { useAuth } from '../../context/AuthContext';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('../../context/AuthContext');

describe('TeamDetail Component', () => {
    const mockParams = { id: '1' };

    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders members section correctly', async () => {
        fetchData.mockResolvedValueOnce({
            data: { name: 'Team Alpha', description: 'A team for project Alpha' },
        });
        fetchData.mockResolvedValueOnce({
            data: [
                { id: 1, individualFullName: 'John Doe', teamRoleName: 'Developer', individualUserName: 'john_doe' },
            ], // Members
        });
        fetchData.mockResolvedValueOnce({ data: [] }); // Roles (not needed for this test)
        fetchData.mockResolvedValueOnce({ data: [] }); // Goals (not needed for this test)
        fetchData.mockResolvedValueOnce({ data: [] }); // Milestones (not needed for this test)

        // Mock the useAuth to return a role
        useAuth.mockReturnValue({
            hasRole: jest.fn().mockReturnValue(false),  // Mocking a non-founder role
        });

        const { container } = render(
            <MemoryRouter initialEntries={['/teams/details/1']}>
                <Routes>
                    <Route path="/teams/details/:id" element={<TeamDetail />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for members section to load
        await waitFor(() => {
            const membersSection = container.querySelector('.team-detail-sub-section:nth-of-type(1)');
            expect(within(membersSection).getByText(/John Doe/i)).toBeInTheDocument();
            expect(within(membersSection).getByText(/Developer/i)).toBeInTheDocument();
        });
    });

    test('renders roles section correctly', async () => {
        fetchData.mockResolvedValueOnce({
            data: { name: 'Team Alpha', description: 'A team for project Alpha' }, // Team details
        });
        fetchData.mockResolvedValueOnce({ data: [] }); // Members (not needed for this test)
        fetchData.mockResolvedValueOnce({
            data: [
                { id: 1, name: 'Developer', description: 'Develop software' },
            ], // Roles
        });
        fetchData.mockResolvedValueOnce({ data: [] }); // Goals (not needed for this test)
        fetchData.mockResolvedValueOnce({ data: [] }); // Milestones (not needed for this test)

        // Mock the useAuth to return a role
        useAuth.mockReturnValue({
            hasRole: jest.fn().mockReturnValue(false),  // Mocking a non-founder role
        });

        const { container } = render(
            <MemoryRouter initialEntries={['/teams/details/1']}>
                <Routes>
                    <Route path="/teams/details/:id" element={<TeamDetail />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for roles section to load
        await waitFor(() => {
            const rolesSection = container.querySelector('.team-detail-sub-section:nth-of-type(2)');
            expect(within(rolesSection).getByText(/Developer/i)).toBeInTheDocument();
            expect(within(rolesSection).getByText(/Develop software/i)).toBeInTheDocument();
        });
    });

    test('renders goals section correctly', async () => {
        fetchData.mockResolvedValueOnce({
            data: { name: 'Team Alpha', description: 'A team for project Alpha' }, // Team details
        });
        fetchData.mockResolvedValueOnce({ data: [] }); // Members (not needed for this test)
        fetchData.mockResolvedValueOnce({ data: [] }); // Roles (not needed for this test)
        fetchData.mockResolvedValueOnce({
            data: [
                { id: 1, title: 'Goal 1', description: 'Complete phase 1', dueDate: '2024-12-01', status: 'InProgress' },
            ], // Goals
        });
        fetchData.mockResolvedValueOnce({ data: [] }); // Milestones (not needed for this test)

        // Mock the useAuth to return a role
        useAuth.mockReturnValue({
            hasRole: jest.fn().mockReturnValue(false),  // Mocking a non-founder role
        });

        const { container } = render(
            <MemoryRouter initialEntries={['/teams/details/1']}>
                <Routes>
                    <Route path="/teams/details/:id" element={<TeamDetail />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for goals section to load
        await waitFor(() => {
            const goalsSection = container.querySelector('.team-detail-sub-section:nth-of-type(3)');
            expect(within(goalsSection).getByText(/Goal 1/i)).toBeInTheDocument();
            expect(within(goalsSection).getByText(/Complete phase 1/i)).toBeInTheDocument();
        });
    });

    test('renders milestones section correctly', async () => {
        fetchData.mockResolvedValueOnce({
            data: { name: 'Team Alpha', description: 'A team for project Alpha' }, // Team details
        });
        fetchData.mockResolvedValueOnce({ data: [] }); // Members (not needed for this test)
        fetchData.mockResolvedValueOnce({ data: [] }); // Roles (not needed for this test)
        fetchData.mockResolvedValueOnce({ data: [] }); // Goals (not needed for this test)
        fetchData.mockResolvedValueOnce({
            data: [
                { id: 1, title: 'Milestone 1', description: 'Milestone description', dueDate: '2024-12-10', status: 'NotStarted', goalTitle: 'Goal 1' },
            ], // Milestones
        });

        // Mock the useAuth to return a role
        useAuth.mockReturnValue({
            hasRole: jest.fn().mockReturnValue(false),  // Mocking a non-founder role
        });

        const { container } = render(
            <MemoryRouter initialEntries={['/teams/details/1']}>
                <Routes>
                    <Route path="/teams/details/:id" element={<TeamDetail />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for milestones section to load
        await waitFor(() => {
            const milestonesSection = container.querySelector('.team-detail-sub-section:nth-of-type(4)');
            expect(within(milestonesSection).getByText(/Milestone 1/i)).toBeInTheDocument();
            expect(within(milestonesSection).getByText(/Milestone description/i)).toBeInTheDocument();
        });
    });

    test('displays loading message while data is being fetched', () => {
        // Mock data fetching to simulate a pending promise
        fetchData.mockResolvedValueOnce(new Promise(() => { })); // Mock the pending state

        // Mock the useAuth to return a role
        useAuth.mockReturnValue({
            hasRole: jest.fn().mockReturnValue(false),  // Mocking a non-founder role
        });

        render(
            <MemoryRouter>
                <TeamDetail />
            </MemoryRouter>
        );

        // Expect the loading message to be shown while data is being fetched
        expect(screen.getByText(/Loading.../i)).toBeInTheDocument();
    });

    test('displays error message when data fetching fails', async () => {
        // Mock data fetching error
        fetchData.mockRejectedValueOnce(new Error('API Error'));
        handleApiErrors.mockReturnValue(true);

        // Mock the useAuth to prevent the undefined error
        useAuth.mockReturnValue({
            hasRole: jest.fn().mockReturnValue(false),  // Mocking a non-founder role
        });

        render(
            <MemoryRouter>
                <TeamDetail />
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });

    test('shows edit buttons and links for StartupFounder', async () => {
        // Mock the fetchData to return team details, members, roles, goals, and milestones
        fetchData.mockResolvedValueOnce({
            data: { name: 'Team Alpha', description: 'A team for project Alpha' },
        });
        fetchData.mockResolvedValueOnce({ data: [{ id: 1, individualFullName: 'John Doe', teamRoleName: 'Developer' }] }); // Members
        fetchData.mockResolvedValueOnce({ data: [{ id: 1, name: 'Developer', description: 'Develop software' }] }); // Roles
        fetchData.mockResolvedValueOnce({ data: [{ id: 1, title: 'Goal 1', description: 'Complete phase 1', dueDate: '2024-12-01', status: 'InProgress' }] }); // Goals
        fetchData.mockResolvedValueOnce({ data: [{ id: 1, title: 'Milestone 1', description: 'Milestone description', dueDate: '2024-12-10', status: 'NotStarted', goalTitle: 'Goal 1' }] }); // Milestones
        handleApiErrors.mockReturnValue(false);

        // Mock the useAuth to return the role of 'StartupFounder'
        useAuth.mockReturnValue({
            hasRole: jest.fn().mockReturnValue(true),
        });

        render(
            <MemoryRouter initialEntries={[`/teams/details/${mockParams.id}`]}>
                <Routes>
                    <Route path="/teams/details/:id" element={<TeamDetail />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByText(/Edit Team/i)).toBeInTheDocument();
            expect(screen.getByText(/Add Member/i)).toBeInTheDocument();
            expect(screen.getByText(/Add Role/i)).toBeInTheDocument();
            expect(screen.getByText(/Add Goal/i)).toBeInTheDocument();
            expect(screen.getByText(/Add Milestone/i)).toBeInTheDocument();
        });
    });

    test('hides edit buttons and links for non-StartupFounder roles', async () => {
        // Mock the fetchData to return team details, members, roles, goals, and milestones
        fetchData.mockResolvedValueOnce({
            data: { name: 'Team Alpha', description: 'A team for project Alpha' },
        });
        fetchData.mockResolvedValueOnce({ data: [{ id: 1, individualFullName: 'John Doe', teamRoleName: 'Developer' }] }); // Members
        fetchData.mockResolvedValueOnce({ data: [{ id: 1, name: 'Developer', description: 'Develop software' }] }); // Roles
        fetchData.mockResolvedValueOnce({ data: [{ id: 1, title: 'Goal 1', description: 'Complete phase 1', dueDate: '2024-12-01', status: 'InProgress' }] }); // Goals
        fetchData.mockResolvedValueOnce({ data: [{ id: 1, title: 'Milestone 1', description: 'Milestone description', dueDate: '2024-12-10', status: 'NotStarted', goalTitle: 'Goal 1' }] }); // Milestones
        handleApiErrors.mockReturnValue(false);

        // Mock the useAuth to return a role other than 'StartupFounder'
        useAuth.mockReturnValue({
            hasRole: jest.fn().mockReturnValue(false),
        });

        render(
            <MemoryRouter initialEntries={[`/teams/details/${mockParams.id}`]}>
                <Routes>
                    <Route path="/teams/details/:id" element={<TeamDetail />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.queryByText(/Edit Team/i)).not.toBeInTheDocument();
            expect(screen.queryByText(/Add Member/i)).not.toBeInTheDocument();
            expect(screen.queryByText(/Add Role/i)).not.toBeInTheDocument();
            expect(screen.queryByText(/Add Goal/i)).not.toBeInTheDocument();
            expect(screen.queryByText(/Add Milestone/i)).not.toBeInTheDocument();
        });
    });
});
