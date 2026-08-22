import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import FormField from '../../components/FormField';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard, FileUploadField } from '../../components/add-bucket/AddBucketLayout';
import AddBucketSuccessModal from '../../components/add-bucket/AddBucketSuccessModal';
import {
  buildCreateBucketPayload,
  defaultTripCarRental,
  defaultTripPassport,
  useAddBucket,
} from '../../context/AddBucketContext';
import { createBucket, uploadBucketFile } from '../../services/bucketService';
import { clearAuth } from '../../utils/storage';
import { useAsyncSubmit } from '../../utils/useAsyncSubmit';

const TRAVEL_TYPES = ['International', 'Domestic', 'Business', 'Leisure'];
const REMINDER_OPTIONS = [
  'Remind me - One Day before',
  'Remind me - 5 Days before',
  'Remind me - 10 Days before',
];
const TRANSPORT_TYPES = ['Flight', 'Boat', 'Car', 'Train', 'Bus'];
const ACCOMMODATION_TYPES = ['Hotel', 'Airbnb', 'Home', 'Resort', 'Villa'];
const CLUB_TYPES = ['Entertainment', 'Sport', 'Games', 'Wellness'];
const MATCH_TYPES = ['Football', 'Tennis', 'Padel ball', 'Basketball', 'Cricket'];

function RepeatSection({ title, onAdd, children }) {
  return (
    <section className="add-bucket-form-section">
      <h3 className="add-bucket-form-section-title">{title}</h3>
      {children}
      <button type="button" className="add-bucket-inline-add" onClick={onAdd}>
        + Add more
      </button>
    </section>
  );
}

function calcDurationDays(startDate, endDate) {
  if (!startDate || !endDate) return '';
  const start = new Date(startDate);
  const end = new Date(endDate);
  if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime()) || end < start) return '';
  const diff = Math.ceil((end - start) / (1000 * 60 * 60 * 24));
  return `${String(diff).padStart(2, '0')} Days`;
}

