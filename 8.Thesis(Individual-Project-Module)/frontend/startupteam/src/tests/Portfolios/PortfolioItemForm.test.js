import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import PortfolioItemForm from '../../pages/Portfolios/PortfolioItemForm';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import { useNavigate } from 'react-router-dom';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');
jest.mock('../../utils/handleApiErrors');
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: jest.fn(),
}));

describe('PortfolioItemForm Component', () => {
    const mockNavigate = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
        useNavigate.mockReturnValue(mockNavigate);
    });

    test('renders form for creating a new portfolio item', () => {
        render(
            <MemoryRouter initialEntries={['/portfolio/manage/new']}>
                <Routes>
                    <Route path="/portfolio/manage/new" element={<PortfolioItemForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Check if the form fields are rendered
        expect(screen.getByLabelText(/Title/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Description/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Technologies/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Skills/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/Attachment/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /Create Portfolio Item/i })).toBeInTheDocument();
    });

    test('renders existing portfolio item for editing', async () => {
        const mockPortfolioItem = {
            title: 'Project Alpha',
            description: 'A description of the project',
            technologies: 'React, Node.js',
            skills: 'JavaScript, CSS',
            industry: 'Tech',
            role: 'Frontend Developer',
            duration: '6 months',
            link: 'https://example.com',
            attachmentUrl: 'https://example.com/attachment.pdf',
            tags: 'Web Development, UI/UX',
        };

        fetchData.mockResolvedValueOnce({ data: mockPortfolioItem });

        render(
            <MemoryRouter initialEntries={['/portfolio/manage/edit/1']}>
                <Routes>
                    <Route path="/portfolio/manage/edit/:id" element={<PortfolioItemForm />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the form fields are pre-filled with existing data
            expect(screen.getByLabelText(/Title/i)).toHaveValue('Project Alpha');
            expect(screen.getByLabelText(/Description/i)).toHaveValue('A description of the project');
            expect(screen.getByLabelText(/Technologies/i)).toHaveValue('React, Node.js');
            expect(screen.getByLabelText(/Skills/i)).toHaveValue('JavaScript, CSS');
            expect(screen.getByLabelText(/Attachment/i)).toBeInTheDocument();
            expect(screen.getByText(/Download current attachment/i)).toBeInTheDocument();
        });
    });

    test('displays validation errors when required fields are missing', async () => {
        render(
            <MemoryRouter initialEntries={['/portfolio/manage/new']}>
                <Routes>
                    <Route path="/portfolio/manage/new" element={<PortfolioItemForm />} />
                </Routes>
            </MemoryRouter>
        );

        fireEvent.click(screen.getByRole('button', { name: /Create Portfolio Item/i }));

        await waitFor(() => {
            // Check for validation error messages
            expect(screen.getByText(/Title is required/i)).toBeInTheDocument();
            expect(screen.getByText(/Description is required/i)).toBeInTheDocument();
        });
    });

    test('handles successful form submission and navigates back to the portfolio management page', async () => {
        fetchData.mockResolvedValueOnce({ data: {} });
        handleApiErrors.mockReturnValue(false); // No errors

        render(
            <MemoryRouter initialEntries={['/portfolio/manage/new']}>
                <Routes>
                    <Route path="/portfolio/manage/new" element={<PortfolioItemForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Fill in the form
        fireEvent.input(screen.getByLabelText(/Title/i), { target: { value: 'Project Beta' } });
        fireEvent.input(screen.getByLabelText(/Description/i), { target: { value: 'A new project description' } });

        fireEvent.click(screen.getByRole('button', { name: /Create Portfolio Item/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith('/portfolio/manage');
        });
    });

    test('handles attachment file upload', async () => {
        const file = new File(['resume'], 'resume.pdf', { type: 'application/pdf' });

        render(
            <MemoryRouter initialEntries={['/portfolio/manage/new']}>
                <Routes>
                    <Route path="/portfolio/manage/new" element={<PortfolioItemForm />} />
                </Routes>
            </MemoryRouter>
        );

        const fileInput = screen.getByLabelText(/Attachment/i);

        // Simulate the file upload by manually creating a FileList
        Object.defineProperty(fileInput, 'files', {
            value: [file],
        });

        fireEvent.change(fileInput);

        expect(fileInput.files[0]).toBe(file);
        expect(fileInput.files).toHaveLength(1);
    });

    test('displays general error message on failure', async () => {
        fetchData.mockRejectedValueOnce(new Error('API call failed'));

        render(
            <MemoryRouter initialEntries={['/portfolio/manage/new']}>
                <Routes>
                    <Route path="/portfolio/manage/new" element={<PortfolioItemForm />} />
                </Routes>
            </MemoryRouter>
        );

        // Fill in the form
        fireEvent.input(screen.getByLabelText(/Title/i), { target: { value: 'Project Beta' } });
        fireEvent.input(screen.getByLabelText(/Description/i), { target: { value: 'A new project description' } });

        fireEvent.click(screen.getByRole('button', { name: /Create Portfolio Item/i }));

        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred/i)).toBeInTheDocument();
        });
    });
});
