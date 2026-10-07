// performance/api/user_auth.js
import http from 'k6/http';
import { API_ROUTES } from '../config/endpoint.js';

export function login(baseUrl, username, password) {
    const url = `${baseUrl}${API_ROUTES.LOGIN}`;

    // Map the 'username' variable to the 'email' JSON key
    const payload = JSON.stringify({
        email: username,
        password: password
    });
    const params = {
        headers: {
            'Content-Type': 'application/json'
        },
    };


    return http.post(url, payload, params);
}