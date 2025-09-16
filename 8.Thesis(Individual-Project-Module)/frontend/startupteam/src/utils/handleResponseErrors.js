export const handleResponseErrors = (responseErrors, setError, getValues) => {
    const normalizedResponseErrors = Object.keys(responseErrors).reduce((acc, key) => {
        acc[key.toLowerCase()] = responseErrors[key];
        return acc;
    }, {});

    const formFields = Object.keys(getValues()).map(field => field.toLowerCase());

    // Handle field-specific errors
    formFields.forEach(field => {
        const normalizedField = field.toLowerCase();
        if (normalizedResponseErrors[normalizedField]) {
            setError(field, { type: 'manual', message: normalizedResponseErrors[normalizedField][0] });
        }
    });

    // Handle general errors
    const generalErrors = Object.keys(normalizedResponseErrors)
        .filter(key => !formFields.includes(key))
        .flatMap(key => normalizedResponseErrors[key]);

    return generalErrors.length > 0 ? generalErrors[0] : '';
};
