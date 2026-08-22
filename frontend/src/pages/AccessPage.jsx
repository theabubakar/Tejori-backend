import { Link } from 'react-router-dom';
import AuthLayout from '../components/AuthLayout';
import BrandHeader from '../components/BrandHeader';
import { clearAuth, getUser } from '../utils/storage';

export default function AccessPage() {
  const user = getUser();

  function handleSignOut() {
    clearAuth();
  }

  return (
    <AuthLayout>
      <BrandHeader subtitle="Welcome back" />
      <div className="text-center">
        <p className="mb-2">You are signed in.</p>
        {user?.fullName && <p className="fw-semibold">{user.fullName}</p>}
        {user?.email && <p className="text-muted">{user.email}</p>}
        {user?.isEmailVerified && (
          <span className="badge rounded-pill text-bg-success mb-4">Email verified</span>
        )}
      </div>
      <div className="d-grid gap-3 mt-3">
        <Link to="/" className="btn tijori-btn-outline text-decoration-none" onClick={handleSignOut}>
          Sign Out
        </Link>
      </div>
    </AuthLayout>
  );
}
