import React from 'react';
import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import { MemoryRouter, Route, Routes, useNavigate } from 'react-router-dom';
import MemberForm from '../../pages/Teams/MemberForm';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn(),  // Mock useNavigate
}));

describe('MemberForm Component', () => {
    const mockNavigate = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
        useNavigate.mockReturnValue(mockNavigate);
    });

    test('renders form for adding a new team member', async () => {
        fetchData.mockResolvedValueOnce({ data: [] }); // Mock no jobs initially

        render(
            <MemoryRouter>
                <Routes>
                    <Route path="*" element={<MemberForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Check if the form elements are rendered
        expect(screen.getByLabelText(/Select Job/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Select Applicant/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Select Role/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /Add Member/i })).toBeInTheDocument();
    });

    test('renders form for editing an existing team member', async () => {
        const mockMemberData = {
            id: '1',
            individualFullName: 'Ali Momenzadeh Kholenjani',
            teamRoleId: '2',
            userId: '123',
        };

        const mockRoles = [
            { id: '1', name: 'Developer' },
            { id: '2', name: 'Designer' },
        ];

        fetchData
            .mockResolvedValueOnce({ data: mockRoles })  // Mock roles
            .mockResolvedValueOnce({ data: mockMemberData });  // Mock member data

        render(
            <MemoryRouter initialEntries={['/teams/team1/members/1/edit']}>
                <Routes>
                    <Route path="/teams/:teamId/members/:id/edit" element={<MemberForm />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByLabelText(/Member Name/i)).toHaveValue('Ali Momenzadeh Kholenjani');
            expect(screen.getByLabelText(/Select Role/i)).toHaveValue('2'); // Pre-filled role
        });

        // Click the update button
        fireEvent.click(screen.getByRole('button', { name: /Update Member/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
        });
    });

    test('displays validation errors when required fields are empty', async () => {
        fetchData.mockResolvedValueOnce({ data: [] });  // Mock no jobs initially

        render(
            <MemoryRouter initialEntries={['/teams/team1/members/new']}>
                <Routes>
                    <Route path="/teams/:teamId/members/new" element={<MemberForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Click the submit button without filling in the form
        fireEvent.click(screen.getByRole('button', { name: /Add Member/i }));

        await waitFor(() => {
            // Check if validation error messages are displayed
            expect(screen.getByText(/Please select a job/i)).toBeInTheDocument();
            expect(screen.getByText(/Please select an applicant/i)).toBeInTheDocument();
            expect(screen.getByText(/Please select a role/i)).toBeInTheDocument();
        });
    });

    test('handles successful form submission and navigates back to the team page', async () => {
        // Mock API responses in sequence
        const mockRoles = [
            { id: 'role1', name: 'Role 1' },
            { id: 'role2', name: 'Role 2' },
        ];

        const mockJobs = [
            { id: 'job1', jobTitle: 'Job 1' },
            { id: 'job2', jobTitle: 'Job 2' },
        ];

        const mockApplicants = [
            {
                userId: 'applicant1',
                individualFullName: 'Ali Momenzadeh Kholenjani',
                individualEmail: 'Ali@example.com',
            },
            {
                userId: 'applicant2',
                individualFullName: 'Jane Momenzadeh Kholenjani',
                individualEmail: 'jane@example.com',
            },
        ];

        fetchData
            .mockResolvedValueOnce({ data: mockRoles }) // 1st call: fetch roles
            .mockResolvedValueOnce({ data: mockJobs }) // 2nd call: fetch jobs
            .mockResolvedValueOnce({ data: mockApplicants }) // 3rd call: fetch applicants
            .mockResolvedValueOnce({ data: { success: true } }); // 4th call: form submission

        handleApiErrors.mockReturnValue(false); // Mock no errors

        // Render the component
        render(
            <MemoryRouter initialEntries={['/teams/team1/members/new']}>
                <Routes>
                    <Route path="/teams/:teamId/members/new" element={<MemberForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Wait for roles and jobs to be loaded
        await waitFor(() => {
            expect(screen.getByLabelText(/Select Job/i)).toBeInTheDocument();
            expect(screen.getByLabelText(/Select Role/i)).toBeInTheDocument();
        });

        // Wait for job options to be rendered
        await waitFor(() => {
            expect(screen.getByRole('option', { name: 'Job 1' })).toBeInTheDocument();
        });

        const jobSelect = screen.getByLabelText(/Select Job/i);

        // Change the job selection
        fireEvent.change(jobSelect, { target: { value: 'job1' } });

        // Wait for fetchApplicants to be called
        await waitFor(() => {
            expect(fetchData).toHaveBeenCalledWith('/jobapplications/job1/successful-applicants');
        });

        // Wait for applicant options to be rendered
        await waitFor(() => {
            expect(screen.getByRole('option', { name: /Ali Momenzadeh Kholenjani/i })).toBeInTheDocument();
        });

        const applicantSelect = screen.getByLabelText(/Select Applicant/i);

        // Change the applicant selection
        fireEvent.change(applicantSelect, { target: { value: 'applicant1' } });

        const roleSelect = screen.getByLabelText(/Select Role/i);

        // Wait for role options to be rendered
        await waitFor(() => {
            expect(screen.getByRole('option', { name: 'Role 1' })).toBeInTheDocument();
        });

        // Change the role selection
        fireEvent.change(roleSelect, { target: { value: 'role1' } });

        // Submit the form
        fireEvent.click(screen.getByRole('button', { name: /Add Member/i }));

        // Wait for the form submission and navigation to be triggered
        await waitFor(() => {
            expect(fetchData).toHaveBeenCalledWith(
                '/teams/team1/members',
                expect.objectContaining({
                    method: 'POST',
                    body: {
                        jobId: 'job1',
                        userId: 'applicant1',
                        teamRoleId: 'role1',
                        teamId: 'team1',
                    },
                })
            );

            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
        });
    });

    test('handles team member deletion', async () => {
        window.confirm = jest.fn(() => true); // Mock confirmation dialog
        fetchData.mockResolvedValueOnce({}); // Mock successful deletion

        render(
            <MemoryRouter initialEntries={['/teams/team1/members/1/edit']}>
                <Routes>
                    <Route path="/teams/:teamId/members/:id/edit" element={<MemberForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.click(screen.getByRole('button', { name: /Delete Team Member/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(`/teams/team1`);
        });
    });

    test('handles API errors gracefully', async () => {
        fetchData.mockRejectedValueOnce(new Error('API Error'));
        handleApiErrors.mockReturnValue(true);

        render(
            <MemoryRouter initialEntries={['/teams/team1/members/new']}>
                <Routes>
                    <Route path="/teams/:teamId/members/new" element={<MemberForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Fill in the form
        fireEvent.change(screen.getByLabelText(/Select Job/i), { target: { value: 'job1' } });
        fireEvent.change(screen.getByLabelText(/Select Applicant/i), { target: { value: 'applicant1' } });
        fireEvent.change(screen.getByLabelText(/Select Role/i), { target: { value: 'role1' } });

        // Click the submit button
        fireEvent.click(screen.getByRole('button', { name: /Add Member/i }));

        // Wait for the error message to be displayed
        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
        });
    });
});
