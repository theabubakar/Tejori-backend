import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import AlertMessage from '../components/AlertMessage';
import AuthLayout from '../components/AuthLayout';
import BackLink from '../components/BackLink';
import FormField from '../components/FormField';
import LoadingButton from '../components/LoadingButton';
import { register } from '../services/authService';
import {
  isValidEmail,
  isValidPassword,
  isValidPhone,
  passwordHelpText,
} from '../utils/validation';

const initialForm = {
  fullName: '',
  countryCode: '+92',
  phoneNumber: '',
  email: '',
  password: '',
  confirmPassword: '',
  acceptTerms: false,
};

export default function RegisterPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState(initialForm);
  const [errors, setErrors] = useState({});
  const [apiError, setApiError] = useState('');
  const [apiErrors, setApiErrors] = useState([]);
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  function handleChange(event) {
    const { name, value, type, checked } = event.target;
    setForm((current) => ({
      ...current,
      [name]: type === 'checkbox' ? checked : value,
    }));
    setErrors((current) => ({ ...current, [name]: '' }));
    setApiError('');
    setApiErrors([]);
  }

  function validate() {
    const nextErrors = {};

    if (!form.fullName.trim()) nextErrors.fullName = 'Full name is required.';
    if (!isValidPhone(form.phoneNumber)) nextErrors.phoneNumber = 'Enter a valid phone number.';
    if (!isValidEmail(form.email)) nextErrors.email = 'Enter a valid email address.';
    if (!isValidPassword(form.password)) nextErrors.password = passwordHelpText();
    if (form.confirmPassword !== form.password) {
      nextErrors.confirmPassword = 'Passwords do not match.';
    }
    if (!form.acceptTerms) nextErrors.acceptTerms = 'You must accept the terms and conditions.';

    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  }

  async function handleSubmit(event) {
    event.preventDefault();
    if (!validate()) return;

    setLoading(true);
    setApiError('');
    setApiErrors([]);

    try {
      const response = await register(form);
      navigate('/verify-otp', {
        state: {
          flow: 'registration',
          email: form.email.trim(),
          maskedEmail: response.data?.maskedEmail || form.email,
        },
      });
    } catch (error) {
      setApiError(error.message);
      setApiErrors(error.validationErrors || error.payload?.errors || []);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AuthLayout wide>
      <BackLink to="/" />
      <h2 className="tijori-title">Create an Account</h2>
      <p className="tijori-subtitle">
        I already have an account.{' '}
        <Link to="/login" className="tijori-link">
          Sign In
        </Link>
      </p>

      <AlertMessage message={apiError} errors={apiErrors} />

      <form onSubmit={handleSubmit} noValidate>
        <FormField
          label="Full Name"
          name="fullName"
          value={form.fullName}
          onChange={handleChange}
          placeholder="Enter Full Name"
          required
          error={errors.fullName}
        />

        <div className="mb-3">
          <label className="tijori-label">
            Phone Number <span className="tijori-required">*</span>
          </label>
          <div className="tijori-phone-row">
            <select
              name="countryCode"
              className="tijori-select"
              value={form.countryCode}
              onChange={handleChange}
            >
              <option value="+92">+92</option>
              <option value="+965">+965</option>
            </select>
            <input
              name="phoneNumber"
              className="tijori-input"
              value={form.phoneNumber}
              onChange={handleChange}
              placeholder="000 0000"
            />
          </div>
          {errors.phoneNumber && <div className="tijori-field-error">{errors.phoneNumber}</div>}
        </div>

        <FormField
          label="Email"
          name="email"
          type="email"
          value={form.email}
          onChange={handleChange}
          placeholder="E.g.: User@domain.com"
          required
          error={errors.email}
          autoComplete="email"
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
          autoComplete="new-password"
        />

        <FormField
          label="Confirm Password"
          name="confirmPassword"
          type={showPassword ? 'text' : 'password'}
          value={form.confirmPassword}
          onChange={handleChange}
          placeholder="******"
          required
          error={errors.confirmPassword}
          autoComplete="new-password"
        />

        <div className="mb-3">
          <label className="tijori-checkbox-label">
            <input
              type="checkbox"
              name="acceptTerms"
              checked={form.acceptTerms}
              onChange={handleChange}
            />
            <span>I have read and accept the terms and conditions.</span>
          </label>
          {errors.acceptTerms && <div className="tijori-field-error">{errors.acceptTerms}</div>}
        </div>

        <div className="form-check mb-3">
          <input
            className="form-check-input"
            type="checkbox"
            id="showPassword"
            checked={showPassword}
            onChange={(event) => setShowPassword(event.target.checked)}
          />
          <label className="form-check-label" htmlFor="showPassword">
            Show password
          </label>
        </div>

        <LoadingButton type="submit" loading={loading}>
          Register
        </LoadingButton>
      </form>
    </AuthLayout>
  );
}
