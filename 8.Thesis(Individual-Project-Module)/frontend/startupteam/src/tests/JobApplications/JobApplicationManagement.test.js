import React from 'react';
import { render, screen, fireEvent, waitFor, within } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import JobApplicationManagement from '../../pages/JobApplications/JobApplicationManagement';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('JobApplicationManagement Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading message while fetching data', () => {
        render(
            <MemoryRouter>
                <JobApplicationManagement />
            </MemoryRouter>
        );

        // Check if loading text is displayed
        expect(screen.getByText(/Loading.../i)).toBeInTheDocument();
    });

    test('renders error message if fetching job advertisements or applications fails', async () => {
        // Simulate fetch failure
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch data'));

        render(
            <MemoryRouter>
                <JobApplicationManagement />
            </MemoryRouter>
        );

        // Wait for the error message to appear
        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });

    test('renders no job applications message when list is empty', async () => {
        // Simulate an empty job application list
        fetchData
            .mockResolvedValueOnce({ data: [] })  // Mock job advertisements
            .mockResolvedValueOnce({ data: [] }); // Mock job applications

        render(
            <MemoryRouter>
                <JobApplicationManagement />
            </MemoryRouter>
        );

        // Wait for the "No job applications available" message
        await waitFor(() => {
            expect(screen.getByText(/No job applications available/i)).toBeInTheDocument();
        });
    });

    test('renders job applications list', async () => {
        // Simulate successful fetch with job applications and job advertisements data
        const mockJobAdvertisements = [
            { id: '1', jobTitle: 'Software Engineer' },
            { id: '2', jobTitle: 'Product Manager' },
        ];

        const mockApplications = [
            {
                id: 1,
                jobTitle: 'Software Engineer',
                startupName: 'Tech Startup',
                jobLocation: 'Remote',
                applicationDate: '2023-09-01T00:00:00Z',
                individualFullName: 'Ali Momenzadeh Kholenjani',
                status: 'Under Review',
            },
            {
                id: 2,
                jobTitle: 'Product Manager',
                startupName: 'Product Inc.',
                jobLocation: 'Onsite',
                applicationDate: '2023-08-20T00:00:00Z',
                individualFullName: 'Jane Smith',
                status: 'Interview Scheduled',
            },
        ];

        fetchData
            .mockResolvedValueOnce({ data: mockJobAdvertisements })  // Mock job advertisements
            .mockResolvedValueOnce({ data: mockApplications });  // Mock job applications

        const { container } = render(
            <MemoryRouter>
                <JobApplicationManagement />
            </MemoryRouter>
        );

        // Wait for the cards to appear
        await waitFor(() => expect(container.querySelectorAll('.job-application-card').length).toBeGreaterThan(0));

        // Get all job application cards using class name
        const jobApplicationCards = container.querySelectorAll('.job-application-card');

        // Check within the first card (Software Engineer)
        const firstApplicationCard = within(jobApplicationCards[0]);
        expect(await firstApplicationCard.findByText('Software Engineer')).toBeInTheDocument();
        expect(await firstApplicationCard.findByText(/Tech Startup/i)).toBeInTheDocument();
        expect(await firstApplicationCard.findByText(/Remote/i)).toBeInTheDocument();
        expect(await firstApplicationCard.findByText(/Ali Momenzadeh Kholenjani/i)).toBeInTheDocument();
        expect(await firstApplicationCard.findByText(/Applied: 9\/1\/2023/i)).toBeInTheDocument();
        expect(await firstApplicationCard.findByText(/Status: Under Review/i)).toBeInTheDocument();

        // Check within the second card (Product Manager)
        const secondApplicationCard = within(jobApplicationCards[1]);
        expect(await secondApplicationCard.findByText('Product Manager')).toBeInTheDocument();
        expect(await secondApplicationCard.findByText(/Product Inc./i)).toBeInTheDocument();
        expect(await secondApplicationCard.findByText(/Onsite/i)).toBeInTheDocument();
        expect(await secondApplicationCard.findByText(/Jane Smith/i)).toBeInTheDocument();
        expect(await secondApplicationCard.findByText(/Applied: 8\/20\/2023/i)).toBeInTheDocument();
        expect(await secondApplicationCard.findByText(/Status: Interview Scheduled/i)).toBeInTheDocument();
    });

    test('filters job applications by selected job advertisement', async () => {
        // Simulate fetch with job advertisements and applications
        const mockJobAdvertisements = [
            { id: '1', jobTitle: 'Software Engineer' },
            { id: '2', jobTitle: 'Product Manager' },
        ];

        const mockApplications = [
            {
                id: 1,
                jobTitle: 'Software Engineer',
                startupName: 'Tech Startup',
                jobLocation: 'Remote',
                applicationDate: '2023-09-01T00:00:00Z',
                individualFullName: 'Ali Momenzadeh Kholenjani',
                status: 'Under Review',
            },
        ];

        fetchData
            .mockResolvedValueOnce({ data: mockJobAdvertisements })  // Mock job advertisements
            .mockResolvedValueOnce({ data: mockApplications });  // Mock filtered applications

        const { container } = render(
            <MemoryRouter>
                <JobApplicationManagement />
            </MemoryRouter>
        );

        // Wait for job advertisements to load
        await waitFor(() => {
            const jobAdDropdown = container.querySelector('select');
            expect(jobAdDropdown).toBeInTheDocument();
        });

        // Select the dropdown box for job advertisements
        const jobSelectBox = container.querySelector('select');

        // Simulate selecting the "Software Engineer" job
        fireEvent.change(jobSelectBox, { target: { value: '1' } });

        // Wait for the applications to be filtered
        await waitFor(() => {
            const applicationCards = container.querySelectorAll('.job-application-card');
            expect(applicationCards.length).toBe(1); // Ensure only one card is displayed after filtering

            // Check within the first filtered card (job application card)
            const firstCard = within(applicationCards[0]);

            // Verify the filtered job application details
            expect(firstCard.getByText('Software Engineer')).toBeInTheDocument();
            expect(firstCard.getByText(/Ali Momenzadeh Kholenjani/i)).toBeInTheDocument();
            expect(firstCard.getByText(/Tech Startup/i)).toBeInTheDocument();
            expect(firstCard.getByText(/Remote/i)).toBeInTheDocument();
        });
    });
});
