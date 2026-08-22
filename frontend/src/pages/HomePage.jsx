import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../components/AlertMessage';
import { getHome } from '../services/homeService';
import { clearAuth, isAuthenticated } from '../utils/storage';
import {
  bucketIcon,
  formatAmount,
  formatDate,
  formatStorageLabel,
  formatStoragePercentage,
  getInitials,
  getStorageProgressWidth,
  matchesSearchQuery,
} from '../utils/formatters';

function HomeIcon({ name }) {
  const icons = {
    folder: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path
          d="M4 7a2 2 0 0 1 2-2h4l2 2h8a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V7z"
          fill="currentColor"
        />
      </svg>
    ),
    shield: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path
          d="M12 3 20 6v6c0 5-3.4 9.4-8 10-4.6-.6-8-5-8-10V6l8-3z"
          fill="currentColor"
        />
      </svg>
    ),
    calendar: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path
          d="M7 2v2H5a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2h-2V2h-2v2H9V2H7zm12 8H5v8h14v-8z"
          fill="currentColor"
        />
      </svg>
    ),
    coin: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path
          d="M12 2a10 10 0 1 0 0 20 10 10 0 0 0 0-20zm1 2.1A8 8 0 0 1 19.9 11H13V4.1zM11 4.1V11H4.1A8 8 0 0 1 11 4.1zM4.1 13H11v6.9A8 8 0 0 1 4.1 13zm9.9 6.9V13h6.9a8 8 0 0 1-6.9 6.9z"
          fill="currentColor"
        />
      </svg>
    ),
    bell: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path
          d="M12 22a2.5 2.5 0 0 0 2.45-2h-4.9A2.5 2.5 0 0 0 12 22zm7-6v-5a7 7 0 1 0-14 0v5l-2 2v1h18v-1l-2-2z"
          fill="currentColor"
        />
      </svg>
    ),
    menu: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path d="M4 7h16v2H4V7zm0 4h16v2H4v-2zm0 4h16v2H4v-2z" fill="currentColor" />
      </svg>
    ),
    search: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path
          d="M10.5 3a7.5 7.5 0 1 1 4.7 13.4l4.3 4.3-1.4 1.4-4.3-4.3A7.5 7.5 0 0 1 10.5 3zm0 2a5.5 5.5 0 1 0 0 11 5.5 5.5 0 0 0 0-11z"
          fill="currentColor"
        />
      </svg>
    ),
    home: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path d="M12 3 3 11h2v9h6v-6h2v6h6v-9h2L12 3z" fill="currentColor" />
      </svg>
    ),
    documents: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path
          d="M6 2a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8l-6-6H6zm7 1.5L18.5 9H13V3.5z"
          fill="currentColor"
        />
      </svg>
    ),
    subscription: (
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path
          d="M4 4h16v4H4V4zm0 6h6v10H4V10zm8 0h8v10h-8V10z"
          fill="currentColor"
        />
      </svg>
    ),
  };

  return <span className="home-icon">{icons[name] || icons.folder}</span>;
}

