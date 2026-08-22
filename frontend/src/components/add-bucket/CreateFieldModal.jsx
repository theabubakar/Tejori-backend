import { useState } from 'react';
import FormField from '../FormField';
import LoadingButton from '../LoadingButton';

const FIELD_TYPES = [
  { value: 'text', label: 'Text' },
  { value: 'textarea', label: 'Text area' },
  { value: 'date', label: 'Date' },
  { value: 'select', label: 'Select box' },
  { value: 'file', label: 'File upload' },
];

export default function CreateFieldModal({ open, onClose, onSubmit, loading }) {
  const [label, setLabel] = useState('');
  const [fieldType, setFieldType] = useState('text');
  const [required, setRequired] = useState(false);
  const [error, setError] = useState('');

  if (!open) return null;

  async function handleSubmit(event) {
    event.preventDefault();
    if (!label.trim()) {
      setError('Label is required.');
      return;
    }

    setError('');
    const fieldKey = label
      .trim()
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, '_')
      .replace(/^_|_$/g, '');

    await onSubmit({
      label: label.trim(),
      fieldKey: fieldKey || `field_${Date.now()}`,
      fieldType,
      isRequired: required,
    });

    setLabel('');
    setFieldType('text');
    setRequired(false);
  }

  function handleClose() {
    setLabel('');
    setFieldType('text');
    setRequired(false);
    setError('');
    onClose();
  }

  return (
    <div className="home-modal-backdrop" role="presentation" onClick={handleClose}>
      <div
        className="add-category-modal add-field-modal"
        role="dialog"
        aria-modal="true"
        onClick={(event) => event.stopPropagation()}
      >
        <button type="button" className="home-modal-close" onClick={handleClose} aria-label="Close">
          ×
        </button>
        <h2 className="add-category-modal-title">Create New Field</h2>
        {error && <div className="tijori-field-error mb-2">{error}</div>}
        <form onSubmit={handleSubmit} noValidate>
          <FormField
            label="Label"
            name="fieldLabel"
            value={label}
            onChange={(event) => setLabel(event.target.value)}
            placeholder="Add a label to indicate what is the field for"
            required
          />
          <FormField label="Type of the field" name="fieldType" required>
            <select
              id="fieldType"
              name="fieldType"
              className="tijori-select w-100"
              value={fieldType}
              onChange={(event) => setFieldType(event.target.value)}
            >
              {FIELD_TYPES.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </FormField>
          <label className="add-bucket-checkbox mb-3">
            <input
              type="checkbox"
              checked={required}
              onChange={(event) => setRequired(event.target.checked)}
            />
            <span>Required field</span>
          </label>
          <LoadingButton type="submit" loading={loading} variant="dark">
            Add New Field
          </LoadingButton>
        </form>
      </div>
    </div>
  );
}
