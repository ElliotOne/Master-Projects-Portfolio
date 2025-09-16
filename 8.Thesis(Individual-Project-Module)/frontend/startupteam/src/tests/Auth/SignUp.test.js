import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import SignUp from '../../pages/Auth/SignUp';
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

describe('SignUp Component', () => {
  const mockNavigate = jest.fn();

  beforeEach(() => {
    useNavigate.mockReturnValue(mockNavigate);
    jest.clearAllMocks(); // Clear any previous mock data
  });

  test('renders SignUp form with initial values', () => {
    render(
      <MemoryRouter>
        <SignUp />
      </MemoryRouter>
    );

    // Check if form fields are present using their 'id' attributes
    expect(screen.getByLabelText(/Email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Password/i, { selector: '#password' })).toBeInTheDocument();
    expect(screen.getByLabelText(/Confirm Password/i, { selector: '#confirmPassword' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Sign Up/i })).toBeInTheDocument();
  });

  test('should display validation errors when fields are empty', async () => {
    render(
      <MemoryRouter>
        <SignUp />
      </MemoryRouter>
    );

    // Click the submit button without filling in fields
    fireEvent.click(screen.getByRole('button', { name: /Sign Up/i }));

    // Wait for the validation errors to be displayed
    expect(await screen.findByText(/Email is required/i)).toBeInTheDocument();

    const passwordInput = screen.getByLabelText(/Password/i, { selector: '#password' });
    const confirmPasswordInput = screen.getByLabelText(/Confirm Password/i, { selector: '#confirmPassword' });

    // Find the nearest validation error for the password field
    expect(await screen.findByText((_, element) =>
      element.textContent === 'Password is required' &&
      passwordInput.closest('.form-group').contains(element)
    )).toBeInTheDocument();

    // Find the nearest validation error for the confirm password field
    expect(await screen.findByText((_, element) =>
      element.textContent === 'Confirm Password is required' &&
      confirmPasswordInput.closest('.form-group').contains(element)
    )).toBeInTheDocument();
  });

  test('displays validation error when passwords do not match', async () => {
    render(
      <MemoryRouter>
        <SignUp />
      </MemoryRouter>
    );

    // Fill in the form with non-matching passwords
    fireEvent.input(screen.getByLabelText(/Password/i, { selector: '#password' }), {
      target: { value: 'password123' },
    });
    fireEvent.input(screen.getByLabelText(/Confirm Password/i, { selector: '#confirmPassword' }), {
      target: { value: 'password456' },
    });

    // Submit the form
    fireEvent.click(screen.getByRole('button', { name: /Sign Up/i }));

    // Wait for the validation error to be displayed
    expect(await screen.findByText(/Passwords do not match/i)).toBeInTheDocument();
  });

  test('handles successful form submission and redirects', async () => {
    fetchData.mockResolvedValueOnce({
      data: { email: 'test@example.com' },
    });
    handleApiErrors.mockReturnValue(false); // No errors

    render(
      <MemoryRouter>
        <SignUp />
      </MemoryRouter>
    );

    // Fill in the form
    fireEvent.input(screen.getByLabelText(/Email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.input(screen.getByLabelText(/Password/i, { selector: '#password' }), {
      target: { value: 'password123' },
    });
    fireEvent.input(screen.getByLabelText(/Confirm Password/i, { selector: '#confirmPassword' }), {
      target: { value: 'password123' },
    });

    // Submit the form
    fireEvent.click(screen.getByRole('button', { name: /Sign Up/i }));

    // Wait for the navigation after successful submission
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/complete-signup', { state: { email: 'test@example.com' } });
    });
  });

  test('displays general error when form submission fails', async () => {
    fetchData.mockRejectedValueOnce(new Error('API call failed'));
    handleApiErrors.mockReturnValue(true); // Simulate API errors

    render(
      <MemoryRouter>
        <SignUp />
      </MemoryRouter>
    );

    // Fill in the form
    fireEvent.input(screen.getByLabelText(/Email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.input(screen.getByLabelText(/Password/i, { selector: '#password' }), {
      target: { value: 'password123' },
    });
    fireEvent.input(screen.getByLabelText(/Confirm Password/i, { selector: '#confirmPassword' }), {
      target: { value: 'password123' },
    });

    // Submit the form
    fireEvent.click(screen.getByRole('button', { name: /Sign Up/i }));

    // Wait for the error message to be displayed
    await waitFor(() => {
      expect(screen.getByText('An unexpected error occurred.')).toBeInTheDocument();
    });
  });
});