export default function HomePage() {
  const navigate = useNavigate();
  const [homeData, setHomeData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchQuery, setSearchQuery] = useState('');

  const loadHomeData = useCallback(
    async ({ silent = false } = {}) => {
      if (!silent) {
        setLoading(true);
      }
      setError('');

      try {
        const response = await getHome();
        setHomeData(response.data);
      } catch (loadError) {
        if (loadError.status === 401) {
          clearAuth();
          navigate('/login', { replace: true });
          return;
        }
        setError(loadError.message || 'Unable to load home data.');
      } finally {
        if (!silent) {
          setLoading(false);
        }
      }
    },
    [navigate],
  );

  useEffect(() => {
    if (!isAuthenticated()) {
      navigate('/login', { replace: true });
      return;
    }

    loadHomeData();
  }, [loadHomeData, navigate]);

  const user = homeData?.user;
  const storage = homeData?.storage;
  const buckets = homeData?.buckets ?? [];
  const projects = homeData?.ongoingProjects ?? [];
  const milestones = homeData?.upcomingMilestones ?? [];
  const paymentAlerts = homeData?.paymentAlerts ?? [];
  const normalizedSearch = searchQuery.trim().toLowerCase();

  const filteredBuckets = useMemo(() => {
    if (!normalizedSearch) return buckets;
    return buckets.filter(
      (bucket) =>
        matchesSearchQuery(bucket.name, normalizedSearch) ||
        matchesSearchQuery(bucket.iconKey, normalizedSearch) ||
        matchesSearchQuery(bucket.documentCount, normalizedSearch),
    );
  }, [buckets, normalizedSearch]);

  const filteredProjects = useMemo(() => {
    if (!normalizedSearch) return projects;
    return projects.filter(
      (project) =>
        matchesSearchQuery(project.name, normalizedSearch) ||
        matchesSearchQuery(project.categoryName, normalizedSearch) ||
        matchesSearchQuery(project.status, normalizedSearch) ||
        matchesSearchQuery(project.documentCount, normalizedSearch),
    );
  }, [projects, normalizedSearch]);

  const filteredMilestones = useMemo(() => {
    if (!normalizedSearch) return milestones;
    return milestones.filter(
      (milestone) =>
        matchesSearchQuery(milestone.title, normalizedSearch) ||
        matchesSearchQuery(milestone.projectName, normalizedSearch) ||
        matchesSearchQuery(milestone.status, normalizedSearch) ||
        matchesSearchQuery(milestone.currency, normalizedSearch),
    );
  }, [milestones, normalizedSearch]);

  const filteredPaymentAlerts = useMemo(() => {
    if (!normalizedSearch) return paymentAlerts;
    return paymentAlerts.filter(
      (alert) =>
        matchesSearchQuery(alert.title, normalizedSearch) ||
        matchesSearchQuery(alert.projectName, normalizedSearch) ||
        matchesSearchQuery(alert.status, normalizedSearch) ||
        matchesSearchQuery(alert.currency, normalizedSearch),
    );
  }, [paymentAlerts, normalizedSearch]);

  return (
    <div className="home-page">
      <div className="home-shell">
        <header className="home-header">
          <div className="home-user-block">
            {user?.profileImageUrl ? (
              <img src={user.profileImageUrl} alt="" className="home-avatar" />
            ) : (
              <div className="home-avatar home-avatar-fallback">{getInitials(user?.fullName)}</div>
            )}
            <div>
              <div className="home-welcome-label">Welcome</div>
              <div className="home-user-name">{user?.fullName || 'User'}</div>
            </div>
          </div>

          <div className="home-header-actions">
            <button type="button" className="home-icon-btn" aria-label="Notifications">
              <HomeIcon name="bell" />
              <span className="home-notification-dot" />
            </button>
            <button type="button" className="home-reminder-btn">
              <span className="home-reminder-plus">+</span>
              <span className="home-reminder-label">Create reminder</span>
            </button>
            <button
              type="button"
              className="home-icon-btn"
              aria-label="Menu"
              onClick={() => navigate('/profile')}
            >
              <HomeIcon name="menu" />
            </button>
          </div>
        </header>

        <div className="home-search-wrap">
          <input
            type="search"
            className="home-search"
            placeholder="Search..."
            aria-label="Search"
            value={searchQuery}
            onChange={(event) => setSearchQuery(event.target.value)}
          />
          <HomeIcon name="search" />
        </div>

        {loading && (
          <div className="home-loading">
            <div className="spinner-border text-info" role="status">
              <span className="visually-hidden">Loading...</span>
            </div>
            <p className="mb-0 mt-3">Loading your home data...</p>
          </div>
        )}

        {!loading && error && <AlertMessage message={error} />}

        {!loading && !error && homeData && (
          <>
            <section className="home-card home-storage-card">
              <div className="home-storage-top">
                <span>Available Storage</span>
                <strong>{formatStoragePercentage(storage?.percentageUsed, storage?.usedBytes)}</strong>
              </div>
              <div className="home-progress-track">
                <div
                  className="home-progress-fill"
                  style={{
                    width: `${getStorageProgressWidth(storage?.percentageUsed, storage?.usedBytes)}%`,
                  }}
                />
              </div>
              <div className="home-storage-meta">
                {formatStorageLabel(storage?.usedBytes ?? 0, storage?.totalBytes ?? 0)}
              </div>
            </section>

            <section className="home-ad-card" aria-label="Advertisement placeholder">
              <div className="home-ad-content">
                <span>YOUR AD HERE</span>
              </div>
            </section>

            {filteredBuckets.length > 0 && (
              <section className="home-buckets">
                {filteredBuckets.map((bucket) => (
                  <article key={bucket.id} className="home-card home-bucket-card">
                    <div className={`home-bucket-icon home-bucket-icon-${bucketIcon(bucket.iconKey)}`}>
                      <HomeIcon name={bucketIcon(bucket.iconKey)} />
                    </div>
                    <div className="home-bucket-name">{bucket.name}</div>
                    {bucket.documentCount > 0 && (
                      <div className="home-bucket-count">{bucket.documentCount} docs</div>
                    )}
                  </article>
                ))}
              </section>
            )}

            <section className="home-section">
              <div className="home-section-header">
                <div>
                  <h2 className="home-section-title">Ongoing Projects</h2>
                  <p className="home-section-subtitle">My documents of actual projects</p>
                </div>
                <button type="button" className="home-add-btn" onClick={() => navigate('/add-bucket')}>
                  + Add Bucket
                </button>
              </div>

              {filteredProjects.length === 0 ? (
                <div className="home-empty-card">
                  {normalizedSearch ? 'No matching projects found.' : 'No ongoing projects yet.'}
                </div>
              ) : (
                <div className="home-list">
                  {filteredProjects.map((project) => (
                    <article key={project.id} className="home-list-item">
                      <div className="home-list-leading">
                        <HomeIcon name="folder" />
                        <div>
                          <div className="home-list-title">{project.name}</div>
                          {project.categoryName && (
                            <div className="home-list-meta">{project.categoryName}</div>
                          )}
                          {project.documentCount > 0 && (
                            <div className="home-list-meta">{project.documentCount} docs</div>
                          )}
                        </div>
                      </div>
                      <div className="home-list-actions">
                        <span className="home-chevron">⌄</span>
                        <span className="home-kebab">⋮</span>
                      </div>
                    </article>
                  ))}
                </div>
              )}
            </section>

            <section className="home-section">
              <div className="home-section-header">
                <div>
                  <h2 className="home-section-title">Upcoming Milestones</h2>
                  <p className="home-section-subtitle">Your next milestones to follow your project</p>
                </div>
              </div>

              {filteredMilestones.length === 0 ? (
                <div className="home-empty-card">
                  {normalizedSearch ? 'No matching milestones found.' : 'No upcoming milestones yet.'}
                </div>
              ) : (
                filteredMilestones.map((milestone) => (
                  <article key={milestone.id} className="home-card home-milestone-card">
                    <h3 className="home-milestone-title">{milestone.title}</h3>
                    <div className="home-milestone-project">
                      <HomeIcon name="folder" />
                      <span>{milestone.projectName}</span>
                    </div>
                    <div className="home-milestone-details">
                      <div className="home-detail-item">
                        <HomeIcon name="calendar" />
                        <span>{formatDate(milestone.dueDate)}</span>
                      </div>
                      <div className="home-detail-item">
                        <HomeIcon name="coin" />
                        <span>
                          {formatAmount(
                            milestone.amount,
                            milestone.currency,
                            milestone.progressPercentage,
                          )}
                        </span>
                      </div>
                      <button type="button" className="home-edit-btn" aria-label="Edit milestone">
                        ✎
                      </button>
                    </div>
                    <button type="button" className="home-primary-btn">
                      View Details
                    </button>
                  </article>
                ))
              )}
            </section>

            <section className="home-section home-section-last">
              <div className="home-section-header">
                <div>
                  <h2 className="home-section-title">Payment Alerts</h2>
                  <p className="home-section-subtitle">You don&apos;t miss your next payment</p>
                </div>
              </div>

              {filteredPaymentAlerts.length === 0 ? (
                <div className="home-empty-card">
                  {normalizedSearch ? 'No matching payment alerts found.' : 'No payment alerts yet.'}
                </div>
              ) : (
                filteredPaymentAlerts.map((alert) => (
                  <article key={alert.id} className="home-card home-payment-card">
                    <div className="home-payment-top">
                      <span className="home-payment-badge">Payment</span>
                      <span className="home-payment-due">
                        {alert.status === 'Next' && <strong>Next </strong>}
                        {formatDate(alert.dueDate)}
                      </span>
                    </div>
                    <div className="home-payment-title">{alert.title}</div>
                    <div className="home-detail-item">
                      <HomeIcon name="coin" />
                      <span>
                        {formatAmount(alert.amount, alert.currency, alert.progressPercentage)}
                      </span>
                    </div>
                  </article>
                ))
              )}
            </section>
          </>
        )}

        <nav className="home-bottom-nav" aria-label="Main navigation">
          <button type="button" className="home-nav-item home-nav-item-active">
            <HomeIcon name="home" />
            <span>Home</span>
          </button>
          <button type="button" className="home-nav-item home-nav-item-disabled" disabled>
            <HomeIcon name="documents" />
            <span>My Documents</span>
          </button>
          <button type="button" className="home-nav-item home-nav-item-disabled" disabled>
            <HomeIcon name="subscription" />
            <span>Subscription</span>
          </button>
        </nav>
      </div>
    </div>
  );
}
