import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import FormField from '../../components/FormField';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard } from '../../components/add-bucket/AddBucketLayout';
import AddBucketSuccessModal from '../../components/add-bucket/AddBucketSuccessModal';
import {
  buildCreateBucketPayload,
  useAddBucket,
} from '../../context/AddBucketContext';
import { createBucket } from '../../services/bucketService';
import { clearAuth } from '../../utils/storage';
import { useAsyncSubmit } from '../../utils/useAsyncSubmit';

export default function AddBucketWarrantyDetailsPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, setup, updateDraft, successResult, setSuccessResult, resetDraft, clearSuccess } =
    useAddBucket();
  const [errors, setErrors] = useState({});
  const [apiError, setApiError] = useState('');
  const { submitting, runSubmit } = useAsyncSubmit();
  const warranty = draft.warranty;

  const warrantySubCategoryName = setup.warrantySubCategories.find(
    (item) => item.key === draft.warrantySubCategoryKey,
  )?.name;

  function updateWarranty(partial) {
    updateDraft({ warranty: { ...warranty, ...partial } });
  }

  function validate() {
    const nextErrors = {};
    if (!warranty.brandName.trim()) nextErrors.brandName = 'Brand name is required.';
    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  }

  async function handleSubmit(event) {
    event.preventDefault();
    if (!validate()) return;

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
        setApiError(error.message || 'Unable to create warranty bucket.');
      }
    });
  }

  return (
    <>
      <AddBucketLayout title="About Warranty" subtitle="Fill out below the informations about the warranty" step={3}>
        <BucketSummaryCard
          draft={draft}
          selectedCategory={selectedCategory}
          warrantySubCategoryName={warrantySubCategoryName}
        />
        <AlertMessage message={apiError} />

        <form className="add-bucket-card add-bucket-form-card" onSubmit={handleSubmit} noValidate>
          <section className="add-bucket-form-section">
            <h3 className="add-bucket-form-section-title">Details of warranty</h3>
            <FormField label="Name of Brand" name="brandName" value={warranty.brandName} onChange={(e) => updateWarranty({ brandName: e.target.value })} placeholder="Enter the Name of Brand" required error={errors.brandName} />
            <FormField label="Price" name="price" value={warranty.price} onChange={(e) => updateWarranty({ price: e.target.value })} placeholder="Enter a Price" />
            <FormField label="Serial Number" name="serialNumber" value={warranty.serialNumber} onChange={(e) => updateWarranty({ serialNumber: e.target.value })} placeholder="Serial number (Optional)" />
            <FormField label="Seller Name" name="sellerName" value={warranty.sellerName} onChange={(e) => updateWarranty({ sellerName: e.target.value })} placeholder="Enter the Seller Name" />
            <div className="tijori-phone-row mb-3">
              <select className="tijori-select" value={warranty.sellerPhoneCountryCode} onChange={(e) => updateWarranty({ sellerPhoneCountryCode: e.target.value })}>
                <option value="+965">+965</option>
                <option value="+92">+92</option>
              </select>
              <input className="tijori-input" value={warranty.sellerPhone} onChange={(e) => updateWarranty({ sellerPhone: e.target.value })} placeholder="Seller Number" />
            </div>
            <FormField label="Start date of the warranty" name="startDate" type="date" value={warranty.startDate} onChange={(e) => updateWarranty({ startDate: e.target.value })} />
            <FormField label="Expiry date of the warranty" name="expiryDate" type="date" value={warranty.expiryDate} onChange={(e) => updateWarranty({ expiryDate: e.target.value })} />
            <FormField label="Purchase location" name="purchaseLocation" value={warranty.purchaseLocation} onChange={(e) => updateWarranty({ purchaseLocation: e.target.value })} placeholder="Kuwait City" />
            <FormField label="Country of manufacture" name="countryOfManufacture" value={warranty.countryOfManufacture} onChange={(e) => updateWarranty({ countryOfManufacture: e.target.value })} placeholder="Kuwait" />
            <FormField label="Store location" name="storeLocationUrl" value={warranty.storeLocationUrl} onChange={(e) => updateWarranty({ storeLocationUrl: e.target.value })} placeholder="Https://link.com..." />
            <label className="tijori-checkbox-label mb-2">
              <input type="checkbox" checked={warranty.expiryReminderEnabled} onChange={(e) => updateWarranty({ expiryReminderEnabled: e.target.checked })} />
              <span>Add reminder for expiry date</span>
            </label>
            {warranty.expiryReminderEnabled && (
              <FormField label="Expiry Reminder" name="expiryReminderTiming" value={warranty.expiryReminderTiming} onChange={(e) => updateWarranty({ expiryReminderTiming: e.target.value })} />
            )}
          </section>

          <section className="add-bucket-form-section">
            <h3 className="add-bucket-form-section-title">Warranty covering</h3>
            {warranty.coverages.map((coverage, index) => (
              <div key={`coverage-${index}`} className="add-bucket-repeat-block">
                <FormField label="Covering areas" name={`coverage-area-${index}`} value={coverage.coverageArea} onChange={(e) => {
                  const coverages = [...warranty.coverages];
                  coverages[index] = { ...coverages[index], coverageArea: e.target.value };
                  updateWarranty({ coverages });
                }} placeholder="Number of covering areas" />
                <FormField label="Choose an option" name={`coverage-option-${index}`} value={coverage.coverageOption} onChange={(e) => {
                  const coverages = [...warranty.coverages];
                  coverages[index] = { ...coverages[index], coverageOption: e.target.value };
                  updateWarranty({ coverages });
                }} placeholder="Kuwait City" />
              </div>
            ))}
            <button
              type="button"
              className="add-bucket-inline-add mb-3"
              onClick={() => updateWarranty({ coverages: [...warranty.coverages, { coverageArea: '', coverageOption: '' }] })}
            >
              + Add more
            </button>
          </section>

          <LoadingButton type="submit" loading={submitting}>
            Add My Bucket
          </LoadingButton>
        </form>
      </AddBucketLayout>

      <AddBucketSuccessModal result={successResult} onClose={clearSuccess} />
    </>
  );
}
