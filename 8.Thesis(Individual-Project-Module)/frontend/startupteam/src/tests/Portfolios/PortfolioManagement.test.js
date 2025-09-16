import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import PortfolioManagement from '../../pages/Portfolios/PortfolioManagement';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');

describe('PortfolioManagement Component', () => {
    beforeEach(() => {
        jest.clearAllMocks(); // Reset mocks before each test
    });

    test('renders loading message while fetching portfolio items', () => {
        render(
            <MemoryRouter>
                <PortfolioManagement />
            </MemoryRouter>
        );

        // Check if the loading message is displayed
        expect(screen.getByText(/Loading/i)).toBeInTheDocument();
    });

    test('renders error message when an error occurs', async () => {
        fetchData.mockRejectedValueOnce(new Error('API call failed'));

        render(
            <MemoryRouter>
                <PortfolioManagement />
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred/i)).toBeInTheDocument();
        });
    });

    test('renders empty state when no portfolio items are available', async () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <PortfolioManagement />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the "No portfolio items available" message is shown
            expect(screen.getByText(/No portfolio items available/i)).toBeInTheDocument();
        });
    });

    test('renders portfolio items when available', async () => {
        const mockPortfolioItems = [
            { id: 1, title: 'Project Alpha', description: 'A great project' },
            { id: 2, title: 'Project Beta', description: 'An amazing project' },
        ];

        fetchData.mockResolvedValueOnce({ data: mockPortfolioItems });

        render(
            <MemoryRouter>
                <PortfolioManagement />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if portfolio items are displayed
            expect(screen.getByText('Project Alpha')).toBeInTheDocument();
            expect(screen.getByText('A great project')).toBeInTheDocument();
            expect(screen.getByText('Project Beta')).toBeInTheDocument();
            expect(screen.getByText('An amazing project')).toBeInTheDocument();
        });
    });

    test('renders "Add New Item" button', async () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <PortfolioManagement />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the "Add New Item" link is rendered
            expect(screen.getByRole('link', { name: /Add New Item/i })).toBeInTheDocument();
        });
    });
});
