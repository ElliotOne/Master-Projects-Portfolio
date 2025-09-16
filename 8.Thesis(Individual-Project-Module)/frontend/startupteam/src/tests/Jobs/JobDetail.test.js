import React from 'react';
import { render, screen, fireEvent, waitFor, within } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import JobDetail from '../../pages/Jobs/JobDetail';
import { fetchData } from '../../utils/fetchData';
import { useAuth } from '../../context/AuthContext';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../context/AuthContext');

describe('JobDetail Component', () => {
    const mockJobData = {
        id: '1',
        jobTitle: 'Software Engineer',
        startupName: 'Tech Startup',
        jobLocation: 'Remote',
        employmentType: 'Full-time',
        startupDescription: 'A fast-growing startup',
        startupStage: 'Seed',
        industry: 'Technology',
        keyTechnologies: 'React, Node.js',
        uniqueSellingPoints: 'Great work culture',
        missionStatement: 'To innovate and deliver quality software',
        foundingYear: '2020',
        teamSize: '10-50',
        startupWebsite: 'https://techstartup.com',
        startupValues: 'Innovation, Integrity',
        jobDescription: 'Develop and maintain web applications',
        requiredSkills: 'React, Node.js, JavaScript',
        jobResponsibilities: 'Write clean code, collaborate with the team',
        salaryRange: '$80k - $120k',
        jobLocationType: 'Remote',
        applicationDeadline: '2023-12-31',
        experience: '2+ years',
        education: 'Bachelor\'s degree',
        requireCV: true,
        requireCoverLetter: true,
        hasApplied: false,
    };

    const mockAuthContext = {
        authState: { user: { role: 'SkilledIndividual' } },
        hasRole: jest.fn((role) => role === 'SkilledIndividual'),
    };

    beforeEach(() => {
        jest.clearAllMocks();
        useAuth.mockReturnValue(mockAuthContext);
    });

    test('renders loading state initially', () => {
        render(
            <MemoryRouter initialEntries={['/jobadvertisements/details/1']}>
                <Routes>
                    <Route path="/jobadvertisements/details/:id" element={<JobDetail />} />
                </Routes>
            </MemoryRouter>
        );

        expect(screen.getByText(/Loading.../i)).toBeInTheDocument();
    });

    test('renders job details after data fetch', async () => {
        fetchData.mockResolvedValueOnce({ data: mockJobData });

        render(
            <MemoryRouter initialEntries={['/jobadvertisements/details/1']}>
                <Routes>
                    <Route path="/jobadvertisements/details/:id" element={<JobDetail />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check for the job title in the heading
            expect(screen.getByRole('heading', { name: 'Software Engineer' })).toBeInTheDocument();

            // Check for the startup name
            expect(screen.getByText('Tech Startup')).toBeInTheDocument();
        });

        // Scope to the job summary section
        const jobSummarySection = screen.getByText('Tech Startup').closest('.job-summary');
        expect(jobSummarySection).toBeInTheDocument();

        // Within the job summary, find the job location
        const jobLocationItem = within(jobSummarySection).getByText((content, element) => {
            return element.tagName.toLowerCase() === 'p' && content === 'Remote';
        });
        expect(jobLocationItem).toBeInTheDocument();

        // Similarly, find the employment type
        const employmentTypeItem = within(jobSummarySection).getByText((content, element) => {
            return element.tagName.toLowerCase() === 'p' && content === 'Full-time';
        });
        expect(employmentTypeItem).toBeInTheDocument();

        // Check for the startup description in the startup information section
        const startupDescriptionGroup = screen.getByText('Startup Description').closest('.info-group');
        expect(startupDescriptionGroup).toBeInTheDocument();
        expect(within(startupDescriptionGroup).getByText('A fast-growing startup')).toBeInTheDocument();
    });

    test('handles job application submission with CV and cover letter', async () => {
        // Mock the auth context to return true for 'SkilledIndividual'
        const mockAuthContext = {
            authState: { user: { role: 'SkilledIndividual' } },
            hasRole: jest.fn((role) => role === 'SkilledIndividual'),
        };
        useAuth.mockReturnValue(mockAuthContext);

        // Mock API responses
        fetchData
            .mockResolvedValueOnce({ data: mockJobData })  // For fetching job details
            .mockResolvedValueOnce({ data: { success: true } }); // For form submission

        // Mock window.alert and window.location.reload
        window.alert = jest.fn();
        Object.defineProperty(window, 'location', {
            value: { reload: jest.fn() },
            writable: true,
        });

        // Render the component inside a router
        render(
            <MemoryRouter initialEntries={['/jobadvertisements/details/1']}>
                <Routes>
                    <Route path="/jobadvertisements/details/:id" element={<JobDetail />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the "Apply Now" button to be rendered
        await waitFor(() => {
            expect(screen.getByRole('button', { name: /Apply Now/i })).toBeInTheDocument();
        });

        // Click the "Apply Now" button
        fireEvent.click(screen.getByRole('button', { name: /Apply Now/i }));

        // Wait for the form fields to be rendered
        await waitFor(() => {
            expect(screen.getByLabelText(/CV/i)).toBeInTheDocument();
            expect(screen.getByLabelText(/Cover Letter/i)).toBeInTheDocument();
        });

        // Simulate file uploads
        const cvInput = screen.getByLabelText(/CV/i);
        const coverLetterInput = screen.getByLabelText(/Cover Letter/i);

        fireEvent.change(cvInput, {
            target: {
                files: [new File(['cv content'], 'cv.pdf', { type: 'application/pdf' })],
            },
        });

        fireEvent.change(coverLetterInput, {
            target: {
                files: [new File(['cover letter content'], 'cover-letter.pdf', { type: 'application/pdf' })],
            },
        });

        // Submit the application
        fireEvent.click(screen.getByRole('button', { name: /Submit Application/i }));

        // Wait for the form submission to be processed
        await waitFor(() => {
            // Verify fetchData was called for form submission
            expect(fetchData).toHaveBeenCalledWith(
                '/jobapplications',
                expect.objectContaining({
                    method: 'POST',
                    body: expect.any(Object),
                    isMultiPart: true,
                })
            );

            // Ensure window.alert and window.location.reload were called
            expect(window.alert).toHaveBeenCalledWith('Application submitted successfully!');
            expect(window.location.reload).toHaveBeenCalled();
        });
    });

    test('shows error message when application submission fails', async () => {
        // Mock the auth context to return true for 'SkilledIndividual'
        const mockAuthContext = {
            authState: { user: { role: 'SkilledIndividual' } },
            hasRole: jest.fn((role) => role === 'SkilledIndividual'),
        };
        useAuth.mockReturnValue(mockAuthContext);

        // Mock API responses
        fetchData
            .mockResolvedValueOnce({ data: mockJobData }) // For fetching job details
            .mockRejectedValueOnce(new Error('Failed to submit')); // For form submission

        // Render the component inside a router
        render(
            <MemoryRouter initialEntries={['/jobadvertisements/details/1']}>
                <Routes>
                    <Route path="/jobadvertisements/details/:id" element={<JobDetail />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for the "Apply Now" button to be rendered
        await waitFor(() => {
            expect(screen.getByRole('button', { name: /Apply Now/i })).toBeInTheDocument();
        });

        // Click the "Apply Now" button
        fireEvent.click(screen.getByRole('button', { name: /Apply Now/i }));

        // Wait for the form fields to be rendered
        await waitFor(() => {
            expect(screen.getByLabelText(/CV/i)).toBeInTheDocument();
            expect(screen.getByLabelText(/Cover Letter/i)).toBeInTheDocument();
        });

        // Simulate file uploads
        const cvInput = screen.getByLabelText(/CV/i);
        const coverLetterInput = screen.getByLabelText(/Cover Letter/i);

        fireEvent.change(cvInput, {
            target: {
                files: [new File(['cv content'], 'cv.pdf', { type: 'application/pdf' })],
            },
        });
        fireEvent.change(coverLetterInput, {
            target: {
                files: [new File(['cover letter content'], 'cover-letter.pdf', { type: 'application/pdf' })],
            },
        });

        // Submit the application
        fireEvent.click(screen.getByRole('button', { name: /Submit Application/i }));

        // Wait for the error message to appear
        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred/i)).toBeInTheDocument();
        });

        // Verify that fetchData was called for submission
        expect(fetchData).toHaveBeenCalledWith('/jobapplications', expect.any(Object));
    });

    test('renders "You have already applied" message if the user has applied for the job', async () => {
        // Mock the auth context to return true for 'SkilledIndividual'
        const mockAuthContext = {
            authState: { user: { role: 'SkilledIndividual' } },
            hasRole: jest.fn((role) => role === 'SkilledIndividual'),
        };
        useAuth.mockReturnValue(mockAuthContext);

        fetchData.mockResolvedValueOnce({ data: { ...mockJobData, hasApplied: true } });

        render(
            <MemoryRouter initialEntries={['/jobadvertisements/details/1']}>
                <Routes>
                    <Route path="/jobadvertisements/details/:id" element={<JobDetail />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByText(/You have already applied for this job/i)).toBeInTheDocument();
        });
    });

    test('renders error message when job details fetch fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch job'));

        render(
            <MemoryRouter initialEntries={['/jobadvertisements/details/1']}>
                <Routes>
                    <Route path="/jobadvertisements/details/:id" element={<JobDetail />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred/i)).toBeInTheDocument();
        });
    });
});
