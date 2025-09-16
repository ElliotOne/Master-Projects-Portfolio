import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import JobsManagement from '../../pages/Jobs/JobsManagement';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('JobsManagement Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <JobsManagement />
            </MemoryRouter>
        );

        // Check if the loading message is displayed
        expect(screen.getByText(/Loading/i)).toBeInTheDocument();
    });

    test('renders error message when fetch fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch'));

        render(
            <MemoryRouter>
                <JobsManagement />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the error message is displayed
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });

    test('renders "No job postings available" when there are no jobs', async () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <JobsManagement />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the "No job postings available" message is displayed
            expect(screen.getByText(/No job postings available/i)).toBeInTheDocument();
        });
    });

    test('renders list of job postings when jobs are available', async () => {
        const mockJobs = [
            {
                id: '1',
                jobTitle: 'Software Engineer',
                startupName: 'Tech Corp',
                jobLocation: 'New York',
                applicationDeadline: '2024-12-01T00:00:00',
                status: 'Active',
            },
            {
                id: '2',
                jobTitle: 'Product Manager',
                startupName: 'Innovate Inc',
                jobLocation: 'San Francisco',
                applicationDeadline: '2024-12-15T00:00:00',
                status: 'Inactive',
            },
        ];

        fetchData.mockResolvedValueOnce({ data: mockJobs });

        render(
            <MemoryRouter>
                <JobsManagement />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if job postings are rendered
            expect(screen.getByText('Software Engineer')).toBeInTheDocument();
            expect(screen.getByText('Product Manager')).toBeInTheDocument();

            // Check if startup names are rendered
            expect(screen.getByText('Tech Corp')).toBeInTheDocument();
            expect(screen.getByText('Innovate Inc')).toBeInTheDocument();

            // Check if job locations are rendered
            expect(screen.getByText('New York')).toBeInTheDocument();
            expect(screen.getByText('San Francisco')).toBeInTheDocument();

            // Check if application deadlines are formatted and rendered
            expect(screen.getByText(/Deadline: 12\/1\/2024/i)).toBeInTheDocument();
            expect(screen.getByText(/Deadline: 12\/15\/2024/i)).toBeInTheDocument();

            // Check if statuses are rendered correctly
            expect(screen.getByText('Active')).toBeInTheDocument();
            expect(screen.getByText('Inactive')).toBeInTheDocument();
        });
    });
});
