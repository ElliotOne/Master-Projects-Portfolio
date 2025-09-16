import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import JobMatches from '../../pages/Matches/JobMatches';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('JobMatches Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <JobMatches />
            </MemoryRouter>
        );

        // Check if the loading message is displayed
        expect(screen.getByText(/Loading/i)).toBeInTheDocument();
    });

    test('renders error message when fetch fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch'));

        render(
            <MemoryRouter>
                <JobMatches />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the error message is displayed
            expect(screen.getByText(/Failed to fetch matched job ads/i)).toBeInTheDocument();
        });
    });

    test('renders "No job postings available" when no jobs are found', async () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <JobMatches />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the "No job postings available" message is displayed
            expect(screen.getByText(/No job postings available/i)).toBeInTheDocument();
        });
    });

    test('renders list of matched jobs when jobs are found', async () => {
        const mockJobs = [
            {
                id: '1',
                jobTitle: 'Frontend Developer',
                startupName: 'TechCorp',
                jobLocation: 'San Francisco',
                applicationDeadline: '2024-08-31T00:00:00',
                score: 95.5,
            },
            {
                id: '2',
                jobTitle: 'Backend Developer',
                startupName: 'Innovatech',
                jobLocation: 'New York',
                applicationDeadline: '2024-09-15T00:00:00',
                score: 89.7,
            },
        ];

        fetchData.mockResolvedValueOnce({ data: mockJobs });

        render(
            <MemoryRouter>
                <JobMatches />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the job titles and details are rendered correctly
            expect(screen.getByText('Frontend Developer')).toBeInTheDocument();
            expect(screen.getByText('Backend Developer')).toBeInTheDocument();
            expect(screen.getByText('TechCorp')).toBeInTheDocument();
            expect(screen.getByText('Innovatech')).toBeInTheDocument();
            expect(screen.getByText('San Francisco')).toBeInTheDocument();
            expect(screen.getByText('New York')).toBeInTheDocument();

            // Check if application deadlines and scores are rendered
            expect(screen.getByText('Deadline: 8/31/2024')).toBeInTheDocument();
            expect(screen.getByText('Deadline: 9/15/2024')).toBeInTheDocument();
            expect(screen.getByText('Score: 95.50')).toBeInTheDocument();
            expect(screen.getByText('Score: 89.70')).toBeInTheDocument();

            // Check if the "More Details" links are rendered
            expect(screen.getAllByRole('link', { name: /More Details/i }).length).toBe(2);
        });
    });
});
