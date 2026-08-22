import { apiRequest } from './api';
import { getToken } from '../utils/storage';

function authHeaders() {
  const token = getToken();
  if (!token) {
    const error = new Error('You are not signed in.');
    error.status = 401;
    throw error;
  }

  return {
    Authorization: `Bearer ${token}`,
  };
}

export function getProfile() {
  return apiRequest('/api/profile', {
    headers: authHeaders(),
  });
}
