import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes, useNavigate } from 'react-router-dom';
import JobForm from '../../pages/Jobs/JobForm';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn(),  // Mock useNavigate
}));

describe('JobForm Component', () => {
    const mockNavigate = jest.fn();

    const mockJobData = {
        id: '1',
        jobTitle: 'Software Engineer',
        startupName: 'Tech Startup',
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
        jobLocation: '',
        applicationDeadline: '2023-12-31',
        experience: '2+ years',
        education: 'Bachelor\'s degree',
        requireCV: true,
        requireCoverLetter: true,
    };

    beforeEach(() => {
        jest.clearAllMocks();
        useNavigate.mockReturnValue(mockNavigate);
    });

    test('renders form for creating a new job', async () => {
        const { container } = render(
            <MemoryRouter initialEntries={['/jobs/manage/new']}>
                <Routes>
                    <Route path="/jobs/manage/new" element={<JobForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Check if the title and form fields are rendered
        const formTitle = container.querySelector('h2');
        expect(formTitle).toHaveTextContent('Create Job');

        expect(screen.getByLabelText(/Startup Name/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Job Title/i)).toBeInTheDocument();
    });

    test('renders form for editing an existing job', async () => {
        fetchData.mockResolvedValueOnce({ data: mockJobData });

        render(
            <MemoryRouter initialEntries={['/jobs/manage/edit/1']}>
                <Routes>
                    <Route path="/jobs/manage/edit/:id" element={<JobForm />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByLabelText(/Startup Name/i)).toHaveValue('Tech Startup');
            expect(screen.getByLabelText(/Job Title/i)).toHaveValue('Software Engineer');
            expect(screen.getByLabelText(/Application Deadline/i)).toHaveValue('2023-12-31');
        });
    });

    test('handles job creation successfully', async () => {
        // Mock fetchData to return an empty data object for the creation response
        fetchData.mockResolvedValueOnce({ data: {} });
        handleApiErrors.mockReturnValue(false);

        render(
            <MemoryRouter initialEntries={['/jobs/manage/new']}>
                <Routes>
                    <Route path="/jobs/manage/new" element={<JobForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Fill out all required form inputs with the 'name' attribute included
        fireEvent.change(screen.getByLabelText(/Startup Name/i), {
            target: { name: 'startupName', value: 'New Startup' },
        });
        fireEvent.change(screen.getByLabelText(/Startup Description/i), {
            target: { name: 'startupDescription', value: 'A new startup description' },
        });
        fireEvent.change(screen.getByLabelText(/Startup Stage/i), {
            target: { name: 'startupStage', value: 'Seed' },
        });
        fireEvent.change(screen.getByLabelText(/Industry/i), {
            target: { name: 'industry', value: 'Technology' },
        });
        fireEvent.change(screen.getByLabelText(/Job Title/i), {
            target: { name: 'jobTitle', value: 'Junior Developer' },
        });
        fireEvent.change(screen.getByLabelText(/Job Description/i), {
            target: { name: 'jobDescription', value: 'Responsibilities include coding' },
        });
        fireEvent.change(screen.getByLabelText(/Employment Type/i), {
            target: { name: 'employmentType', value: 'FullTime' },
        });
        fireEvent.change(screen.getByLabelText(/Location Type/i), {
            target: { name: 'jobLocationType', value: 'Remote' },
        });
        fireEvent.change(screen.getByLabelText(/Application Deadline/i), {
            target: { name: 'applicationDeadline', value: '2099-12-31' }, // Future date
        });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /Create Job/i }));

        // // Wait for the fetchData call and ensure the correct URL and data are passed
        await waitFor(() => {
            expect(fetchData).toHaveBeenCalledWith(
                '/jobadvertisements',
                expect.objectContaining({
                    method: 'POST',
                    body: expect.objectContaining({
                        startupName: 'New Startup',
                        startupDescription: 'A new startup description',
                        startupStage: 'Seed',
                        industry: 'Technology',
                        jobTitle: 'Junior Developer',
                        jobDescription: 'Responsibilities include coding',
                        employmentType: 'FullTime',
                        jobLocationType: 'Remote',
                        applicationDeadline: '2099-12-31',
                    }),
                })
            );
        });

        // Ensure navigation occurred
        expect(mockNavigate).toHaveBeenCalledWith('/jobs/manage');
    });

    test('handles job edit successfully', async () => {
        fetchData.mockResolvedValueOnce({ data: mockJobData });
        fetchData.mockResolvedValueOnce({ data: { success: true } });

        render(
            <MemoryRouter initialEntries={['/jobs/manage/edit/1']}>
                <Routes>
                    <Route path="/jobs/manage/edit/:id" element={<JobForm />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            fireEvent.input(screen.getByLabelText(/Startup Name/i), {
                target: { value: 'Updated Startup' },
            });
        });

        fireEvent.click(screen.getByText(/Update Job/i));

        await waitFor(() => {
            expect(fetchData).toHaveBeenCalledWith('/jobadvertisements/1', expect.any(Object));
        });
    });

    test('displays validation errors when required fields are missing', async () => {
        render(
            <MemoryRouter initialEntries={['/jobs/manage/new']}>
                <Routes>
                    <Route path="/jobs/manage/new" element={<JobForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.click(screen.getByDisplayValue(/Create Job/i));

        await waitFor(() => {
            expect(screen.getByText(/Startup name is required/i)).toBeInTheDocument();
            expect(screen.getByText(/Job title is required/i)).toBeInTheDocument();
            expect(screen.getByText(/Application deadline is required/i)).toBeInTheDocument();
        });
    });

    test('handles delete job action', async () => {
        // Mock the confirmation dialog to always return true
        window.confirm = jest.fn(() => true);

        // Mock the API call that handles the deletion
        fetchData.mockResolvedValueOnce({});

        render(
            <MemoryRouter initialEntries={['/jobs/manage/edit/1']}>
                <Routes>
                    <Route path="/jobs/manage/edit/:id" element={<JobForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Click the Delete Goal button
        fireEvent.click(screen.getByRole('button', { name: /Delete Job/i }));

        await waitFor(() => {
            expect(fetchData).toHaveBeenCalledWith('/jobadvertisements/1', {
                method: 'DELETE',
            });
            expect(mockNavigate).toHaveBeenCalledWith(`/jobs/manage`);
        });
    });

    test('displays error message when job fetch fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('API Error'));

        // Mock handleApiErrors to return true, indicating an error occurred
        handleApiErrors.mockReturnValue(true);

        render(
            <MemoryRouter initialEntries={['/jobs/manage/edit/1']}>
                <Routes>
                    <Route path="/jobs/manage/edit/:id" element={<JobForm />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred/i)).toBeInTheDocument();
        });
    });
});
