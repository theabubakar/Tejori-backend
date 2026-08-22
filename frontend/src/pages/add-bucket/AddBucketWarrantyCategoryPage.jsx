import { useNavigate } from 'react-router-dom';
import AddBucketLayout, { BucketSummaryCard, CategoryCardGrid } from '../../components/add-bucket/AddBucketLayout';
import LoadingButton from '../../components/LoadingButton';
import { useAddBucket } from '../../context/AddBucketContext';

export default function AddBucketWarrantyCategoryPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, setup, updateDraft } = useAddBucket();

  const warrantyOptions = setup.warrantySubCategories.map((item) => ({
    id: item.key,
    name: item.name,
    iconKey: item.key,
  }));

  const selectedOption = setup.warrantySubCategories.find(
    (item) => item.key === draft.warrantySubCategoryKey,
  );

  function handleNext(event) {
    event.preventDefault();
    if (!draft.warrantySubCategoryKey) return;
    navigate('/add-bucket/warranty/details');
  }

  return (
    <AddBucketLayout title="Category of Warranty" subtitle="Choose from below type of your contract" step={2}>
      <BucketSummaryCard draft={draft} selectedCategory={selectedCategory} />

      <form className="add-bucket-card add-bucket-form-card" onSubmit={handleNext}>
        <CategoryCardGrid
          categories={warrantyOptions}
          selectedId={draft.warrantySubCategoryKey}
          onSelect={(key) => updateDraft({ warrantySubCategoryKey: key })}
        />

        {selectedOption && (
          <p className="add-bucket-help-text">{selectedOption.description}</p>
        )}

        <label className="tijori-checkbox-label mb-3">
          <input
            type="checkbox"
            checked={draft.scanWithAiOcr}
            onChange={(event) => updateDraft({ scanWithAiOcr: event.target.checked })}
          />
          <span>Scan Ai Ocr Proccess to extract informations.</span>
        </label>

        <LoadingButton type="submit" disabled={!draft.warrantySubCategoryKey}>
          Add The Contract
        </LoadingButton>
      </form>
    </AddBucketLayout>
  );
}
