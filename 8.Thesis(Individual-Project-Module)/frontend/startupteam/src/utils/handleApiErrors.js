import { handleResponseErrors } from './handleResponseErrors';

export const handleApiErrors = (response, setError, getValues = false, setGeneralError) => {
    let hasErrors = false;

    // Handle validation error format
    if (response.errors) {
        // Collect all validation error messages
        const validationErrors = Object.values(response.errors).flat().join(' ');
        setGeneralError(prev => prev || validationErrors);
        hasErrors = true;
    }
    // Check if the response follows the ApiResponse<T> format
    else if (!response.success && !response.title) {
        if (!getValues) {
            if (response.errors) {
                const generalErrors = handleResponseErrors(response.errors, setError, getValues);
                if (generalErrors) {
                    setGeneralError(prev => prev || generalErrors);
                    hasErrors = true;
                }
            }
        }
        if (response.message) {
            setGeneralError(prev => prev || response.message);
            hasErrors = true;
        }
    }

    return hasErrors;
};