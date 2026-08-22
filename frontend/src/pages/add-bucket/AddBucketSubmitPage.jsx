import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard } from '../../components/add-bucket/AddBucketLayout';
import AddBucketSuccessModal from '../../components/add-bucket/AddBucketSuccessModal';
import { buildCreateBucketPayload, useAddBucket } from '../../context/AddBucketContext';
import { createBucket } from '../../services/bucketService';
import { clearAuth } from '../../utils/storage';
import { useAsyncSubmit } from '../../utils/useAsyncSubmit';

export default function AddBucketSubmitPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, successResult, setSuccessResult, resetDraft, clearSuccess } = useAddBucket();
  const [apiError, setApiError] = useState('');
  const { submitting, runSubmit } = useAsyncSubmit();

  async function handleSubmit(event) {
    event.preventDefault();
    setApiError('');

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

  return (
    <>
      <AddBucketLayout title="Review & Create" subtitle="Confirm your bucket details before creating.">
        <BucketSummaryCard draft={draft} selectedCategory={selectedCategory} />
        <AlertMessage message={apiError} />

        <form className="add-bucket-card add-bucket-form-card" onSubmit={handleSubmit}>
          <p className="add-bucket-help-text mb-3">
            Your bucket will be created with the information entered in the previous step.
          </p>
          <LoadingButton type="submit" loading={submitting}>
            Add My Bucket
          </LoadingButton>
        </form>
      </AddBucketLayout>

      <AddBucketSuccessModal result={successResult} onClose={clearSuccess} />
    </>
  );
}
