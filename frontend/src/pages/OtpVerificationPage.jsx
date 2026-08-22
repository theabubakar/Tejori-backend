import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import AlertMessage from '../components/AlertMessage';
import AuthLayout from '../components/AuthLayout';
import BackLink from '../components/BackLink';
import LoadingButton from '../components/LoadingButton';
import OtpInput from '../components/OtpInput';
import {
  resendForgotPasswordOtpByEmail,
  resendRegistrationOtpByEmail,
  verifyForgotPasswordOtp,
  verifyRegistrationOtp,
} from '../services/authService';
import { saveAuth } from '../utils/storage';
import { isValidOtp } from '../utils/validation';

export default function OtpVerificationPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const isForgotFlow = location.pathname.includes('forgot-password');
  const flow = location.state?.flow || (isForgotFlow ? 'forgot-password' : 'registration');
  const email = location.state?.email || '';
  const maskedEmail = location.state?.maskedEmail || email;

  const [otpCode, setOtpCode] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const [resendLoading, setResendLoading] = useState(false);

  useEffect(() => {
    if (!email) {
      navigate(flow === 'forgot-password' ? '/forgot-password' : '/register', { replace: true });
    }
  }, [email, flow, navigate]);

  async function handleVerify(event) {
    event.preventDefault();
    setError('');
    setSuccess('');

    if (!isValidOtp(otpCode)) {
      setError('OTP must be a 4-digit code.');
      return;
    }

    setLoading(true);

    try {
      if (flow === 'forgot-password') {
        const response = await verifyForgotPasswordOtp({ email, otpCode });
        navigate('/reset-password', {
          state: {
            email,
            resetToken: response.data?.resetToken,
          },
        });
        return;
      }

      const response = await verifyRegistrationOtp({ email, otpCode });
      saveAuth(response.data?.authToken);
      navigate('/account-created');
    } catch (verifyError) {
      setError(verifyError.message);
    } finally {
      setLoading(false);
    }
  }

  async function handleResend() {
    setError('');
    setSuccess('');
    setResendLoading(true);

    try {
      const response =
        flow === 'forgot-password'
          ? await resendForgotPasswordOtpByEmail({ email })
          : await resendRegistrationOtpByEmail({ email });

      setSuccess(response.message || 'OTP sent successfully.');
    } catch (resendError) {
      setError(resendError.message);
    } finally {
      setResendLoading(false);
    }
  }

  return (
    <AuthLayout>
      <BackLink to={flow === 'forgot-password' ? '/forgot-password' : '/register'} />
      <h2 className="tijori-title text-center">OTP</h2>
      <p className="tijori-subtitle text-center">
        We sent a One Time Password to <strong>{maskedEmail}</strong>
      </p>

      <AlertMessage type="success" message={success} />
      <AlertMessage message={error} />

      <form onSubmit={handleVerify}>
        <OtpInput value={otpCode} onChange={setOtpCode} disabled={loading} />

        <LoadingButton type="submit" loading={loading}>
          Verify
        </LoadingButton>
      </form>

      <div className="tijori-divider">Didn&apos;t receive the code?</div>

      <LoadingButton variant="outline" loading={resendLoading} onClick={handleResend}>
        Send OTP By Email
      </LoadingButton>

      {flow === 'registration' && (
        <p className="text-center mt-3 mb-0">
          <Link to="/login" className="tijori-link">
            Back to Sign In
          </Link>
        </p>
      )}
    </AuthLayout>
  );
}
