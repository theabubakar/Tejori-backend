import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import AlertMessage from '../components/AlertMessage';
import { getProfile } from '../services/profileService';
import { clearAuth, isAuthenticated } from '../utils/storage';
import { getInitials } from '../utils/formatters';

function ProfileNavIcon({ name }) {
  const icons = {
    language: '🌐',
    notifications: '🔔',
    privacy: '📄',
    info: 'ℹ️',
    contact: '🎧',
  };
  return <span className="profile-menu-icon">{icons[name] || '•'}</span>;
}

export default function ProfilePage() {
  const navigate = useNavigate();
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isAuthenticated()) {
      navigate('/login', { replace: true });
      return;
    }

    async function loadProfile() {
      setLoading(true);
      setError('');
      try {
        const response = await getProfile();
        setProfile(response.data);
      } catch (loadError) {
        if (loadError.status === 401) {
          clearAuth();
          navigate('/login', { replace: true });
          return;
        }
        setError(loadError.message || 'Unable to load profile.');
      } finally {
        setLoading(false);
      }
    }

    loadProfile();
  }, [navigate]);

  function handleLogout() {
    clearAuth();
    navigate('/login', { replace: true });
  }

  const phoneLabel =
    profile?.countryCode && profile?.phoneNumber
      ? `${profile.countryCode} ${profile.phoneNumber}`
      : profile?.phoneNumber || '—';

  return (
    <div className="profile-page">
      <div className="profile-shell">
        <div className="profile-topbar">
          <Link to="/home" className="add-bucket-back">
            ←
          </Link>
          <div className="profile-topbar-title">Profile</div>
          <div className="add-bucket-topbar-spacer" />
        </div>

        {loading && <div className="add-bucket-loading">Loading profile...</div>}
        {!loading && <AlertMessage message={error} />}

        {!loading && profile && (
          <>
            <section className="profile-header-card">
              <div className="profile-avatar-wrap">
                {profile.profileImageUrl ? (
                  <img src={profile.profileImageUrl} alt="" className="profile-avatar-image" />
                ) : (
                  <div className="profile-avatar-fallback">{getInitials(profile.fullName)}</div>
                )}
              </div>
              <h1 className="profile-name">{profile.fullName || 'User'}</h1>
              <p className="profile-meta">Phone number: {phoneLabel}</p>
              <p className="profile-meta">Email: {profile.email || '—'}</p>
              <Link to="/forgot-password" className="profile-change-password">
                Change Password
              </Link>
            </section>

            <section className="profile-section">
              <h2 className="profile-section-title">Settings</h2>
              <button type="button" className="profile-menu-card">
                <ProfileNavIcon name="language" />
                <span>Language</span>
                <strong>{profile.language || 'ENGLISH'}</strong>
              </button>
              <button type="button" className="profile-menu-card">
                <ProfileNavIcon name="notifications" />
                <span>Notifications</span>
                <strong>{profile.notificationPreference || 'ALL'}</strong>
              </button>
              <button type="button" className="profile-menu-card">
                <ProfileNavIcon name="privacy" />
                <span>Privacy Policy</span>
              </button>
            </section>

            <section className="profile-section">
              <h2 className="profile-section-title">Support</h2>
              <button type="button" className="profile-menu-card">
                <ProfileNavIcon name="info" />
                <span>Who we are?</span>
              </button>
              <button type="button" className="profile-menu-card">
                <ProfileNavIcon name="contact" />
                <span>Contact us</span>
              </button>
            </section>

            <button type="button" className="profile-logout-btn" onClick={handleLogout}>
              Logout
            </button>
            <button type="button" className="profile-delete-btn">
              Delete Account
            </button>
          </>
        )}
      </div>
    </div>
  );
}
