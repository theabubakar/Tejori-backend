import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard } from '../../components/add-bucket/AddBucketLayout';
import AddNewCategoryModal from '../../components/add-bucket/AddNewCategoryModal';
import { getCustomGroupCategory, useAddBucket } from '../../context/AddBucketContext';
import {
  addCustomCategory,
  deleteCustomCategory,
  getBucketSetup,
  getCategoryFormFields,
} from '../../services/bucketService';
import { clearAuth, isAuthenticated } from '../../utils/storage';

export default function AddBucketCustomCategoriesPage() {
  const navigate = useNavigate();
  const { draft, setup, updateDraft, setSetup } = useAddBucket();
  const [apiError, setApiError] = useState('');
  const [loading, setLoading] = useState(true);
  const [addingCategory, setAddingCategory] = useState(false);
  const [continuing, setContinuing] = useState(false);
  const [showCategoryModal, setShowCategoryModal] = useState(false);
  const [selectedSubCategoryId, setSelectedSubCategoryId] = useState('');

  useEffect(() => {
    if (!isAuthenticated()) {
      navigate('/login', { replace: true });
      return;
    }

    if (!draft.name.trim() || !draft.description.trim() || !draft.initialDocument.fileToken) {
      navigate('/add-bucket', { replace: true });
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
          clearAuth();
          navigate('/login', { replace: true });
          return;
        }
        setApiError(error.message || 'Unable to load custom categories.');
      } finally {
        setLoading(false);
      }
    }

    loadSetup();
  }, [navigate, setSetup]);

  useEffect(() => {
    if (draft.categoryId && setup.customCategories?.some((item) => item.id === draft.categoryId)) {
      setSelectedSubCategoryId(draft.categoryId);
    }
  }, [draft.categoryId, setup.customCategories]);

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

  async function handleDeleteCategory(event, categoryId) {
    event.stopPropagation();
    setApiError('');
    try {
      await deleteCustomCategory(categoryId);
      const response = await getBucketSetup();
      setSetup(response.data);
      if (selectedSubCategoryId === categoryId) {
        setSelectedSubCategoryId('');
      }
    } catch (error) {
      setApiError(error.message || 'Unable to delete custom category.');
    }
  }

  async function handleContinue() {
    if (!selectedSubCategoryId) {
      setApiError('Select one of your custom categories to continue.');
      return;
    }

    const selected = setup.customCategories.find((item) => item.id === selectedSubCategoryId);
    if (!selected) {
      setApiError('Selected custom category is no longer available.');
      return;
    }

    setContinuing(true);
    setApiError('');
    try {
      updateDraft({
        categoryId: selected.id,
        pendingCustomCategory: selected,
        customGroupCategoryId: setup.customGroupCategoryId || '',
      });

      const response = await getCategoryFormFields(selected.id);
      const fields = response.data || [];
      if (fields.length === 0) {
        navigate('/add-bucket/custom/form');
        return;
      }
      navigate('/add-bucket/custom/submit');
    } catch (error) {
      setApiError(error.message || 'Unable to continue with the selected category.');
    } finally {
      setContinuing(false);
    }
  }

  const customGroup = getCustomGroupCategory(setup);
  const customCategories = setup.customCategories || [];

  return (
    <>
      <AddBucketLayout
        title="Category of Custom"
        subtitle="Choose from below type of your custom"
        step={2}
        totalSteps={3}
      >
        <BucketSummaryCard
          draft={draft}
          selectedCategory={customGroup || { name: 'Custom', iconKey: 'custom' }}
          selectedSubCategoryName={
            customCategories.find((item) => item.id === selectedSubCategoryId)?.name
          }
        />

        {loading && <div className="add-bucket-loading">Loading custom categories...</div>}
        {!loading && <AlertMessage message={apiError} />}

        {!loading && (
          <div className="add-bucket-card add-bucket-form-card">
            <div className="add-bucket-section-header">
              <p className="add-bucket-custom-list-help mb-0">
                Choose one of your custom categories with its own dynamic form.
              </p>
              <button type="button" className="home-add-btn" onClick={() => setShowCategoryModal(true)}>
                + Add New Category
              </button>
            </div>

            {customCategories.length === 0 ? (
              <p className="add-bucket-custom-list-empty">
                You have not created any custom categories yet. Use Add New Category to create one.
              </p>
            ) : (
              <div className="add-bucket-custom-category-grid">
                {customCategories.map((category) => (
                  <div
                    key={category.id}
                    className={`add-bucket-custom-category-card ${
                      selectedSubCategoryId === category.id ? 'is-selected' : ''
                    }`}
                    role="button"
                    tabIndex={0}
                    onClick={() => setSelectedSubCategoryId(category.id)}
                    onKeyDown={(event) => {
                      if (event.key === 'Enter' || event.key === ' ') {
                        event.preventDefault();
                        setSelectedSubCategoryId(category.id);
                      }
                    }}
                  >
                    {selectedSubCategoryId === category.id && (
                      <span className="add-bucket-custom-category-check">✓</span>
                    )}
                    <button
                      type="button"
                      className="add-bucket-custom-category-delete"
                      aria-label={`Delete ${category.name}`}
                      onClick={(event) => handleDeleteCategory(event, category.id)}
                    >
                      ×
                    </button>
                    <span className="add-bucket-custom-category-name">{category.name}</span>
                  </div>
                ))}
              </div>
            )}

            <LoadingButton type="button" loading={continuing} onClick={handleContinue}>
              Continue
            </LoadingButton>
          </div>
        )}
      </AddBucketLayout>

      <AddNewCategoryModal
        open={showCategoryModal}
        onClose={() => setShowCategoryModal(false)}
        onSubmit={handleAddCategory}
        loading={addingCategory}
      />
    </>
  );
}
