import { Link } from 'react-router-dom';

const SUCCESS_ICONS = {
  trip: '🗺️',
  appointment: '📅',
  medicine: '💊',
  custom: '⊞',
  warranty: '✓',
  contract: '✓',
  bucket: '✓',
};

export default function AddBucketSuccessModal({ result, onClose }) {
  if (!result) return null;

  const icon = SUCCESS_ICONS[result.flowType] || SUCCESS_ICONS.bucket;

  return (
    <div className="home-modal-backdrop" role="presentation">
      <div className="add-bucket-success-card" role="dialog" aria-modal="true">
        <button type="button" className="home-modal-close" onClick={onClose} aria-label="Close">
          ×
        </button>
        <div className="add-bucket-success-icon">{icon}</div>
        <h2 className="add-bucket-success-title">{result.successTitle}</h2>
        <p className="add-bucket-success-message">{result.successMessage}</p>
        <Link to="/home" className="text-decoration-none d-block" onClick={onClose}>
          <span className="add-bucket-success-btn">Go Back To My Homepage</span>
        </Link>
      </div>
    </div>
  );
}
