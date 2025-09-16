import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import TeamManagement from '../../pages/Teams/TeamManagement';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('TeamManagement Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        // Ensure no teams are fetched yet (loading state)
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <TeamManagement />
            </MemoryRouter>
        );

        // Check that the loading message is displayed
        expect(screen.getByText(/loading/i)).toBeInTheDocument();
    });

    test('renders an error message when fetching teams fails', async () => {
        // Simulate API error
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch teams.'));

        render(
            <MemoryRouter>
                <TeamManagement />
            </MemoryRouter>
        );

        // Wait for the error message to appear
        await waitFor(() => {
            expect(screen.getByText(/an unexpected error occurred/i)).toBeInTheDocument();
        });
    });

    test('displays "No teams available" when no teams are returned', async () => {
        // Simulate empty response from API
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <TeamManagement />
            </MemoryRouter>
        );

        // Wait for the "No teams available" message to appear
        await waitFor(() => {
            expect(screen.getByText(/no teams available/i)).toBeInTheDocument();
        });
    });

    test('renders a list of teams when data is fetched successfully', async () => {
        // Simulate API response with team data
        const mockTeams = [
            { id: '1', name: 'Team Alpha', description: 'First team description' },
            { id: '2', name: 'Team Beta', description: 'Second team description' },
        ];
        fetchData.mockResolvedValueOnce({ data: mockTeams });

        const { container } = render(
            <MemoryRouter>
                <TeamManagement />
            </MemoryRouter>
        );

        // Wait for the teams to be displayed
        await waitFor(() => {
            const teamCards = container.querySelectorAll('.team-card'); // Select team cards

            mockTeams.forEach((team, index) => {
                const teamCard = teamCards[index]; // Target each team card individually
                expect(teamCard).toHaveTextContent(team.name);
                expect(teamCard).toHaveTextContent(team.description);

                // Find the "View Team" link within this specific card
                const viewTeamLink = teamCard.querySelector('a.btn-primary');
                expect(viewTeamLink).toHaveAttribute('href', `/teams/${team.id}`);
            });
        });
    });
});
