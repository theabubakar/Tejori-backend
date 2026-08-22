const TOKEN_KEY = 'tijori_access_token';
const USER_KEY = 'tijori_user';

export function saveAuth(authToken) {
  if (!authToken) return;
  localStorage.setItem(TOKEN_KEY, authToken.accessToken);
  localStorage.setItem(USER_KEY, JSON.stringify(authToken.user ?? {}));
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function getUser() {
  const raw = localStorage.getItem(USER_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw);
  } catch {
    return null;
  }
}

export function clearAuth() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

export function isAuthenticated() {
  return Boolean(getToken());
}
