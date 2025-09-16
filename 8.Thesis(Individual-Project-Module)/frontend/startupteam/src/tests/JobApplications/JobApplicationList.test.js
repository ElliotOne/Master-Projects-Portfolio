import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import JobApplicationList from '../../pages/JobApplications/JobApplicationList';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('JobApplicationList Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading message while fetching data', () => {
        render(
            <MemoryRouter>
                <JobApplicationList />
            </MemoryRouter>
        );

        // Check if loading text is displayed
        expect(screen.getByText(/Loading.../i)).toBeInTheDocument();
    });

    test('renders error message if fetch fails', async () => {
        // Simulate fetch failure
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch job applications'));

        render(
            <MemoryRouter>
                <JobApplicationList />
            </MemoryRouter>
        );

        // Wait for the error message to appear
        await waitFor(() => {
            expect(screen.getByText(/Failed to fetch job applications/i)).toBeInTheDocument();
        });
    });

    test('renders no job applications message when list is empty', async () => {
        // Simulate an empty job application list
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <JobApplicationList />
            </MemoryRouter>
        );

        // Wait for the "No job applications available" message
        await waitFor(() => {
            expect(screen.getByText(/No job applications available/i)).toBeInTheDocument();
        });
    });

    test('renders job applications list', async () => {
        // Simulate successful fetch with job applications data
        const mockApplications = [
            {
                id: 1,
                jobTitle: 'Software Engineer',
                startupName: 'Tech Startup',
                jobLocation: 'Remote',
                applicationDate: '2023-09-01T00:00:00Z',
                status: 'Under Review',
            },
            {
                id: 2,
                jobTitle: 'Product Manager',
                startupName: 'Product Inc.',
                jobLocation: 'Onsite',
                applicationDate: '2023-08-20T00:00:00Z',
                status: 'Interview Scheduled',
            },
        ];
        fetchData.mockResolvedValueOnce({ data: mockApplications });

        render(
            <MemoryRouter>
                <JobApplicationList />
            </MemoryRouter>
        );

        // Wait for the job applications to render
        await waitFor(() => {
            // Check if the first job application is rendered correctly
            expect(screen.getByText('Software Engineer')).toBeInTheDocument();
            expect(screen.getByText(/Tech Startup/i)).toBeInTheDocument();
            expect(screen.getByText(/Remote/i)).toBeInTheDocument();
            expect(screen.getByText(/Applied: 9\/1\/2023/i)).toBeInTheDocument();
            expect(screen.getByText(/Status: Under Review/i)).toBeInTheDocument();

            // Check if the second job application is rendered correctly
            expect(screen.getByText('Product Manager')).toBeInTheDocument();
            expect(screen.getByText(/Product Inc./i)).toBeInTheDocument();
            expect(screen.getByText(/Onsite/i)).toBeInTheDocument();
            expect(screen.getByText(/Applied: 8\/20\/2023/i)).toBeInTheDocument();
            expect(screen.getByText(/Status: Interview Scheduled/i)).toBeInTheDocument();
        });
    });
});
