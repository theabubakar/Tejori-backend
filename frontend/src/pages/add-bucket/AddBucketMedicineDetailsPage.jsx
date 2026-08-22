import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import FormField from '../../components/FormField';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard, FileUploadField } from '../../components/add-bucket/AddBucketLayout';
import AddBucketSuccessModal from '../../components/add-bucket/AddBucketSuccessModal';
import {
  buildCreateBucketPayload,
  defaultMedicineItem,
  defaultPatientItem,
  defaultScheduleItem,
  useAddBucket,
} from '../../context/AddBucketContext';
import { createBucket, uploadBucketFile } from '../../services/bucketService';
import { clearAuth } from '../../utils/storage';
import { useAsyncSubmit } from '../../utils/useAsyncSubmit';

const REMINDER_OPTIONS = [
  'Remind me - One Day before',
  'Remind me - 5 Days before',
  'Remind me - 10 Days before',
];
const FREQUENCY_OPTIONS = ['Daily', 'Weekly', 'Monthly', 'Annual'];

function MedicineAccordionSection({
  title,
  subtitle,
  actionLabel,
  open,
  onToggle,
  onAction,
  children,
}) {
  return (
    <section className={`add-bucket-accordion-section ${open ? 'is-open' : 'is-collapsed'}`}>
      <button type="button" className="add-bucket-accordion-header" onClick={onToggle}>
        <h3 className="add-bucket-accordion-title">{title}</h3>
        <span className="add-bucket-accordion-chevron" aria-hidden="true">
          {open ? '▲' : '▼'}
        </span>
      </button>

      {open && (
        <div className="add-bucket-accordion-body">
          <h4 className="add-bucket-form-section-title">{subtitle}</h4>
          {children}
          <button type="button" className="add-bucket-section-action-btn" onClick={onAction}>
            {actionLabel}
          </button>
        </div>
      )}
    </section>
  );
}

export default function AddBucketMedicineDetailsPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, updateDraft, successResult, setSuccessResult, resetDraft, clearSuccess } =
    useAddBucket();
  const [apiError, setApiError] = useState('');
  const { submitting, runSubmit } = useAsyncSubmit();
  const [uploadKey, setUploadKey] = useState(null);
  const [openSections, setOpenSections] = useState({
    medicines: true,
    patients: false,
    schedules: false,
  });
  const medicine = draft.medicine;

  function updateMedicine(partial) {
    updateDraft({ medicine: { ...medicine, ...partial } });
  }

  function toggleSection(sectionKey) {
    setOpenSections((current) => ({
      ...current,
      [sectionKey]: !current[sectionKey],
    }));
  }

  async function uploadFile(file, applyUpdate) {
    if (!file) return;
    setApiError('');
    try {
      const response = await uploadBucketFile(file);
      applyUpdate(response.data.fileToken, response.data.fileName);
    } catch (error) {
      setApiError(error.message || 'Unable to upload file.');
    } finally {
      setUploadKey(null);
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
        setApiError(error.message || 'Unable to create medicine bucket.');
      }
    });
  }

  return (
    <>
      <AddBucketLayout
        title="About the Medicine"
        subtitle="Fill out below the information about the medicine."
        step={2}
        totalSteps={2}
      >
        <BucketSummaryCard draft={draft} selectedCategory={selectedCategory} />
        <AlertMessage message={apiError} />

        <form className="add-bucket-card add-bucket-form-card" onSubmit={handleSubmit} noValidate>
          <MedicineAccordionSection
            title="MY MEDICINES"
            subtitle="Details of My Medicine"
            actionLabel="ADD MY MEDICINES"
            open={openSections.medicines}
            onToggle={() => toggleSection('medicines')}
            onAction={() =>
              updateMedicine({ medicines: [...medicine.medicines, { ...defaultMedicineItem }] })
            }
          >
            {medicine.medicines.map((item, index) => (
              <div key={`medicine-${index}`} className="add-bucket-repeat-block">
                <FormField
                  label="Name of the Medicine"
                  name={`medicine-name-${index}`}
                  value={item.name}
                  onChange={(e) => {
                    const medicines = [...medicine.medicines];
                    medicines[index] = { ...medicines[index], name: e.target.value };
                    updateMedicine({ medicines });
                  }}
                  placeholder="Medicine name"
                  required
                />
                <FormField label="Add reminder" name={`medicine-reminder-${index}`}>
                  <select
                    className="tijori-select w-100"
                    value={item.reminderTiming}
                    onChange={(e) => {
                      const medicines = [...medicine.medicines];
                      medicines[index] = { ...medicines[index], reminderTiming: e.target.value };
                      updateMedicine({ medicines });
                    }}
                  >
                    {REMINDER_OPTIONS.map((option) => (
                      <option key={option} value={option}>
                        {option}
                      </option>
                    ))}
                  </select>
                </FormField>
                <FormField label="Notes" name={`medicine-notes-${index}`}>
                  <textarea
                    className="tijori-input add-bucket-textarea"
                    rows={3}
                    value={item.notes}
                    onChange={(e) => {
                      const medicines = [...medicine.medicines];
                      medicines[index] = { ...medicines[index], notes: e.target.value };
                      updateMedicine({ medicines });
                    }}
                    placeholder="Write some notes about your Medicine..."
                  />
                </FormField>
                <FileUploadField
                  label="Upload receipt/document"
                  fileName={item.fileName}
                  uploading={uploadKey === `medicine-${index}`}
                  onChange={(e) => {
                    const file = e.target.files?.[0];
                    setUploadKey(`medicine-${index}`);
                    uploadFile(file, (token, name) => {
                      const medicines = [...medicine.medicines];
                      medicines[index] = { ...medicines[index], fileToken: token, fileName: name };
                      updateMedicine({ medicines });
                    });
                  }}
                />
                <label className="add-bucket-checkbox">
                  <input
                    type="checkbox"
                    checked={item.scanWithAiOcr}
                    onChange={(e) => {
                      const medicines = [...medicine.medicines];
                      medicines[index] = { ...medicines[index], scanWithAiOcr: e.target.checked };
                      updateMedicine({ medicines });
                    }}
                  />
                  <span>Scan AI-OCR Process to extract information</span>
                </label>
              </div>
            ))}
            <button
              type="button"
              className="add-bucket-inline-add mb-3"
              onClick={() => updateMedicine({ medicines: [...medicine.medicines, { ...defaultMedicineItem }] })}
            >
              + Add multiple records
            </button>
          </MedicineAccordionSection>

          <MedicineAccordionSection
            title="PATIENT NAME"
            subtitle="Details of Patient"
            actionLabel="ADD TO PATIENT NAME"
            open={openSections.patients}
            onToggle={() => toggleSection('patients')}
            onAction={() => updateMedicine({ patients: [...medicine.patients, { ...defaultPatientItem }] })}
          >
            {medicine.patients.map((item, index) => (
              <div key={`patient-${index}`} className="add-bucket-repeat-block">
                <FormField
                  label="Name of the Patient"
                  name={`patient-name-${index}`}
                  value={item.name}
                  onChange={(e) => {
                    const patients = [...medicine.patients];
                    patients[index] = { ...patients[index], name: e.target.value };
                    updateMedicine({ patients });
                  }}
                  placeholder="Patient name"
                />
                <FormField
                  label="Date of Birth"
                  name={`patient-dob-${index}`}
                  type="date"
                  value={item.dateOfBirth}
                  onChange={(e) => {
                    const patients = [...medicine.patients];
                    patients[index] = { ...patients[index], dateOfBirth: e.target.value };
                    updateMedicine({ patients });
                  }}
                />
                <FormField label="Notes" name={`patient-notes-${index}`}>
                  <textarea
                    className="tijori-input add-bucket-textarea"
                    rows={3}
                    value={item.notes}
                    onChange={(e) => {
                      const patients = [...medicine.patients];
                      patients[index] = { ...patients[index], notes: e.target.value };
                      updateMedicine({ patients });
                    }}
                    placeholder="Write some notes about your Medicine..."
                  />
                </FormField>
              </div>
            ))}
            <button
              type="button"
              className="add-bucket-inline-add mb-3"
              onClick={() => updateMedicine({ patients: [...medicine.patients, { ...defaultPatientItem }] })}
            >
              + Add multiple records
            </button>
          </MedicineAccordionSection>

          <MedicineAccordionSection
            title="MEDICINE NAME"
            subtitle="Details of Medicine"
            actionLabel="ADD TO MEDICINE NAME"
            open={openSections.schedules}
            onToggle={() => toggleSection('schedules')}
            onAction={() =>
              updateMedicine({ schedules: [...medicine.schedules, { ...defaultScheduleItem }] })
            }
          >
            {medicine.schedules.map((item, index) => (
              <div key={`schedule-${index}`} className="add-bucket-repeat-block">
                <FileUploadField
                  label="Photo of Medicine"
                  fileName={item.photoFileName}
                  uploading={uploadKey === `schedule-${index}`}
                  onChange={(e) => {
                    const file = e.target.files?.[0];
                    setUploadKey(`schedule-${index}`);
                    uploadFile(file, (token, name) => {
                      const schedules = [...medicine.schedules];
                      schedules[index] = { ...schedules[index], photoFileToken: token, photoFileName: name };
                      updateMedicine({ schedules });
                    });
                  }}
                />
                <div className="add-bucket-date-row">
                  <FormField
                    label="Start date Medicine"
                    name={`schedule-start-${index}`}
                    type="date"
                    value={item.startDate}
                    onChange={(e) => {
                      const schedules = [...medicine.schedules];
                      schedules[index] = { ...schedules[index], startDate: e.target.value };
                      updateMedicine({ schedules });
                    }}
                  />
                  <FormField
                    label="End date Medicine"
                    name={`schedule-end-${index}`}
                    type="date"
                    value={item.endDate}
                    onChange={(e) => {
                      const schedules = [...medicine.schedules];
                      schedules[index] = { ...schedules[index], endDate: e.target.value };
                      updateMedicine({ schedules });
                    }}
                  />
                </div>
                <div className="add-bucket-frequency-row">
                  <span className="tijori-label d-block mb-2">When to eat</span>
                  {FREQUENCY_OPTIONS.map((option) => (
                    <button
                      key={option}
                      type="button"
                      className={`add-bucket-frequency-chip ${
                        item.frequency === option ? 'add-bucket-frequency-chip-active' : ''
                      }`}
                      onClick={() => {
                        const schedules = [...medicine.schedules];
                        schedules[index] = { ...schedules[index], frequency: option };
                        updateMedicine({ schedules });
                      }}
                    >
                      {option}
                    </button>
                  ))}
                </div>
                <FormField
                  label="Dosage of Medicine"
                  name={`dosage-${index}`}
                  value={item.dosage}
                  onChange={(e) => {
                    const schedules = [...medicine.schedules];
                    schedules[index] = { ...schedules[index], dosage: e.target.value };
                    updateMedicine({ schedules });
                  }}
                  placeholder="Dosage of Medicine"
                />
                <FormField label="Notes" name={`schedule-notes-${index}`}>
                  <textarea
                    className="tijori-input add-bucket-textarea"
                    rows={3}
                    value={item.notes}
                    onChange={(e) => {
                      const schedules = [...medicine.schedules];
                      schedules[index] = { ...schedules[index], notes: e.target.value };
                      updateMedicine({ schedules });
                    }}
                    placeholder="Write some notes about your Medicine..."
                  />
                </FormField>
              </div>
            ))}
            <button
              type="button"
              className="add-bucket-inline-add mb-3"
              onClick={() => updateMedicine({ schedules: [...medicine.schedules, { ...defaultScheduleItem }] })}
            >
              + Add multiple records
            </button>
          </MedicineAccordionSection>

          <LoadingButton type="submit" loading={submitting}>
            ADD MY BUCKET
          </LoadingButton>
        </form>
      </AddBucketLayout>

      <AddBucketSuccessModal result={successResult} onClose={clearSuccess} />
    </>
  );
}
