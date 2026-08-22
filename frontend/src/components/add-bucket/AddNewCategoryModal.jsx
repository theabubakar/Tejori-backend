import { useState } from 'react';
import AlertMessage from '../AlertMessage';
import LoadingButton from '../LoadingButton';

export default function AddNewCategoryModal({ open, onClose, onSubmit, loading }) {
  const [name, setName] = useState('');
  const [error, setError] = useState('');

  if (!open) return null;

  async function handleSubmit(event) {
    event.preventDefault();
    if (!name.trim()) {
      setError('Category name is required.');
      return;
    }

    setError('');
    await onSubmit(name.trim());
    setName('');
  }

  function handleClose() {
    setName('');
    setError('');
    onClose();
  }

  return (
    <div className="home-modal-backdrop" role="presentation" onClick={handleClose}>
      <div
        className="add-category-modal"
        role="dialog"
        aria-modal="true"
        onClick={(event) => event.stopPropagation()}
      >
        <button type="button" className="home-modal-close" onClick={handleClose} aria-label="Close">
          ×
        </button>
        <div className="add-category-modal-icon">⊞+</div>
        <h2 className="add-category-modal-title">Add a New Category</h2>
        <p className="add-category-modal-text">
          Add a New Category to your list and manage better your Buckets
        </p>
        <AlertMessage message={error} />
        <form onSubmit={handleSubmit} noValidate>
          <input
            className="tijori-input mb-3"
            value={name}
            onChange={(event) => setName(event.target.value)}
            placeholder="Name the new category"
            autoFocus
          />
          <LoadingButton type="submit" loading={loading} variant="dark">
            ADD NEW CATEGORY
          </LoadingButton>
        </form>
      </div>
    </div>
  );
}
