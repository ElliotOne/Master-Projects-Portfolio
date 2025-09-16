import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import PortfolioItemDetail from '../../pages/Portfolios/PortfolioItemDetail';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('PortfolioItemDetail Component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders loading state initially', () => {
        fetchData.mockResolvedValueOnce({ data: {} });

        render(
            <MemoryRouter initialEntries={['/portfolios/details/1']}>
                <Routes>
                    <Route path="/portfolios/details/:id" element={<PortfolioItemDetail />} />
                </Routes>
            </MemoryRouter>
        );

        expect(screen.getByText(/Loading/i)).toBeInTheDocument();
    });

    test('renders error message when fetch fails', async () => {
        fetchData.mockRejectedValueOnce(new Error('Failed to fetch portfolio item'));

        render(
            <MemoryRouter initialEntries={['/portfolios/details/1']}>
                <Routes>
                    <Route path="/portfolios/details/:id" element={<PortfolioItemDetail />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByText(/An unexpected error occurred/i)).toBeInTheDocument();
        });
    });

    test('renders portfolio item details when data is fetched successfully', async () => {
        const mockPortfolioItem = {
            title: 'Portfolio Item 1',
            description: 'A description of the portfolio item',
            type: 'Type A',
            technologies: 'React, Node.js',
            skills: 'JavaScript, CSS',
            industry: 'Tech',
            role: 'Frontend Developer',
            duration: '6 months',
            link: 'https://example.com',
            attachmentUrl: 'https://example.com/attachment.pdf',
            tags: 'Tag1, Tag2',
        };

        fetchData.mockResolvedValueOnce({ data: mockPortfolioItem });

        render(
            <MemoryRouter initialEntries={['/portfolios/details/1']}>
                <Routes>
                    <Route path="/portfolios/details/:id" element={<PortfolioItemDetail />} />
                </Routes>
            </MemoryRouter>
        );

        await waitFor(() => {
            // Check if the title is rendered
            expect(screen.getByText('Portfolio Item 1')).toBeInTheDocument();

            // Check other portfolio details
            expect(screen.getByText('A description of the portfolio item')).toBeInTheDocument();
            expect(screen.getByText('Type A')).toBeInTheDocument();
            expect(screen.getByText('React, Node.js')).toBeInTheDocument();
            expect(screen.getByText('JavaScript, CSS')).toBeInTheDocument();
            expect(screen.getByText('Tech')).toBeInTheDocument();
            expect(screen.getByText('Frontend Developer')).toBeInTheDocument();
            expect(screen.getByText('6 months')).toBeInTheDocument();

            // Check if the link and attachment URLs are rendered as clickable links
            expect(screen.getByRole('link', { name: /https:\/\/example\.com/i })).toBeInTheDocument();
            expect(screen.getByRole('link', { name: /Download Attachment/i })).toBeInTheDocument();

            // Check tags
            expect(screen.getByText('Tag1, Tag2')).toBeInTheDocument();
        });
    });
});