export default function AddBucketTripDetailsPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, updateDraft, successResult, setSuccessResult, resetDraft, clearSuccess } =
    useAddBucket();
  const [apiError, setApiError] = useState('');
  const { submitting, runSubmit } = useAsyncSubmit();
  const [uploadKey, setUploadKey] = useState(null);
  const trip = draft.trip;

  const durationLabel = useMemo(
    () => calcDurationDays(trip.startDate, trip.returnDate),
    [trip.startDate, trip.returnDate],
  );

  function updateTrip(partial) {
    updateDraft({ trip: { ...trip, ...partial } });
  }

  async function uploadFile(file, updater) {
    if (!file) return;
    setUploadKey(updater);
    setApiError('');
    try {
      const response = await uploadBucketFile(file);
      updater(response.data.fileToken, response.data.fileName);
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
        setApiError(error.message || 'Unable to create trip bucket.');
      }
    });
  }

  return (
    <>
      <AddBucketLayout title="About the Trip" subtitle="Fill out below the informations about the Trip." step={3}>
        <BucketSummaryCard draft={draft} selectedCategory={selectedCategory} />
        <AlertMessage message={apiError} />

        <form className="add-bucket-card add-bucket-form-card" onSubmit={handleSubmit} noValidate>
          <section className="add-bucket-form-section">
            <h3 className="add-bucket-form-section-title">Details of the trip</h3>
            <FormField
              label="Name of the trip"
              name="tripName"
              value={trip.tripName}
              onChange={(e) => updateTrip({ tripName: e.target.value })}
              placeholder="Add a Name to the trip"
            />
            <FormField
              label="Number of people"
              name="numberOfPeople"
              value={trip.numberOfPeople}
              onChange={(e) => updateTrip({ numberOfPeople: e.target.value })}
              placeholder="e.g. 1 person"
            />
            <FormField label="Type of travel" name="travelType">
              <select
                id="travelType"
                className="tijori-select w-100"
                value={trip.travelType}
                onChange={(e) => updateTrip({ travelType: e.target.value })}
              >
                {TRAVEL_TYPES.map((option) => (
                  <option key={option} value={option}>
                    {option}
                  </option>
                ))}
              </select>
            </FormField>

            {trip.passports.map((passport, index) => (
              <div key={`passport-${index}`} className="add-bucket-repeat-block">
                <FormField label="Select document" name={`passport-type-${index}`}>
                  <select
                    className="tijori-select w-100"
                    value={passport.documentType}
                    onChange={(e) => {
                      const passports = [...trip.passports];
                      passports[index] = { ...passports[index], documentType: e.target.value };
                      updateTrip({ passports });
                    }}
                  >
                    <option value="Passport">Passport</option>
                    <option value="ID">ID</option>
                    <option value="Visa">Visa</option>
                  </select>
                </FormField>
                <FileUploadField
                  label="Upload front side of document"
                  fileName={passport.frontFileName}
                  uploading={uploadKey === `passport-front-${index}`}
                  onChange={(e) =>
                    uploadFile(e.target.files?.[0], (token, name) => {
                      const passports = [...trip.passports];
                      passports[index] = { ...passports[index], frontFileToken: token, frontFileName: name };
                      updateTrip({ passports });
                    })
                  }
                />
                <FileUploadField
                  label="Upload back side of document"
                  fileName={passport.backFileName}
                  uploading={uploadKey === `passport-back-${index}`}
                  onChange={(e) =>
                    uploadFile(e.target.files?.[0], (token, name) => {
                      const passports = [...trip.passports];
                      passports[index] = { ...passports[index], backFileToken: token, backFileName: name };
                      updateTrip({ passports });
                    })
                  }
                />
              </div>
            ))}
            <button
              type="button"
              className="add-bucket-inline-add mb-3"
              onClick={() => updateTrip({ passports: [...trip.passports, { ...defaultTripPassport }] })}
            >
              + Add person
            </button>

            <div className="add-bucket-date-row">
              <FormField
                label="Start Date of the trip"
                name="startDate"
                type="date"
                value={trip.startDate}
                onChange={(e) => updateTrip({ startDate: e.target.value })}
              />
              <FormField
                label="Return Date"
                name="returnDate"
                type="date"
                value={trip.returnDate}
                onChange={(e) => updateTrip({ returnDate: e.target.value })}
              />
            </div>
            {durationLabel && <div className="add-bucket-duration">Duration of the trip: {durationLabel}</div>}

            <FileUploadField
              label="Invoice of the trip (e.g. Travel agency, booking)"
              fileName={trip.invoiceFileName}
              uploading={uploadKey === 'invoice'}
              onChange={(e) =>
                uploadFile(e.target.files?.[0], (token, name) =>
                  updateTrip({ invoiceFileToken: token, invoiceFileName: name }),
                )
              }
            />
            <label className="add-bucket-checkbox">
              <input
                type="checkbox"
                checked={trip.savePassportInfo}
                onChange={(e) => updateTrip({ savePassportInfo: e.target.checked })}
              />
              <span>Save All Our Passport/ID and invoice information</span>
            </label>
          </section>

          <RepeatSection
            title="Transportation"
            onAdd={() =>
              updateTrip({
                transportations: [...trip.transportations, { type: 'Flight', fileToken: '', fileName: '' }],
              })
            }
          >
            {trip.transportations.map((item, index) => (
              <div key={`transport-${index}`} className="add-bucket-repeat-block">
                <FormField label="Type of transportation" name={`transport-type-${index}`}>
                  <select
                    className="tijori-select w-100"
                    value={item.type}
                    onChange={(e) => {
                      const transportations = [...trip.transportations];
                      transportations[index] = { ...transportations[index], type: e.target.value };
                      updateTrip({ transportations });
                    }}
                  >
                    {TRANSPORT_TYPES.map((option) => (
                      <option key={option} value={option}>
                        {option}
                      </option>
                    ))}
                  </select>
                </FormField>
                <FileUploadField
                  label="Upload Ticket"
                  fileName={item.fileName}
                  uploading={uploadKey === `transport-${index}`}
                  onChange={(e) =>
                    uploadFile(e.target.files?.[0], (token, name) => {
                      const transportations = [...trip.transportations];
                      transportations[index] = { ...transportations[index], fileToken: token, fileName: name };
                      updateTrip({ transportations });
                    })
                  }
                />
              </div>
            ))}
          </RepeatSection>

          <RepeatSection
            title="Accommodation"
            onAdd={() =>
              updateTrip({
                accommodations: [...trip.accommodations, { type: 'Hotel', fileToken: '', fileName: '' }],
              })
            }
          >
            {trip.accommodations.map((item, index) => (
              <div key={`accommodation-${index}`} className="add-bucket-repeat-block">
                <FormField label="Type of accommodation" name={`accommodation-type-${index}`}>
                  <select
                    className="tijori-select w-100"
                    value={item.type}
                    onChange={(e) => {
                      const accommodations = [...trip.accommodations];
                      accommodations[index] = { ...accommodations[index], type: e.target.value };
                      updateTrip({ accommodations });
                    }}
                  >
                    {ACCOMMODATION_TYPES.map((option) => (
                      <option key={option} value={option}>
                        {option}
                      </option>
                    ))}
                  </select>
                </FormField>
                <FileUploadField
                  label="Upload reservation/invoice"
                  fileName={item.fileName}
                  uploading={uploadKey === `accommodation-${index}`}
                  onChange={(e) =>
                    uploadFile(e.target.files?.[0], (token, name) => {
                      const accommodations = [...trip.accommodations];
                      accommodations[index] = { ...accommodations[index], fileToken: token, fileName: name };
                      updateTrip({ accommodations });
                    })
                  }
                />
              </div>
            ))}
          </RepeatSection>

          <RepeatSection
            title="Car rental"
            onAdd={() => updateTrip({ carRentals: [...trip.carRentals, { ...defaultTripCarRental }] })}
          >
            {trip.carRentals.map((item, index) => (
              <div key={`car-${index}`} className="add-bucket-repeat-block">
                <div className="add-bucket-date-row">
                  <FormField
                    label="Start date rental"
                    name={`car-start-${index}`}
                    type="date"
                    value={item.startDate}
                    onChange={(e) => {
                      const carRentals = [...trip.carRentals];
                      carRentals[index] = { ...carRentals[index], startDate: e.target.value };
                      updateTrip({ carRentals });
                    }}
                  />
                  <FormField
                    label="End date rental"
                    name={`car-end-${index}`}
                    type="date"
                    value={item.endDate}
                    onChange={(e) => {
                      const carRentals = [...trip.carRentals];
                      carRentals[index] = { ...carRentals[index], endDate: e.target.value };
                      updateTrip({ carRentals });
                    }}
                  />
                </div>
                <FileUploadField
                  label="Car Insurance"
                  fileName={item.fileName}
                  uploading={uploadKey === `car-${index}`}
                  onChange={(e) =>
                    uploadFile(e.target.files?.[0], (token, name) => {
                      const carRentals = [...trip.carRentals];
                      carRentals[index] = { ...carRentals[index], fileToken: token, fileName: name };
                      updateTrip({ carRentals });
                    })
                  }
                />
              </div>
            ))}
          </RepeatSection>

          <RepeatSection
            title="Clubs reservation"
            onAdd={() => updateTrip({ clubs: [...trip.clubs, { type: 'Entertainment', fileToken: '', fileName: '' }] })}
          >
            {trip.clubs.map((item, index) => (
              <div key={`club-${index}`} className="add-bucket-repeat-block">
                <FormField label="Type of Club" name={`club-type-${index}`}>
                  <select
                    className="tijori-select w-100"
                    value={item.type}
                    onChange={(e) => {
                      const clubs = [...trip.clubs];
                      clubs[index] = { ...clubs[index], type: e.target.value };
                      updateTrip({ clubs });
                    }}
                  >
                    {CLUB_TYPES.map((option) => (
                      <option key={option} value={option}>
                        {option}
                      </option>
                    ))}
                  </select>
                </FormField>
                <FileUploadField
                  label="Upload reservation/invoice"
                  fileName={item.fileName}
                  uploading={uploadKey === `club-${index}`}
                  onChange={(e) =>
                    uploadFile(e.target.files?.[0], (token, name) => {
                      const clubs = [...trip.clubs];
                      clubs[index] = { ...clubs[index], fileToken: token, fileName: name };
                      updateTrip({ clubs });
                    })
                  }
                />
              </div>
            ))}
          </RepeatSection>

          <RepeatSection
            title="Matches reservation"
            onAdd={() => updateTrip({ matches: [...trip.matches, { type: 'Football', fileToken: '', fileName: '' }] })}
          >
            {trip.matches.map((item, index) => (
              <div key={`match-${index}`} className="add-bucket-repeat-block">
                <FormField label="Type of Match" name={`match-type-${index}`}>
                  <select
                    className="tijori-select w-100"
                    value={item.type}
                    onChange={(e) => {
                      const matches = [...trip.matches];
                      matches[index] = { ...matches[index], type: e.target.value };
                      updateTrip({ matches });
                    }}
                  >
                    {MATCH_TYPES.map((option) => (
                      <option key={option} value={option}>
                        {option}
                      </option>
                    ))}
                  </select>
                </FormField>
                <FileUploadField
                  label="Upload Ticket"
                  fileName={item.fileName}
                  uploading={uploadKey === `match-${index}`}
                  onChange={(e) =>
                    uploadFile(e.target.files?.[0], (token, name) => {
                      const matches = [...trip.matches];
                      matches[index] = { ...matches[index], fileToken: token, fileName: name };
                      updateTrip({ matches });
                    })
                  }
                />
              </div>
            ))}
          </RepeatSection>

          <FormField label="Add a reminder" name="reminderTiming">
            <select
              id="reminderTiming"
              className="tijori-select w-100"
              value={trip.reminderTiming}
              onChange={(e) => updateTrip({ reminderTiming: e.target.value })}
            >
              {REMINDER_OPTIONS.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
          </FormField>
          <FormField label="Notes" name="notes">
            <textarea
              id="notes"
              className="tijori-input add-bucket-textarea"
              rows={3}
              value={trip.notes}
              onChange={(e) => updateTrip({ notes: e.target.value })}
              placeholder="Write some notes to remind to your appointment..."
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
