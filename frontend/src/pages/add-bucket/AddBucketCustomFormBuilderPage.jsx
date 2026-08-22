import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout from '../../components/add-bucket/AddBucketLayout';
import CreateFieldModal from '../../components/add-bucket/CreateFieldModal';
import { useAddBucket } from '../../context/AddBucketContext';
import { addCategoryFormField, getCategoryFormFields } from '../../services/bucketService';
import { clearAuth, isAuthenticated } from '../../utils/storage';

export default function AddBucketCustomFormBuilderPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, updateDraft } = useAddBucket();
  const [apiError, setApiError] = useState('');
  const [loading, setLoading] = useState(true);
  const [savingField, setSavingField] = useState(false);
  const [showFieldModal, setShowFieldModal] = useState(false);
  const [fields, setFields] = useState([]);

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
      setApiError('');
      try {
        const response = await getCategoryFormFields(draft.categoryId);
        setFields(response.data || []);
      } catch (error) {
        if (error.status === 401) {
          clearAuth();
          navigate('/login', { replace: true });
          return;
        }
        setApiError(error.message || 'Unable to load category fields.');
      } finally {
        setLoading(false);
      }
    }

    loadFields();
  }, [draft.categoryId, navigate]);

  async function handleAddField(fieldPayload) {
    setSavingField(true);
    setApiError('');
    try {
      const response = await addCategoryFormField(draft.categoryId, fieldPayload);
      const nextFields = [...fields, response.data];
      setFields(nextFields);
      updateDraft({ custom: { ...draft.custom, fields: nextFields } });
      setShowFieldModal(false);
    } catch (error) {
      setApiError(error.message || 'Unable to add field.');
    } finally {
      setSavingField(false);
    }
  }

  function handleContinue() {
    if (!draft.name.trim() || !draft.description.trim() || !draft.initialDocument.fileToken) {
      setApiError('Complete bucket name, information, and document upload on step 1 before continuing.');
      navigate('/add-bucket');
      return;
    }

    if (fields.length === 0) {
      setApiError('Add at least one field before continuing.');
      return;
    }
    navigate('/add-bucket/custom/submit');
  }

  return (
    <>
      <AddBucketLayout
        title="Create the form"
        subtitle="To use in this category and create new Bucket"
        step={2}
        totalSteps={3}
      >
        <div className="add-bucket-custom-category-banner">
          <span className="add-bucket-custom-category-icon">⊞+</span>
          <span>New Category: {selectedCategory?.name || 'Custom'}</span>
        </div>

        {loading && <div className="add-bucket-loading">Loading form fields...</div>}
        {!loading && <AlertMessage message={apiError} />}

        {!loading && (
          <div className="add-bucket-card add-bucket-form-card">
            <button type="button" className="add-bucket-inline-add mb-3" onClick={() => setShowFieldModal(true)}>
              + Create field
            </button>

            {fields.length > 0 && (
              <div className="add-bucket-custom-field-list">
                {fields.map((field) => (
                  <div key={field.id} className="add-bucket-custom-field-item">
                    <strong>{field.label}</strong>
                    <span>{field.fieldType}</span>
                    {field.isRequired && <em>Required</em>}
                  </div>
                ))}
              </div>
            )}

            <LoadingButton type="button" onClick={handleContinue}>
              Continue
            </LoadingButton>
          </div>
        )}
      </AddBucketLayout>

      <CreateFieldModal
        open={showFieldModal}
        onClose={() => setShowFieldModal(false)}
        onSubmit={handleAddField}
        loading={savingField}
      />
    </>
  );
}
