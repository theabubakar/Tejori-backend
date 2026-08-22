const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

function extractValidationErrors(payload) {
  if (Array.isArray(payload?.errors)) {
    return payload.errors.filter(Boolean);
  }

  if (payload?.errors && typeof payload.errors === 'object') {
    return Object.values(payload.errors).flat().filter(Boolean);
  }

  return [];
}

function buildApiErrorMessage(payload, status) {
  const validationErrors = extractValidationErrors(payload);

  if (validationErrors.length > 0) {
    return validationErrors.join(' ');
  }

  if (payload?.message) {
    return payload.message;
  }

  if (payload?.title) {
    return payload.title;
  }

  return `Request failed with status ${status}`;
}

async function parseResponse(response) {
  const contentType = response.headers.get('content-type') || '';

  if (contentType.includes('application/json')) {
    return response.json();
  }

  const rawText = await response.text();
  const trimmedText = rawText.trim();

  if (trimmedText.startsWith('{') || trimmedText.startsWith('[')) {
    try {
      return JSON.parse(trimmedText);
    } catch {
      return {
        success: false,
        message: trimmedText || `Unexpected server response (${response.status}).`,
        data: null,
        errors: null,
      };
    }
  }

  return {
    success: false,
    message: trimmedText || `Unexpected server response (${response.status}).`,
    data: null,
    errors: null,
  };
}

function buildNetworkError(error) {
  const message =
    'Unable to connect to the API. Start the backend with `dotnet run` in src/Tijori.API. ' +
    'For local development, leave VITE_API_BASE_URL empty so Vite proxies /api to http://localhost:5022.';

  const networkError = new Error(message);
  networkError.isNetworkError = true;
  networkError.originalError = error;
  return networkError;
}

export async function apiRequest(path, options = {}) {
  const url = `${API_BASE_URL}${path}`;
  const isFormData = options.body instanceof FormData;

  let response;

  try {
    response = await fetch(url, {
      ...options,
      headers: {
        ...(isFormData ? {} : { 'Content-Type': 'application/json' }),
        ...(options.headers || {}),
      },
    });
  } catch (error) {
    throw buildNetworkError(error);
  }

  const payload = await parseResponse(response);

  if (!response.ok) {
    const validationErrors = extractValidationErrors(payload);

    const message = buildApiErrorMessage(payload, response.status);

    const apiError = new Error(message);
    apiError.status = response.status;
    apiError.payload = payload;
    apiError.validationErrors = validationErrors;
    throw apiError;
  }

  if (payload?.success === false) {
    const validationErrors = extractValidationErrors(payload);

    const message = buildApiErrorMessage(payload, response.status);
    const apiError = new Error(message);
    apiError.payload = payload;
    apiError.validationErrors = validationErrors;
    throw apiError;
  }

  return payload;
}

export { API_BASE_URL };
