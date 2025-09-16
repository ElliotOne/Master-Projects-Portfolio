import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import TeamList from '../../pages/Teams/TeamList';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('TeamList Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <TeamList />
            </MemoryRouter>
        );

        // Check that loading message is displayed
        expect(screen.getByText(/loading/i)).toBeInTheDocument();
    });

    test('renders an error message when fetching teams fails', async () => {
        // Simulate an API failure
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch teams.'));

        render(
            <MemoryRouter>
                <TeamList />
            </MemoryRouter>
        );

        // Wait for the error message to be displayed
        await waitFor(() => {
            expect(screen.getByText(/failed to fetch teams/i)).toBeInTheDocument();
        });
    });

    test('displays "No teams available" when no teams are returned', async () => {
        // Simulate an empty response
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <TeamList />
            </MemoryRouter>
        );

        // Wait for the "No teams available" message to appear
        await waitFor(() => {
            expect(screen.getByText(/no teams available/i)).toBeInTheDocument();
        });
    });

    test('renders a list of teams when data is fetched successfully', async () => {
        // Simulate a successful API call with some team data
        const mockTeams = [
            { id: '1', name: 'Team Alpha', description: 'First team description' },
            { id: '2', name: 'Team Beta', description: 'Second team description' },
        ];
        fetchData.mockResolvedValueOnce({ data: mockTeams });

        const { container } = render(
            <MemoryRouter>
                <TeamList />
            </MemoryRouter>
        );

        // Wait for the teams to be displayed
        await waitFor(() => {
            const teamCards = container.querySelectorAll('.team-card'); // Select all team cards

            mockTeams.forEach((team, index) => {
                const teamCard = teamCards[index];
                expect(teamCard).toHaveTextContent(team.name);
                expect(teamCard).toHaveTextContent(team.description);

                // Find the "View Team" link within the specific card
                const viewTeamLink = teamCard.querySelector('a.btn-primary');
                expect(viewTeamLink).toHaveAttribute('href', `/teams/${team.id}`);
            });
        });
    });
});
