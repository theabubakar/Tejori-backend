import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import FormField from '../../components/FormField';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard, FileUploadField } from '../../components/add-bucket/AddBucketLayout';
import AddBucketSuccessModal from '../../components/add-bucket/AddBucketSuccessModal';
import {
  buildCreateBucketPayload,
  defaultDocument,
  useAddBucket,
} from '../../context/AddBucketContext';
import { createBucket, uploadBucketFile } from '../../services/bucketService';
import { clearAuth } from '../../utils/storage';
import { useAsyncSubmit } from '../../utils/useAsyncSubmit';

export default function AddBucketContractDocumentsPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, updateDraft, successResult, setSuccessResult, resetDraft, clearSuccess } =
    useAddBucket();
  const [apiError, setApiError] = useState('');
  const { submitting, runSubmit } = useAsyncSubmit();
  const [uploadingIndex, setUploadingIndex] = useState(null);

  function updateDocument(index, partial) {
    const documents = [...draft.documents];
    documents[index] = { ...documents[index], ...partial };
    updateDraft({ documents });
  }

  async function handleUpload(index, event) {
    const file = event.target.files?.[0];
    if (!file) return;

    setUploadingIndex(index);
    setApiError('');
    try {
      const response = await uploadBucketFile(file);
      updateDocument(index, {
        fileToken: response.data.fileToken,
        fileName: response.data.fileName,
      });
    } catch (error) {
      setApiError(error.message || 'Unable to upload file.');
    } finally {
      setUploadingIndex(null);
    }
  }

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
      <AddBucketLayout title="About documents" subtitle="Add more document to your contract" step={3}>
        <BucketSummaryCard draft={draft} selectedCategory={selectedCategory} />
        <AlertMessage message={apiError} />

        <form className="add-bucket-card add-bucket-form-card" onSubmit={handleSubmit} noValidate>
          {draft.documents.map((document, index) => (
            <div key={`document-${index}`} className="add-bucket-repeat-block">
              <div className="add-bucket-repeat-label">{index + 1} Document</div>
              <FormField
                label="Type of document"
                name={`document-type-${index}`}
                value={document.documentType}
                onChange={(e) => updateDocument(index, { documentType: e.target.value })}
                placeholder="Sponsorship of the Contract"
              />
              <FileUploadField
                label="Upload the document"
                fileName={document.fileName}
                uploading={uploadingIndex === index}
                onChange={(e) => handleUpload(index, e)}
              />
              <FormField label="Start Date" name={`start-${index}`} type="date" value={document.startDate} onChange={(e) => updateDocument(index, { startDate: e.target.value })} />
              <FormField label="End date" name={`end-${index}`} type="date" value={document.endDate} onChange={(e) => updateDocument(index, { endDate: e.target.value })} />
              <FormField label="Extension Date" name={`extension-${index}`} type="date" value={document.extensionDate} onChange={(e) => updateDocument(index, { extensionDate: e.target.value })} />
            </div>
          ))}

          <button
            type="button"
            className="add-bucket-inline-add mb-3"
            onClick={() => updateDraft({ documents: [...draft.documents, { ...defaultDocument }] })}
          >
            + Add more document
          </button>

          <FormField label="Remarks" name="remarks">
            <textarea
              id="remarks"
              name="remarks"
              className="tijori-input add-bucket-textarea"
              value={draft.remarks}
              onChange={(event) => updateDraft({ remarks: event.target.value })}
              rows={3}
            />
          </FormField>

          <LoadingButton type="submit" loading={submitting}>
            Add My Bucket
          </LoadingButton>
        </form>
      </AddBucketLayout>

      <AddBucketSuccessModal result={successResult} onClose={clearSuccess} />
    </>
  );
}
