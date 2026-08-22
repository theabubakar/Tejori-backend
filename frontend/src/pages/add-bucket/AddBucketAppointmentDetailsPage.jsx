import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import FormField from '../../components/FormField';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard, FileUploadField } from '../../components/add-bucket/AddBucketLayout';
import AddBucketSuccessModal from '../../components/add-bucket/AddBucketSuccessModal';
import {
  buildCreateBucketPayload,
  defaultAppointmentRecord,
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

export default function AddBucketAppointmentDetailsPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, updateDraft, successResult, setSuccessResult, resetDraft, clearSuccess } =
    useAddBucket();
  const [apiError, setApiError] = useState('');
  const { submitting, runSubmit } = useAsyncSubmit();
  const [uploadIndex, setUploadIndex] = useState(null);
  const appointment = draft.appointment;

  function updateRecord(index, partial) {
    const records = [...appointment.records];
    records[index] = { ...records[index], ...partial };
    updateDraft({ appointment: { ...appointment, records } });
  }

  async function handleUpload(index, event) {
    const file = event.target.files?.[0];
    if (!file) return;

    setUploadIndex(index);
    setApiError('');
    try {
      const response = await uploadBucketFile(file);
      updateRecord(index, {
        fileToken: response.data.fileToken,
        fileName: response.data.fileName,
      });
    } catch (error) {
      setApiError(error.message || 'Unable to upload file.');
    } finally {
      setUploadIndex(null);
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
        setApiError(error.message || 'Unable to create appointment bucket.');
      }
    });
  }

  return (
    <>
      <AddBucketLayout
        title="About the Appointment"
        subtitle="Fill out below the informations about the appointment."
        step={3}
      >
        <BucketSummaryCard draft={draft} selectedCategory={selectedCategory} />
        <AlertMessage message={apiError} />

        <form className="add-bucket-card add-bucket-form-card" onSubmit={handleSubmit} noValidate>
          <section className="add-bucket-form-section">
            <h3 className="add-bucket-form-section-title">Details of My Appointment</h3>

            {appointment.records.map((record, index) => (
              <div key={`appointment-${index}`} className="add-bucket-repeat-block">
                <FormField
                  label="Name the Appointment"
                  name={`appointment-name-${index}`}
                  value={record.appointmentName}
                  onChange={(e) => updateRecord(index, { appointmentName: e.target.value })}
                  placeholder="Appointment name"
                  required
                />
                <FormField
                  label="Date"
                  name={`appointment-date-${index}`}
                  type="date"
                  value={record.appointmentDate}
                  onChange={(e) => updateRecord(index, { appointmentDate: e.target.value })}
                />
                <div className="add-bucket-time-row">
                  <FormField
                    label="Hours"
                    name={`hours-${index}`}
                    value={record.hours}
                    onChange={(e) => updateRecord(index, { hours: e.target.value })}
                    placeholder="09"
                  />
                  <span className="add-bucket-time-separator">:</span>
                  <FormField
                    label="Minutes"
                    name={`minutes-${index}`}
                    value={record.minutes}
                    onChange={(e) => updateRecord(index, { minutes: e.target.value })}
                    placeholder="30"
                  />
                  <FormField label="AM/PM" name={`ampm-${index}`}>
                    <select
                      className="tijori-select w-100"
                      value={record.amPm}
                      onChange={(e) => updateRecord(index, { amPm: e.target.value })}
                    >
                      <option value="AM">AM</option>
                      <option value="PM">PM</option>
                    </select>
                  </FormField>
                </div>
                <FormField label="Add Reminder" name={`reminder-${index}`}>
                  <select
                    className="tijori-select w-100"
                    value={record.reminderTiming}
                    onChange={(e) => updateRecord(index, { reminderTiming: e.target.value })}
                  >
                    {REMINDER_OPTIONS.map((option) => (
                      <option key={option} value={option}>
                        {option}
                      </option>
                    ))}
                  </select>
                </FormField>
                <FormField label="Notes" name={`notes-${index}`}>
                  <textarea
                    className="tijori-input add-bucket-textarea"
                    rows={3}
                    value={record.notes}
                    onChange={(e) => updateRecord(index, { notes: e.target.value })}
                    placeholder="Write some notes to memorize your appointment..."
                  />
                </FormField>
                <FormField
                  label="Location"
                  name={`location-${index}`}
                  value={record.locationLink}
                  onChange={(e) => updateRecord(index, { locationLink: e.target.value })}
                  placeholder="Location link"
                />
                <FileUploadField
                  label="Appointment Document"
                  fileName={record.fileName}
                  uploading={uploadIndex === index}
                  onChange={(e) => handleUpload(index, e)}
                />
                <label className="add-bucket-checkbox">
                  <input
                    type="checkbox"
                    checked={record.scanWithAiOcr}
                    onChange={(e) => updateRecord(index, { scanWithAiOcr: e.target.checked })}
                  />
                  <span>Scan Ai Ocr Process to extract informations.</span>
                </label>
              </div>
            ))}

            <button
              type="button"
              className="add-bucket-inline-add mb-3"
              onClick={() =>
                updateDraft({
                  appointment: {
                    ...appointment,
                    records: [...appointment.records, { ...defaultAppointmentRecord }],
                  },
                })
              }
            >
              + Add multiple records
            </button>
          </section>

          <LoadingButton type="submit" loading={submitting}>
            Add My Appointments
          </LoadingButton>
        </form>
      </AddBucketLayout>

      <AddBucketSuccessModal result={successResult} onClose={clearSuccess} />
    </>
  );
}
