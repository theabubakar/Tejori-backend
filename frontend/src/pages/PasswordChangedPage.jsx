import { Link } from 'react-router-dom';
import AuthLayout from '../components/AuthLayout';
import LoadingButton from '../components/LoadingButton';

export default function PasswordChangedPage() {
  return (
    <AuthLayout>
      <h2 className="tijori-title text-center">Password Changed</h2>
      <p className="tijori-subtitle text-center">Your password was successfully updated</p>
      <div className="tijori-success-icon" aria-hidden="true">
        &#128077;
      </div>
      <Link to="/login" className="text-decoration-none d-block">
        <LoadingButton variant="primary">Get Access to the App</LoadingButton>
      </Link>
    </AuthLayout>
  );
}
