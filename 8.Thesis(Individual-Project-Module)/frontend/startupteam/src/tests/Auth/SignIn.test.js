import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import SignIn from '../../pages/Auth/SignIn';
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

describe('SignIn Component', () => {
  const mockNavigate = jest.fn();

  beforeEach(() => {
    useNavigate.mockReturnValue(mockNavigate);
    jest.clearAllMocks(); // Clear any previous mock data
  });

  afterEach(() => {
    jest.resetAllMocks(); // Ensure all mocks are reset after each test
  });

  test('renders SignIn form with initial values', () => {
    render(
      <MemoryRouter>
        <SignIn />
      </MemoryRouter>
    );

    // Check if form fields are present
    expect(screen.getByLabelText(/Email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Sign In/i })).toBeInTheDocument();
  });

  test('displays validation errors when fields are empty', async () => {
    render(
      <MemoryRouter>
        <SignIn />
      </MemoryRouter>
    );

    // Click the submit button without filling in fields
    fireEvent.click(screen.getByRole('button', { name: /Sign In/i }));

    // Wait for the validation errors to be displayed
    await waitFor(() => {
      expect(screen.getByText(/Email is required/i)).toBeInTheDocument();
      expect(screen.getByText(/Password is required/i)).toBeInTheDocument();
    });
  });

  test('handles successful form submission and redirects', async () => {
    fetchData.mockResolvedValueOnce({
      data: { token: 'mockToken' },
    });
    handleApiErrors.mockReturnValue(false); // No errors

    render(
      <MemoryRouter>
        <SignIn />
      </MemoryRouter>
    );

    // Fill in the form
    fireEvent.input(screen.getByLabelText(/Email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.input(screen.getByLabelText(/Password/i), {
      target: { value: 'password123' },
    });

    // Submit the form
    fireEvent.click(screen.getByRole('button', { name: /Sign In/i }));

    // Wait for the navigation after successful submission
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
    expect(localStorage.getItem('jwtToken')).toBe('mockToken');
  });

  test('displays general error when form submission fails', async () => {
    fetchData.mockRejectedValueOnce(new Error('API call failed'));

    render(
      <MemoryRouter>
        <SignIn />
      </MemoryRouter>
    );

    // Fill in the form
    fireEvent.input(screen.getByLabelText(/Email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.input(screen.getByLabelText(/Password/i), {
      target: { value: 'password123' },
    });

    // Submit the form
    fireEvent.click(screen.getByRole('button', { name: /Sign In/i }));

    // Wait for the error message to be displayed
    await waitFor(() => {
      expect(screen.getByText(/An unexpected error occurred./i)).toBeInTheDocument();
    });
  });
});
