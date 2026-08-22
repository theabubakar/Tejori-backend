import { Link } from 'react-router-dom';
import AuthLayout from '../components/AuthLayout';
import LoadingButton from '../components/LoadingButton';
import { getUser } from '../utils/storage';

export default function AccountCreatedPage() {
  const user = getUser();

  return (
    <AuthLayout>
      <h2 className="tijori-title text-center">Account Created</h2>
      <p className="tijori-subtitle text-center">Your account was successfully created</p>
      <div className="tijori-success-icon" aria-hidden="true">
        &#128077;
      </div>
      {user?.fullName && <p className="text-center mb-4">Welcome, {user.fullName}</p>}
      <Link to="/home" className="text-decoration-none d-block">
        <LoadingButton variant="primary">Get Access to the App</LoadingButton>
      </Link>
    </AuthLayout>
  );
}
