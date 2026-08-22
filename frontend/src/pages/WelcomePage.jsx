import { Link } from 'react-router-dom';
import AuthLayout from '../components/AuthLayout';
import BrandHeader from '../components/BrandHeader';

export default function WelcomePage() {
  return (
    <AuthLayout>
      <BrandHeader subtitle="All in one place!" />
      <p className="text-center mb-4">Get Access to your App</p>

      <div className="d-grid gap-3">
        <Link to="/register" className="btn tijori-btn-primary text-decoration-none">
          Register
        </Link>
        <Link to="/login" className="btn tijori-btn-dark text-decoration-none">
          Log In
        </Link>
      </div>
    </AuthLayout>
  );
}
