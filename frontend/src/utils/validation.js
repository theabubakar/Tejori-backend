const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const phonePattern = /^\d{7,15}$/;

export function isValidEmail(value) {
  return emailPattern.test(String(value || '').trim());
}

export function isValidPhone(value) {
  return phonePattern.test(String(value || '').replace(/\s/g, ''));
}

export function isValidPassword(value) {
  const password = String(value || '');
  return (
    password.length >= 8 &&
    /[A-Z]/.test(password) &&
    /[a-z]/.test(password) &&
    /\d/.test(password)
  );
}

export function passwordHelpText() {
  return 'Password must be at least 8 characters and include uppercase, lowercase, and a number.';
}

export function isValidOtp(value) {
  return /^\d{4}$/.test(String(value || '').trim());
}
