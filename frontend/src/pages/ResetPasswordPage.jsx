import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import AlertMessage from '../components/AlertMessage';
import AuthLayout from '../components/AuthLayout';
import BackLink from '../components/BackLink';
import FormField from '../components/FormField';
import LoadingButton from '../components/LoadingButton';
import { resetPassword } from '../services/authService';
import { isValidPassword, passwordHelpText } from '../utils/validation';

export default function ResetPasswordPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const resetToken = location.state?.resetToken || '';

  const [form, setForm] = useState({
    newPassword: '',
    confirmNewPassword: '',
  });
  const [errors, setErrors] = useState({});
  const [apiError, setApiError] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  useEffect(() => {
    if (!resetToken) {
      navigate('/forgot-password', { replace: true });
    }
  }, [resetToken, navigate]);

  function handleChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
    setErrors((current) => ({ ...current, [name]: '' }));
    setApiError('');
  }

  function validate() {
    const nextErrors = {};
    if (!isValidPassword(form.newPassword)) nextErrors.newPassword = passwordHelpText();
    if (form.confirmNewPassword !== form.newPassword) {
      nextErrors.confirmNewPassword = 'Passwords do not match.';
    }
    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  }

  async function handleSubmit(event) {
    event.preventDefault();
    if (!validate()) return;

    setLoading(true);
    setApiError('');

    try {
      await resetPassword({
        resetToken,
        newPassword: form.newPassword,
        confirmNewPassword: form.confirmNewPassword,
      });
      navigate('/password-changed');
    } catch (error) {
      setApiError(error.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AuthLayout>
      <BackLink to="/forgot-password/verify-otp" />
      <h2 className="tijori-title">New Password</h2>
      <p className="tijori-subtitle">Please enter a new password</p>

      <AlertMessage message={apiError} />

      <form onSubmit={handleSubmit} noValidate>
        <FormField
          label="New Password"
          name="newPassword"
          type={showPassword ? 'text' : 'password'}
          value={form.newPassword}
          onChange={handleChange}
          placeholder="******"
          required
          error={errors.newPassword}
          autoComplete="new-password"
        />

        <FormField
          label="Confirm New Password"
          name="confirmNewPassword"
          type={showPassword ? 'text' : 'password'}
          value={form.confirmNewPassword}
          onChange={handleChange}
          placeholder="******"
          required
          error={errors.confirmNewPassword}
          autoComplete="new-password"
        />

        <div className="form-check mb-3">
          <input
            className="form-check-input"
            type="checkbox"
            id="showResetPassword"
            checked={showPassword}
            onChange={(event) => setShowPassword(event.target.checked)}
          />
          <label className="form-check-label" htmlFor="showResetPassword">
            Show password
          </label>
        </div>

        <LoadingButton type="submit" loading={loading}>
          Change Password
        </LoadingButton>
      </form>

      <p className="text-center mt-3 mb-0">
        <Link to="/login" className="tijori-link">
          Back to Sign In
        </Link>
      </p>
    </AuthLayout>
  );
}
