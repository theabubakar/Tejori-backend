import { apiRequest } from './api';

export function register(form) {
  return apiRequest('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify({
      fullName: form.fullName.trim(),
      countryCode: form.countryCode.trim(),
      phoneNumber: form.phoneNumber.replace(/\s/g, ''),
      email: form.email.trim(),
      password: form.password,
      confirmPassword: form.confirmPassword,
      acceptTerms: form.acceptTerms,
    }),
  });
}

export function verifyRegistrationOtp(payload) {
  return apiRequest('/api/auth/register/verify-otp', {
    method: 'POST',
    body: JSON.stringify({
      email: payload.email.trim(),
      otpCode: payload.otpCode.trim(),
    }),
  });
}

export function resendRegistrationOtpByEmail(payload) {
  return apiRequest('/api/auth/register/resend-otp/email', {
    method: 'POST',
    body: JSON.stringify({
      email: payload.email.trim(),
    }),
  });
}

export function login(payload) {
  return apiRequest('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({
      identifier: payload.identifier.trim(),
      password: payload.password,
    }),
  });
}

export function sendForgotPasswordOtpByEmail(payload) {
  return apiRequest('/api/auth/forgot-password/otp/email', {
    method: 'POST',
    body: JSON.stringify({
      email: payload.email.trim(),
    }),
  });
}

export function verifyForgotPasswordOtp(payload) {
  return apiRequest('/api/auth/forgot-password/verify-otp', {
    method: 'POST',
    body: JSON.stringify({
      email: payload.email.trim(),
      otpCode: payload.otpCode.trim(),
    }),
  });
}

export function resendForgotPasswordOtpByEmail(payload) {
  return apiRequest('/api/auth/forgot-password/resend-otp/email', {
    method: 'POST',
    body: JSON.stringify({
      email: payload.email.trim(),
    }),
  });
}

export function resetPassword(payload) {
  return apiRequest('/api/auth/forgot-password/reset-password', {
    method: 'POST',
    body: JSON.stringify({
      resetToken: payload.resetToken,
      newPassword: payload.newPassword,
      confirmNewPassword: payload.confirmNewPassword,
    }),
  });
}
