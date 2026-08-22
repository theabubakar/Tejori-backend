import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import FormField from '../../components/FormField';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, {
  CategoryCardGrid,
  FileUploadField,
} from '../../components/add-bucket/AddBucketLayout';
import {
  getCustomGroupCategory,
  isContractCategory,
  isCustomGroupCategory,
  isCustomSubCategory,
  isMyAppointmentsCategory,
  isMyMedicineCategory,
  isMyTripsCategory,
  isWarrantyCategory,
  useAddBucket,
} from '../../context/AddBucketContext';
import { addCustomCategory, getBucketSetup, uploadBucketFile } from '../../services/bucketService';
import { isAuthenticated } from '../../utils/storage';
import AddNewCategoryModal from '../../components/add-bucket/AddNewCategoryModal';

export default function AddBucketStep1Page() {
  const navigate = useNavigate();
  const { draft, setup, updateDraft, setSetup } = useAddBucket();
  const [errors, setErrors] = useState({});
  const [apiError, setApiError] = useState('');
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [addingCategory, setAddingCategory] = useState(false);
  const [showCategoryModal, setShowCategoryModal] = useState(false);

  useEffect(() => {
    if (!isAuthenticated()) {
      navigate('/login', { replace: true });
      return;
    }

    async function loadSetup() {
      setLoading(true);
      setApiError('');
      try {
        const response = await getBucketSetup();
        setSetup(response.data);
      } catch (error) {
        if (error.status === 401) {
          navigate('/login', { replace: true });
          return;
        }
        setApiError(error.message || 'Unable to load bucket categories.');
      } finally {
        setLoading(false);
      }
    }

    loadSetup();
  }, [navigate, setSetup]);

  function validate() {
    const nextErrors = {};
    if (!draft.name.trim()) nextErrors.name = 'Bucket name is required.';
    if (!draft.description.trim()) nextErrors.description = 'Bucket information is required.';
    if (!draft.categoryId) nextErrors.categoryId = 'Select a category.';
    if (!draft.initialDocument.fileToken) {
      nextErrors.document = 'Please upload a document before continuing.';
    }
    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  }

  function handleNext(event) {
    event.preventDefault();
    if (!validate()) return;

    const category =
      setup.categories.find((item) => item.id === draft.categoryId) || getCustomGroupCategory(setup);

    if (isCustomGroupCategory(category)) {
      updateDraft({
        customGroupCategoryId: category.id,
        categoryId: category.id,
        pendingCustomCategory: null,
      });
      navigate('/add-bucket/custom/categories');
      return;
    }
    if (isContractCategory(category)) {
      navigate('/add-bucket/contract/details');
      return;
    }
    if (isWarrantyCategory(category)) {
      navigate('/add-bucket/warranty/category');
      return;
    }
    if (isMyTripsCategory(category)) {
      navigate('/add-bucket/trip/details');
      return;
    }
    if (isMyAppointmentsCategory(category)) {
      navigate('/add-bucket/appointment/details');
      return;
    }
    if (isMyMedicineCategory(category)) {
      navigate('/add-bucket/medicine/details');
      return;
    }
    if (isCustomSubCategory(category)) {
      navigate('/add-bucket/custom/form');
      return;
    }
    navigate('/add-bucket/submit');
  }

  async function handleInitialUpload(event) {
    const file = event.target.files?.[0];
    if (!file) return;

    setUploading(true);
    setApiError('');
    setErrors((current) => ({ ...current, document: undefined }));
    try {
      const response = await uploadBucketFile(file);
      updateDraft({
        initialDocument: {
          ...draft.initialDocument,
          fileToken: response.data.fileToken,
          fileName: response.data.fileName,
          documentType: draft.initialDocument.documentType || 'Initial Document',
        },
      });
    } catch (error) {
      setApiError(error.message || 'Unable to upload file.');
    } finally {
      setUploading(false);
    }
  }

  async function handleAddCategory(name) {
    setAddingCategory(true);
    setApiError('');
    try {
      const response = await addCustomCategory(name);
      const setupResponse = await getBucketSetup();
      setSetup(setupResponse.data);
      updateDraft({
        categoryId: response.data.id,
        pendingCustomCategory: response.data,
        customGroupCategoryId: setupResponse.data.customGroupCategoryId || '',
      });
      setShowCategoryModal(false);
      navigate('/add-bucket/custom/form');
    } catch (error) {
      setApiError(error.message || 'Unable to add category.');
    } finally {
      setAddingCategory(false);
    }
  }

  return (
    <AddBucketLayout title="Add Bucket" subtitle="Add a Bucket to manage" step={1}>
      {loading && <div className="add-bucket-loading">Loading categories...</div>}
      {!loading && <AlertMessage message={apiError} />}

      {!loading && (
        <form className="add-bucket-card add-bucket-form-card" onSubmit={handleNext} noValidate>
          <FormField
            label="Enter a Name"
            name="name"
            value={draft.name}
            onChange={(event) => updateDraft({ name: event.target.value })}
            placeholder="Enter Bucket's Name"
            required
            error={errors.name}
          />

          <FormField label="Informations about the Bucket" name="description" required error={errors.description}>
            <textarea
              id="description"
              name="description"
              className="tijori-input add-bucket-textarea"
              value={draft.description}
              onChange={(event) => updateDraft({ description: event.target.value })}
              placeholder="Describe the Bucket in few words..."
              rows={4}
            />
          </FormField>

          <div className="add-bucket-section-header">
            <h2 className="add-bucket-section-title">What do you want to add to this Bucket?</h2>
            <button type="button" className="home-add-btn" onClick={() => setShowCategoryModal(true)}>
              + Add New Category
            </button>
          </div>

          {errors.categoryId && <div className="tijori-field-error mb-2">{errors.categoryId}</div>}

          <CategoryCardGrid
            categories={setup.categories}
            selectedId={draft.categoryId}
            onSelect={(categoryId) =>
              updateDraft({
                categoryId,
                pendingCustomCategory: null,
              })
            }
          />

          <FileUploadField
            label="Enter a document"
            required
            fileName={draft.initialDocument.fileName}
            onChange={handleInitialUpload}
            uploading={uploading}
            error={errors.document}
          />

          <LoadingButton type="submit">Next</LoadingButton>
        </form>
      )}

      <AddNewCategoryModal
        open={showCategoryModal}
        onClose={() => setShowCategoryModal(false)}
        onSubmit={handleAddCategory}
        loading={addingCategory}
      />
    </AddBucketLayout>
  );
}
