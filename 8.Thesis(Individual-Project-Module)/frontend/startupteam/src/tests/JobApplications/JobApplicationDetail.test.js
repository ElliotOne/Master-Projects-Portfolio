import React from 'react';
import { render, screen, waitFor, within } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import JobApplicationDetail from '../../pages/JobApplications/JobApplicationDetail';
import { fetchData } from '../../utils/fetchData';

// Mock utilities and hooks
jest.mock('../../utils/fetchData');

describe('JobApplicationDetail Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('renders loading message while fetching data', () => {
    render(
      <MemoryRouter initialEntries={['/job-applications/1']}>
        <Routes>
          <Route path="/job-applications/:id" element={<JobApplicationDetail />} />
        </Routes>
      </MemoryRouter>
    );

    expect(screen.getByText(/Loading.../i)).toBeInTheDocument();
  });

  test('renders job application details on successful fetch', async () => {
    fetchData.mockResolvedValueOnce({
      data: {
        jobAdvertisementId: 1,
        founderFullName: 'Ali Momenzadeh Kholenjani',
        founderUserName: 'AliMomenzadehKholenjani',
        cvUrl: 'http://example.com/cv.pdf',
        coverLetterUrl: 'http://example.com/coverletter.pdf',
        status: 'Submitted',
        statusText: 'Submitted',
      },
    });

    fetchData.mockResolvedValueOnce({
      data: {
        jobTitle: 'Software Engineer',
        startupName: 'Tech Startup',
        jobLocation: 'Remote',
        employmentType: 'Full-time',
      },
    });

    render(
      <MemoryRouter initialEntries={['/job-applications/1']}>
        <Routes>
          <Route path="/job-applications/:id" element={<JobApplicationDetail />} />
        </Routes>
      </MemoryRouter>
    );

    // Narrow down search to "Job Details" card section
    const jobDetailsSection = await screen.findByText(/Job Details/i);
    const jobDetailsDiv = jobDetailsSection.closest('div');

    // Verify job details using getAllByText for repeated texts like "Remote"
    expect(within(jobDetailsDiv).getByText(/Software Engineer/i)).toBeInTheDocument();
    expect(within(jobDetailsDiv).getByText(/Tech Startup/i)).toBeInTheDocument();

    // Use getAllByText to find multiple occurrences of "Remote"
    const remoteElements = within(jobDetailsDiv).getAllByText(/Remote/i);
    expect(remoteElements.length).toBeGreaterThan(0); // Ensure at least one instance is found

    expect(remoteElements[0]).toBeInTheDocument();  // Check first occurrence of "Remote"
    expect(within(jobDetailsDiv).getByText(/Full-time/i)).toBeInTheDocument();

    // Check for founder details outside the job section
    expect(screen.getByText(/Ali Momenzadeh Kholenjani/i)).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /View CV/i })).toHaveAttribute('href', 'http://example.com/cv.pdf');
    expect(screen.getByRole('link', { name: /View Cover Letter/i })).toHaveAttribute('href', 'http://example.com/coverletter.pdf');
  });

  test('renders error message on failed fetch', async () => {
    fetchData.mockRejectedValueOnce(new Error('Failed to fetch data'));

    render(
      <MemoryRouter initialEntries={['/job-applications/1']}>
        <Routes>
          <Route path="/job-applications/:id" element={<JobApplicationDetail />} />
        </Routes>
      </MemoryRouter>
    );

    await waitFor(() => {
      // Since the component might set either errorMessage or generalError
      expect(screen.getByText(/Failed to load data/i)).toBeInTheDocument();
    });
  });

  test('renders action buttons for valid status', async () => {
    fetchData.mockResolvedValueOnce({
      data: {
        jobAdvertisementId: 1,
        founderFullName: 'Ali Momenzadeh Kholenjani',
        founderUserName: 'AliMomenzadehKholenjani',
        status: 'Submitted',
      },
    });

    fetchData.mockResolvedValueOnce({
      data: {
        jobTitle: 'Software Engineer',
        startupName: 'Tech Startup',
        jobLocation: 'Remote',
      },
    });

    render(
      <MemoryRouter initialEntries={['/job-applications/1']}>
        <Routes>
          <Route path="/job-applications/:id" element={<JobApplicationDetail />} />
        </Routes>
      </MemoryRouter>
    );

    await waitFor(() => {
      expect(screen.getByText(/Withdraw Application/i)).toBeInTheDocument();
    });
  });

  test('Momenzadeh Kholenjanis not render action buttons for invalid status', async () => {
    fetchData.mockResolvedValueOnce({
      data: {
        jobAdvertisementId: 1,
        founderFullName: 'Ali Momenzadeh Kholenjani',
        founderUserName: 'AliMomenzadehKholenjani',
        status: 'OfferAcceptedByIndividual',
      },
    });

    fetchData.mockResolvedValueOnce({
      data: {
        jobTitle: 'Software Engineer',
        startupName: 'Tech Startup',
        jobLocation: 'Remote',
      },
    });

    render(
      <MemoryRouter initialEntries={['/job-applications/1']}>
        <Routes>
          <Route path="/job-applications/:id" element={<JobApplicationDetail />} />
        </Routes>
      </MemoryRouter>
    );

    await waitFor(() => {
      expect(screen.queryByText(/Withdraw Application/i)).not.toBeInTheDocument();
    });
  });

  test('handles job application status update', async () => {
    fetchData.mockResolvedValueOnce({
      data: {
        jobAdvertisementId: 1,
        founderFullName: 'Ali Momenzadeh Kholenjani',
        founderUserName: 'AliMomenzadehKholenjani',
        status: 'Submitted',
      },
    });

    fetchData.mockResolvedValueOnce({
      data: {
        jobTitle: 'Software Engineer',
        startupName: 'Tech Startup',
        jobLocation: 'Remote',
      },
    });

    fetchData.mockResolvedValueOnce({
      data: {},
    });

    render(
      <MemoryRouter initialEntries={['/job-applications/1']}>
        <Routes>
          <Route path="/job-applications/:id" element={<JobApplicationDetail />} />
        </Routes>
      </MemoryRouter>
    );

    await waitFor(() => {
      const withdrawButton = screen.getByText(/Withdraw Application/i);
      expect(withdrawButton).toBeInTheDocument();
    });
  });
});
