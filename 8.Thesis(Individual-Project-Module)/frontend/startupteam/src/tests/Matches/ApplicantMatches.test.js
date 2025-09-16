import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, useSearchParams } from 'react-router-dom';
import ApplicantMatches from '../../pages/Matches/ApplicantMatches';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useSearchParams: jest.fn(),
}));

describe('ApplicantMatches Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        useSearchParams.mockReturnValue([new URLSearchParams('?jobAdId=1')]);
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <ApplicantMatches />
            </MemoryRouter>
        );

        // Check if the loading message is displayed
        expect(screen.getByText(/Loading/i)).toBeInTheDocument();
    });

    test('renders error message when fetch fails', async () => {
        useSearchParams.mockReturnValue([new URLSearchParams('?jobAdId=1')]);
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch'));

        render(
            <MemoryRouter>
                <ApplicantMatches />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the error message is displayed
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });

    test('renders "No matched applicants available" when no applicants are found', async () => {
        useSearchParams.mockReturnValue([new URLSearchParams('?jobAdId=1')]);
        fetchData.mockResolvedValueOnce({ data: [] });

        render(
            <MemoryRouter>
                <ApplicantMatches />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the "No matched applicants available" message is displayed
            expect(screen.getByText(/No matched applicants available/i)).toBeInTheDocument();
        });
    });

    test('renders list of matched applicants when applicants are found', async () => {
        const mockApplicants = [
            {
                id: '1',
                jobTitle: 'Frontend Developer',
                startupName: 'TechCorp',
                jobLocation: 'San Francisco',
                individualFullName: 'Ali Momenzadeh Kholenjani',
                applicationDate: '2024-08-15T00:00:00',
                status: 'Shortlisted',
                score: 85.75,
            },
            {
                id: '2',
                jobTitle: 'Backend Developer',
                startupName: 'InnoTech',
                jobLocation: 'New York',
                individualFullName: 'Jane Smith',
                applicationDate: '2024-08-10T00:00:00',
                status: 'UnderReview',
                score: 90.50,
            },
        ];

        useSearchParams.mockReturnValue([new URLSearchParams('?jobAdId=1')]);
        fetchData.mockResolvedValueOnce({ data: mockApplicants });

        render(
            <MemoryRouter>
                <ApplicantMatches />
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if applicant names and job titles are rendered
            expect(screen.getByText('Frontend Developer')).toBeInTheDocument();
            expect(screen.getByText('Backend Developer')).toBeInTheDocument();
            expect(screen.getByText('Ali Momenzadeh Kholenjani')).toBeInTheDocument();
            expect(screen.getByText('Jane Smith')).toBeInTheDocument();

            // Check if locations and application dates are rendered
            expect(screen.getByText('San Francisco')).toBeInTheDocument();
            expect(screen.getByText('New York')).toBeInTheDocument();
            expect(screen.getByText('Applied: 8/15/2024')).toBeInTheDocument();
            expect(screen.getByText('Applied: 8/10/2024')).toBeInTheDocument();

            // Check if statuses and scores are rendered using custom matcher for "Shortlisted" and "UnderReview"
            expect(screen.getByText((content, element) => content.includes('Shortlisted'))).toBeInTheDocument();
            expect(screen.getByText((content, element) => content.includes('UnderReview'))).toBeInTheDocument();
            expect(screen.getByText((content, element) => content.includes('Score: 85.75'))).toBeInTheDocument();
            expect(screen.getByText((content, element) => content.includes('Score: 90.50'))).toBeInTheDocument();
        });
    });
});
