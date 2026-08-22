import { Link } from 'react-router-dom';
import CategoryIcon from './CategoryIcon';
import { isCustomGroupCategory } from '../../context/AddBucketContext';

export default function AddBucketLayout({ title, subtitle, step, totalSteps = 3, children }) {
  return (
    <div className="add-bucket-page">
      <div className="add-bucket-shell">
        <div className="add-bucket-topbar">
          <Link to="/home" className="add-bucket-back">
            ←
          </Link>
          <div className="add-bucket-topbar-title">Create bucket</div>
          <div className="add-bucket-topbar-spacer" />
        </div>

        <div className="add-bucket-card add-bucket-intro-card">
          <div className="add-bucket-intro-icon">📁</div>
          <div>
            <div className="add-bucket-intro-label">New Bucket</div>
            <h1 className="add-bucket-title">{title}</h1>
            {subtitle && <p className="add-bucket-subtitle">{subtitle}</p>}
          </div>
          {step && (
            <div className="add-bucket-step-badge">
              {step}/{totalSteps}
            </div>
          )}
        </div>

        {children}
      </div>
    </div>
  );
}

export function BucketSummaryCard({ draft, selectedCategory, warrantySubCategoryName, selectedSubCategoryName }) {
  const typeLabel = isCustomGroupCategory(selectedCategory)
    ? 'Custom'
    : selectedCategory?.name || '—';

  return (
    <div className="add-bucket-summary-card">
      <div><strong>Bucket:</strong> {draft.name || '—'}</div>
      <div><strong>Type:</strong> {typeLabel}</div>
      {selectedSubCategoryName && (
        <div><strong>Category:</strong> {selectedSubCategoryName}</div>
      )}
      {warrantySubCategoryName && (
        <div><strong>Category:</strong> {warrantySubCategoryName}</div>
      )}
    </div>
  );
}

export function CategoryCardGrid({ categories, selectedId, onSelect }) {
  return (
    <div className="add-bucket-category-grid">
      {categories.map((category) => (
        <button
          key={category.id}
          type="button"
          className={`add-bucket-category-card ${
            selectedId === category.id ? 'add-bucket-category-card-selected' : ''
          }`}
          onClick={() => onSelect(category.id)}
        >
          <span className="add-bucket-category-icon">
            <CategoryIcon iconKey={category.iconKey} />
          </span>
          <span>{category.name.toUpperCase()}</span>
        </button>
      ))}
    </div>
  );
}

export function FileUploadField({ label, required = false, fileName, onChange, uploading, error }) {
  return (
    <div className="mb-3">
      <label className="tijori-label">
        {label}
        {required && <span className="tijori-required"> *</span>}
      </label>
      <label className="add-bucket-upload">
        <input type="file" className="visually-hidden" onChange={onChange} disabled={uploading} />
        <span className="add-bucket-upload-icon">↑</span>
        <span>{uploading ? 'Uploading...' : fileName || 'Upload the file (PDF, Word...)'}</span>
      </label>
      {error && <div className="tijori-field-error">{error}</div>}
    </div>
  );
}
