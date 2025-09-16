import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import JobApplicationForm from '../../pages/JobApplications/JobApplicationForm';
import { fetchData } from '../../utils/fetchData';
import { useParams } from 'react-router-dom';
import { handleApiErrors } from '../../utils/handleApiErrors';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useParams: jest.fn(),
}));

describe('JobApplicationForm Component', () => {
    beforeEach(() => {
        useParams.mockReturnValue({ id: '123' });
        jest.clearAllMocks();
    });

    test('renders the form correctly with default values', async () => {
        fetchData.mockResolvedValueOnce({
            data: {
                id: '123',
                jobAdvertisementId: 'job-1',
                status: 'UnderReview',
                individualFullName: 'Ali Momenzadeh Kholenjani',
                cvUrl: 'http://example.com/cv.pdf',
                coverLetterUrl: 'http://example.com/coverletter.pdf',
                statusText: 'Under Review',
                interviewDate: null
            }
        }).mockResolvedValueOnce({
            data: {
                jobTitle: 'Software Engineer',
                startupName: 'Tech Startup',
                jobLocation: 'Remote',
                employmentType: 'Full-time',
            }
        });

        render(
            <MemoryRouter>
                <JobApplicationForm />
            </MemoryRouter>
        );

        // Wait for data to load
        await waitFor(() => {
            expect(screen.getByText(/Software Engineer/i)).toBeInTheDocument();
            expect(screen.getByText(/Tech Startup/i)).toBeInTheDocument();
        });

        expect(screen.getByText(/View CV/i)).toBeInTheDocument();
        expect(screen.getByText(/View Cover Letter/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/New Application Status/i)).toBeInTheDocument();
    });

    test('displays validation errors when fields are empty', async () => {
        fetchData.mockResolvedValueOnce({
            data: {
                id: '123',
                jobAdvertisementId: 'job-1',
                status: 'InterviewScheduled',
                individualFullName: 'Ali Momenzadeh Kholenjani',
                cvUrl: 'http://example.com/cv.pdf',
                coverLetterUrl: 'http://example.com/coverletter.pdf',
                statusText: 'Interview Scheduled',
                interviewDate: null
            }
        }).mockResolvedValueOnce({
            data: {
                jobTitle: 'Software Engineer',
            }
        });

        render(
            <MemoryRouter>
                <JobApplicationForm />
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByLabelText(/New Application Status/i)).toBeInTheDocument();
        });

        // Change status to InterviewScheduled (so interview date is required)
        fireEvent.change(screen.getByLabelText(/New Application Status/i), {
            target: { value: 'InterviewScheduled' },
        });

        // Submit the form without filling in the required interview date
        fireEvent.click(screen.getByRole('button', { name: /Update Application/i }));

        // Wait for validation errors to appear
        expect(await screen.findByText(/Interview date is required/i)).toBeInTheDocument();
    });

    test('handles successful form submission and displays success message', async () => {
        // Mock initial data fetch
        fetchData
            .mockResolvedValueOnce({
                data: {
                    id: '123',
                    jobAdvertisementId: 'job-1',
                    status: 'InterviewScheduled',
                    individualFullName: 'Ali Momenzadeh Kholenjani',
                    individualUserName: 'Ali_MomenzadehKholenjani',
                    statusText: 'Interview Scheduled',
                    interviewDate: '2024-09-20',
                },
            })
            .mockResolvedValueOnce({
                data: {
                    jobTitle: 'Software Engineer',
                },
            });

        // Mock the PUT request
        fetchData.mockResolvedValueOnce({});

        // Mock window.alert
        global.alert = jest.fn();

        // Store the original window.location
        const originalLocation = window.location;

        // Delete window.location so we can redefine it
        delete window.location;

        // Redefine window.location with a mocked reload function
        window.location = {
            ...originalLocation,
            reload: jest.fn(),
        };

        render(
            <MemoryRouter>
                <JobApplicationForm />
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByLabelText(/New Application Status/i)).toBeInTheDocument();
        });

        // Fill in the form fields
        fireEvent.change(screen.getByLabelText(/New Application Status/i), {
            target: { value: 'InterviewScheduled' },
        });

        fireEvent.change(screen.getByLabelText(/Interview Date/i), {
            target: { value: '2099-09-20' },
        });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /Update Application/i }));

        // Wait for fetchData to be called
        await waitFor(() => {
            expect(fetchData).toHaveBeenCalledWith(
                '/jobapplications/123',
                expect.objectContaining({
                    method: 'PUT',
                    body: expect.objectContaining({
                        status: 'InterviewScheduled',
                        interviewDate: '2099-09-20',
                    }),
                })
            );
        });

        // Check that alert was called with the correct message
        expect(global.alert).toHaveBeenCalledWith('Job application updated successfully!');

        // Check that window.location.reload was called
        expect(window.location.reload).toHaveBeenCalled();

        // Restore mocks
        global.alert.mockRestore();
        window.location = originalLocation;
    });

    test('handles API errors gracefully', async () => {
        fetchData.mockResolvedValueOnce({
            data: {
                id: '123',
                jobAdvertisementId: 'job-1',
                status: 'InterviewScheduled',
                individualFullName: 'Ali Momenzadeh Kholenjani',
                statusText: 'Interview Scheduled',
                interviewDate: '2024-09-20'
            }
        }).mockResolvedValueOnce({
            data: {
                jobTitle: 'Software Engineer',
            }
        });

        fetchData.mockResolvedValueOnce({
            success: false,
            errors: {
                general: ['An unexpected error occurred while updating the job application.'],
            },
        });

        handleApiErrors.mockImplementation((response, setError, getValues, setGeneralError) => {
            setGeneralError('An unexpected error occurred while updating the job application.');
            return true;
          });

        render(
            <MemoryRouter initialEntries={['/jobapplications/123']}>
                <Routes>
                    <Route path="/jobapplications/:id" element={<JobApplicationForm />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByLabelText(/New Application Status/i)).toBeInTheDocument();
        });

        // Fill in the form fields
        fireEvent.change(screen.getByLabelText(/New Application Status/i), {
            target: { value: 'InterviewScheduled' },
        });
        fireEvent.change(screen.getByLabelText(/Interview Date/i), {
            target: { value: '2024-09-20' },
        });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /Update Application/i }));

        // Wait for error message to appear
        await waitFor(() => {
            expect(
                screen.getByText(/An unexpected error occurred while updating the job application\./i)
            ).toBeInTheDocument();
        });
    });
});
