import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import JobList from '../../pages/Jobs/JobList';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../components/JobCard', () => ({ job }) => (
    <div data-testid="job-card">{job.jobTitle}</div>
));

describe('JobList Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        fetchData.mockResolvedValueOnce({ data: [] }); // Mock empty data

        render(<JobList />);

        // Check if the loading message is displayed initially
        expect(screen.getByText(/Loading/i)).toBeInTheDocument();
    });

    test('renders error message when fetch fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch'));

        render(<JobList />);

        await waitFor(() => {
            // Check if the error message is displayed
            expect(screen.getByText(/Failed to fetch job listings/i)).toBeInTheDocument();
        });
    });

    test('renders "No job postings available" when there are no jobs', async () => {
        fetchData.mockResolvedValueOnce({ data: [] });

        render(<JobList />);

        await waitFor(() => {
            // Check if the "No job postings available" message is displayed
            expect(screen.getByText(/No job postings available/i)).toBeInTheDocument();
        });
    });

    test('renders list of job cards when jobs are available', async () => {
        const mockJobs = [
            { id: '1', jobTitle: 'Software Engineer' },
            { id: '2', jobTitle: 'Project Manager' },
        ];

        fetchData.mockResolvedValueOnce({ data: mockJobs });

        render(<JobList />);

        await waitFor(() => {
            // Check if JobCard components are rendered for each job
            expect(screen.getAllByTestId('job-card')).toHaveLength(2);
            expect(screen.getByText('Software Engineer')).toBeInTheDocument();
            expect(screen.getByText('Project Manager')).toBeInTheDocument();
        });
    });
});
