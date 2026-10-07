import http from 'k6/http';
import { API_ROUTES } from '../config/endpoint.js';

export function executeTransfer(baseUrl, payload){
    const userToken = payload.token;
    const fromId = payload.from;
    const recipientId = payload.to;

    const url = `${baseUrl}${API_ROUTES.TRANSFER(fromId, recipientId)}`;

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${userToken}`,
        },
    };

    return http.put(url, JSON.stringify(payload), params);
}


