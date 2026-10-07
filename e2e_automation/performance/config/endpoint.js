
export const API_ROUTES = {
    // Update this to match your screenshot
    LOGIN: '/api/users/login', 
    TRANSFER: (fromId, recipientId) => `/api/account//transfer/${fromId}/${recipientId}`,
};