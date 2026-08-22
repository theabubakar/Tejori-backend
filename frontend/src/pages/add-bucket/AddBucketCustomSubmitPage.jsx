import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import FormField from '../../components/FormField';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard, FileUploadField } from '../../components/add-bucket/AddBucketLayout';
import AddBucketSuccessModal from '../../components/add-bucket/AddBucketSuccessModal';
import { buildCreateBucketPayload, useAddBucket } from '../../context/AddBucketContext';
import { createBucket, getCategoryFormFields, uploadBucketFile } from '../../services/bucketService';
import { clearAuth, isAuthenticated } from '../../utils/storage';
import { useAsyncSubmit } from '../../utils/useAsyncSubmit';

export default function AddBucketCustomSubmitPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, updateDraft, successResult, setSuccessResult, resetDraft, clearSuccess } =
    useAddBucket();
  const [fields, setFields] = useState([]);
  const [apiError, setApiError] = useState('');
  const [loading, setLoading] = useState(true);
  const [uploadingFieldId, setUploadingFieldId] = useState(null);
  const { submitting, runSubmit } = useAsyncSubmit();

  useEffect(() => {
    if (!isAuthenticated()) {
      navigate('/login', { replace: true });
      return;
    }

    if (!draft.categoryId) {
      navigate('/add-bucket', { replace: true });
      return;
    }

    async function loadFields() {
      setLoading(true);
      try {
        const response = await getCategoryFormFields(draft.categoryId);
        setFields(response.data || []);
      } catch (error) {
        if (error.status === 401) {
          clearAuth();
          navigate('/login', { replace: true });
          return;
        }
        setApiError(error.message || 'Unable to load form fields.');
      } finally {
        setLoading(false);
      }
    }

    loadFields();
  }, [draft.categoryId, navigate]);

  function updateValue(fieldId, partial) {
    updateDraft({
      custom: {
        ...draft.custom,
        values: {
          ...draft.custom.values,
          [fieldId]: {
            ...(draft.custom.values[fieldId] || {}),
            ...partial,
          },
        },
      },
    });
  }

  async function handleFileUpload(fieldId, event) {
    const file = event.target.files?.[0];
    if (!file) return;

    setUploadingFieldId(fieldId);
    setApiError('');
    try {
      const response = await uploadBucketFile(file);
      updateValue(fieldId, {
        fileToken: response.data.fileToken,
        fileName: response.data.fileName,
      });
    } catch (error) {
      setApiError(error.message || 'Unable to upload file.');
    } finally {
      setUploadingFieldId(null);
    }
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setApiError('');

    if (!draft.name.trim() || !draft.description.trim() || !draft.initialDocument.fileToken) {
      setApiError('Complete bucket name, information, and document upload on step 1 before submitting.');
      navigate('/add-bucket');
      return;
    }

    await runSubmit(async () => {
      try {
        const payload = buildCreateBucketPayload(draft, selectedCategory);
        const response = await createBucket(payload);
        setSuccessResult(response.data);
        resetDraft();
      } catch (error) {
        if (error.status === 401) {
          clearAuth();
          navigate('/login', { replace: true });
          return;
        }
        setApiError(error.message || 'Unable to create bucket.');
      }
    });
  }

  function renderField(field) {
    const value = draft.custom.values[field.id] || {};

    if (field.fieldType === 'textarea') {
      return (
        <FormField key={field.id} label={field.label} name={field.id} required={field.isRequired}>
          <textarea
            id={field.id}
            className="tijori-input add-bucket-textarea"
            rows={3}
            value={value.text || ''}
            onChange={(event) => updateValue(field.id, { text: event.target.value })}
          />
        </FormField>
      );
    }

    if (field.fieldType === 'date') {
      return (
        <FormField
          key={field.id}
          label={field.label}
          name={field.id}
          type="date"
          value={value.text || ''}
          onChange={(event) => updateValue(field.id, { text: event.target.value })}
          required={field.isRequired}
        />
      );
    }

    if (field.fieldType === 'select') {
      let options = [];
      try {
        options = field.optionsJson ? JSON.parse(field.optionsJson) : [];
      } catch {
        options = [];
      }
      return (
        <FormField key={field.id} label={field.label} name={field.id} required={field.isRequired}>
          <select
            id={field.id}
            className="tijori-select w-100"
            value={value.text || ''}
            onChange={(event) => updateValue(field.id, { text: event.target.value })}
          >
            <option value="">Select an option</option>
            {options.map((option) => (
              <option key={option} value={option}>
                {option}
              </option>
            ))}
          </select>
        </FormField>
      );
    }

    if (field.fieldType === 'file') {
      return (
        <FileUploadField
          key={field.id}
          label={field.label}
          required={field.isRequired}
          fileName={value.fileName}
          uploading={uploadingFieldId === field.id}
          onChange={(event) => handleFileUpload(field.id, event)}
        />
      );
    }

    return (
      <FormField
        key={field.id}
        label={field.label}
        name={field.id}
        value={value.text || ''}
        onChange={(event) => updateValue(field.id, { text: event.target.value })}
        required={field.isRequired}
      />
    );
  }

  return (
    <>
      <AddBucketLayout title="Fill your bucket" subtitle="Complete the custom fields for this bucket." step={3}>
        <BucketSummaryCard draft={draft} selectedCategory={selectedCategory} />
        {loading && <div className="add-bucket-loading">Loading form...</div>}
        {!loading && <AlertMessage message={apiError} />}

        {!loading && (
          <form className="add-bucket-card add-bucket-form-card" onSubmit={handleSubmit} noValidate>
            {fields.map((field) => renderField(field))}
            <LoadingButton type="submit" loading={submitting}>
              Add My Bucket
            </LoadingButton>
          </form>
        )}
      </AddBucketLayout>

      <AddBucketSuccessModal result={successResult} onClose={clearSuccess} />
    </>
  );
}
