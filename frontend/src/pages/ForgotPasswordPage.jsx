import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../components/AlertMessage';
import AuthLayout from '../components/AuthLayout';
import BackLink from '../components/BackLink';
import FormField from '../components/FormField';
import LoadingButton from '../components/LoadingButton';
import { sendForgotPasswordOtpByEmail } from '../services/authService';
import { isValidEmail } from '../utils/validation';

export default function ForgotPasswordPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');

    if (!isValidEmail(email)) {
      setError('Enter a valid email address.');
      return;
    }

    setLoading(true);

    try {
      const response = await sendForgotPasswordOtpByEmail({ email: email.trim() });
      navigate('/forgot-password/verify-otp', {
        state: {
          flow: 'forgot-password',
          email: email.trim(),
          maskedEmail: response.data?.maskedRecipient || email.trim(),
        },
      });
    } catch (submitError) {
      setError(submitError.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AuthLayout>
      <BackLink to="/login" label="Back to Sign In" />
      <h2 className="tijori-title">Forgot Password</h2>
      <p className="tijori-subtitle">Please enter your email to send an OTP for verification.</p>

      <AlertMessage message={error} />

      <form onSubmit={handleSubmit} noValidate>
        <FormField
          label="Email"
          name="email"
          type="email"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
          placeholder="E.g.: User@domain.com"
          required
          autoComplete="email"
        />

        <LoadingButton type="submit" loading={loading}>
          Send OTP
        </LoadingButton>
      </form>
    </AuthLayout>
  );
}
