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

export function getBucketSetup() {
  return apiRequest('/api/buckets/setup', {
    headers: authHeaders(),
  });
}

export function addCustomCategory(name) {
  return apiRequest('/api/buckets/categories', {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify({ name }),
  });
}

export function deleteCustomCategory(categoryId) {
  return apiRequest(`/api/buckets/categories/${categoryId}`, {
    method: 'DELETE',
    headers: authHeaders(),
  });
}

export function uploadBucketFile(file) {
  const formData = new FormData();
  formData.append('file', file);

  return apiRequest('/api/buckets/files', {
    method: 'POST',
    headers: authHeaders(),
    body: formData,
  });
}

export function createBucket(payload) {
  return apiRequest('/api/buckets', {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
}

export function getCategoryFormFields(categoryId) {
  return apiRequest(`/api/buckets/categories/${categoryId}/fields`, {
    headers: authHeaders(),
  });
}

export function addCategoryFormField(categoryId, payload) {
  return apiRequest(`/api/buckets/categories/${categoryId}/fields`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(payload),
  });
}
