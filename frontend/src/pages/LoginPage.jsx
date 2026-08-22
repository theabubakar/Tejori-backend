import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import AlertMessage from '../components/AlertMessage';
import AuthLayout from '../components/AuthLayout';
import BackLink from '../components/BackLink';
import FormField from '../components/FormField';
import LoadingButton from '../components/LoadingButton';
import { login } from '../services/authService';
import { saveAuth } from '../utils/storage';

export default function LoginPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ identifier: '', password: '' });
  const [errors, setErrors] = useState({});
  const [apiError, setApiError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  function handleChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
    setErrors((current) => ({ ...current, [name]: '' }));
    setApiError('');
    setSuccessMessage('');
  }

  function validate() {
    const nextErrors = {};
    if (!form.identifier.trim()) nextErrors.identifier = 'Email or phone is required.';
    if (!form.password) nextErrors.password = 'Password is required.';
    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  }

  async function handleSubmit(event) {
    event.preventDefault();
    if (!validate()) return;

    setLoading(true);
    setApiError('');

    try {
      const response = await login(form);
      saveAuth(response.data);
      setSuccessMessage(response.message || 'Signed in successfully.');
      setTimeout(() => navigate('/home'), 600);
    } catch (error) {
      setApiError(error.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AuthLayout>
      <BackLink to="/" />
      <h2 className="tijori-title">Sign In</h2>
      <p className="tijori-subtitle">
        I don&apos;t have an account.{' '}
        <Link to="/register" className="tijori-link">
          Create an Account
        </Link>
      </p>

      <AlertMessage type="success" message={successMessage} />
      <AlertMessage message={apiError} />

      <form onSubmit={handleSubmit} noValidate>
        <FormField
          label="Phone Number or Email"
          name="identifier"
          value={form.identifier}
          onChange={handleChange}
          placeholder="Enter your Phone number or Email"
          required
          error={errors.identifier}
          autoComplete="username"
        />

        <FormField
          label="Password"
          name="password"
          type={showPassword ? 'text' : 'password'}
          value={form.password}
          onChange={handleChange}
          placeholder="******"
          required
          error={errors.password}
          autoComplete="current-password"
        />

        <div className="d-flex justify-content-between align-items-center mb-3">
          <div className="form-check">
            <input
              className="form-check-input"
              type="checkbox"
              id="showLoginPassword"
              checked={showPassword}
              onChange={(event) => setShowPassword(event.target.checked)}
            />
            <label className="form-check-label" htmlFor="showLoginPassword">
              Show password
            </label>
          </div>
          <Link to="/forgot-password" className="tijori-link">
            I forgot my password
          </Link>
        </div>

        <LoadingButton type="submit" loading={loading}>
          Sign In
        </LoadingButton>
      </form>
    </AuthLayout>
  );
}
