import config from '../config';

const apiBaseUrl = config.apiBaseUrl;

export const fetchData = async (endpoint, options = {}) => {
    const token = localStorage.getItem('jwtToken');
    let body;

    if (options.body) {
        if (options.isMultiPart) {
            // Use FormData if form contains files
            body = new FormData();
            Object.keys(options.body).forEach((key) => {
                if (options.body[key] !== null && options.body[key] !== undefined && options.body[key] !== '') {
                    body.append(key, options.body[key]);
                }
            });
        }
        else {
            // Otherwise, use JSON
            body = JSON.stringify(
                Object.keys(options.body).reduce((acc, key) => {
                    acc[key] = options.body[key] === '' ? null : options.body[key];
                    return acc;
                }, {})
            );
        }
    }

    try {
        const response = await fetch(`${apiBaseUrl}${endpoint}`, {
            method: options.method || 'GET', // Default to 'GET'
            headers: {
                'Authorization': `Bearer ${token}`,
                ...(options.isMultiPart ? {} : { 'Content-Type': 'application/json' }),
                ...options.headers, // Allow additional headers
            },
            body: body || undefined,
        });

        return await response.json();
    } catch (error) {
        console.error('API error:', error);
        throw error;
    }
};
