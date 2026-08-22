import { createContext, useContext, useMemo, useState } from 'react';

const STORAGE_KEY = 'tijori_add_bucket_draft';

const defaultContract = {
  companyName: '',
  representativeName: '',
  companyPhoneCountryCode: '+965',
  companyPhone: '',
  whatsAppCountryCode: '+965',
  whatsApp: '',
  companyEmail: '',
  contractName: '',
  contractDate: '',
  contractAmount: '',
  currency: 'KD',
  numberOfPayments: '',
  paymentMethod: '',
  alertListType: 'Monthly notifications',
  payments: [{ amount: '', dueDate: '', notificationTiming: 'Get notified 1 Day before' }],
  phases: [{ title: '', dueDate: '', notificationTiming: 'Get notified 1 Day before', progressPercentage: '' }],
};

const defaultWarranty = {
  brandName: '',
  price: '',
  currency: 'KD',
  serialNumber: '',
  sellerName: '',
  sellerPhoneCountryCode: '+965',
  sellerPhone: '',
  startDate: '',
  expiryDate: '',
  purchaseLocation: '',
  countryOfManufacture: '',
  storeLocationUrl: '',
  expiryReminderEnabled: false,
  expiryReminderTiming: 'One Week before',
  coverages: [{ coverageArea: '', coverageOption: '' }],
};

const defaultDocument = {
  documentType: '',
  fileToken: '',
  fileName: '',
  startDate: '',
  endDate: '',
  extensionDate: '',
};

const defaultTripPassport = {
  documentType: 'Passport',
  frontFileToken: '',
  frontFileName: '',
  backFileToken: '',
  backFileName: '',
};

const defaultTripFileItem = { type: '', fileToken: '', fileName: '' };
const defaultTripCarRental = { startDate: '', endDate: '', fileToken: '', fileName: '' };

const defaultTrip = {
  tripName: '',
  numberOfPeople: '',
  travelType: 'International',
  startDate: '',
  returnDate: '',
  savePassportInfo: false,
  reminderTiming: 'Remind me - One Day before',
  notes: '',
  passports: [{ ...defaultTripPassport }],
  transportations: [{ type: 'Flight', fileToken: '', fileName: '' }],
  accommodations: [{ type: 'Hotel', fileToken: '', fileName: '' }],
  carRentals: [{ ...defaultTripCarRental }],
  clubs: [{ type: 'Entertainment', fileToken: '', fileName: '' }],
  matches: [{ type: 'Football', fileToken: '', fileName: '' }],
  invoiceFileToken: '',
  invoiceFileName: '',
};

const defaultAppointmentRecord = {
  appointmentName: '',
  appointmentDate: '',
  hours: '',
  minutes: '',
  amPm: 'AM',
  reminderTiming: 'Remind me - One Day before',
  notes: '',
  locationLink: '',
  fileToken: '',
  fileName: '',
  scanWithAiOcr: false,
};

const defaultAppointment = {
  records: [{ ...defaultAppointmentRecord }],
};

const defaultMedicineItem = {
  name: '',
  reminderTiming: 'Remind me - One Day before',
  notes: '',
  fileToken: '',
  fileName: '',
  scanWithAiOcr: false,
};

const defaultPatientItem = {
  name: '',
  dateOfBirth: '',
  notes: '',
};

const defaultScheduleItem = {
  photoFileToken: '',
  photoFileName: '',
  startDate: '',
  endDate: '',
  frequency: 'Daily',
  dosage: '',
  notes: '',
};

const defaultMedicine = {
  medicines: [{ ...defaultMedicineItem }],
  patients: [{ ...defaultPatientItem }],
  schedules: [{ ...defaultScheduleItem }],
};

const defaultCustom = {
  fields: [],
  values: {},
};

const defaultState = {
  name: '',
  description: '',
  categoryId: '',
  customGroupCategoryId: '',
  pendingCustomCategory: null,
  warrantySubCategoryKey: '',
  scanWithAiOcr: false,
  initialDocument: { ...defaultDocument },
  contract: { ...defaultContract },
  warranty: { ...defaultWarranty },
  trip: { ...defaultTrip },
  appointment: { ...defaultAppointment },
  medicine: { ...defaultMedicine },
  custom: { ...defaultCustom },
  documents: [{ ...defaultDocument }],
  remarks: '',
};

function loadDraft() {
  try {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    return raw ? { ...defaultState, ...JSON.parse(raw) } : { ...defaultState };
  } catch {
    return { ...defaultState };
  }
}

const AddBucketContext = createContext(null);

export function AddBucketProvider({ children }) {
  const [draft, setDraft] = useState(loadDraft);
  const [setup, setSetup] = useState({
    categories: [],
    customCategories: [],
    customGroupCategoryId: null,
    warrantySubCategories: [],
  });
  const [successResult, setSuccessResult] = useState(null);

  function persist(nextDraft) {
    setDraft(nextDraft);
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(nextDraft));
  }

  function updateDraft(partial) {
    persist({ ...draft, ...partial });
  }

  function resetDraft() {
    sessionStorage.removeItem(STORAGE_KEY);
    setDraft({ ...defaultState });
  }

  function clearSuccess() {
    setSuccessResult(null);
  }

  const selectedCategory = useMemo(() => {
    const fromSetup = setup.categories.find((item) => item.id === draft.categoryId);
    if (fromSetup) {
      return fromSetup;
    }

    const fromCustomList = setup.customCategories?.find((item) => item.id === draft.categoryId);
    if (fromCustomList) {
      return fromCustomList;
    }

    if (draft.pendingCustomCategory?.id === draft.categoryId) {
      return draft.pendingCustomCategory;
    }

    return null;
  }, [setup.categories, setup.customCategories, draft.categoryId, draft.pendingCustomCategory]);

  const value = {
    draft,
    setup,
    selectedCategory,
    successResult,
    setSetup,
    updateDraft,
    resetDraft,
    clearSuccess,
    setSuccessResult,
  };

  return <AddBucketContext.Provider value={value}>{children}</AddBucketContext.Provider>;
}

export function useAddBucket() {
  const context = useContext(AddBucketContext);
  if (!context) {
    throw new Error('useAddBucket must be used within AddBucketProvider.');
  }
  return context;
}

export function isContractCategory(category) {
  return category?.iconKey?.toLowerCase() === 'contract';
}

export function isWarrantyCategory(category) {
  return category?.iconKey?.toLowerCase() === 'warranties';
}

export function isMyTripsCategory(category) {
  return category?.iconKey?.toLowerCase() === 'my-trips';
}

export function isMyAppointmentsCategory(category) {
  return category?.iconKey?.toLowerCase() === 'my-appointments';
}

export function isMyMedicineCategory(category) {
  return category?.iconKey?.toLowerCase() === 'my-medicine';
}

export function isCustomGroupCategory(category) {
  return category?.iconKey?.toLowerCase() === 'custom' && !category?.isCustom;
}

export function isCustomSubCategory(category) {
  return category?.isCustom || category?.iconKey?.toLowerCase() === 'custom-sub';
}

export function isCustomCategory(category) {
  return isCustomSubCategory(category);
}

export function getCustomGroupCategory(setup) {
  if (setup.customGroupCategoryId) {
    return setup.categories.find((item) => item.id === setup.customGroupCategoryId) || null;
  }

  return setup.categories.find((item) => isCustomGroupCategory(item)) || null;
}

function toOptionalNumber(value) {
  if (value === '' || value === null || value === undefined) {
    return null;
  }

  const parsed = Number(value);
  return Number.isFinite(parsed) ? parsed : null;
}

function buildDocumentsPayload(draft) {
  const documents = [];
  const seenFileTokens = new Set();

  const pushDocument = (item) => {
    const documentType = item.documentType?.trim();
    const fileToken = item.fileToken?.trim();

    if (!documentType && !fileToken) {
      return;
    }

    if (fileToken) {
      if (seenFileTokens.has(fileToken)) {
        return;
      }
      seenFileTokens.add(fileToken);
    }

    documents.push({
      documentType: documentType || 'Document',
      fileToken: fileToken || null,
      startDate: item.startDate || null,
      endDate: item.endDate || null,
      extensionDate: item.extensionDate || null,
    });
  };

  draft.documents.forEach(pushDocument);

  if (draft.initialDocument?.documentType || draft.initialDocument?.fileToken) {
    pushDocument({
      ...draft.initialDocument,
      documentType: draft.initialDocument.documentType?.trim() || 'Initial Document',
    });
  }

  return documents;
}

export function buildCreateBucketPayload(draft, selectedCategory) {
  const documents = buildDocumentsPayload(draft);

  const payload = {
    name: draft.name.trim(),
    description: draft.description.trim(),
    categoryId: draft.categoryId,
    scanWithAiOcr: draft.scanWithAiOcr,
    warrantySubCategoryKey: draft.warrantySubCategoryKey || null,
    remarks: draft.remarks || null,
    documents,
  };

  if (isContractCategory(selectedCategory)) {
    payload.contract = {
      companyName: draft.contract.companyName || null,
      representativeName: draft.contract.representativeName || null,
      companyPhoneCountryCode: draft.contract.companyPhoneCountryCode || null,
      companyPhone: draft.contract.companyPhone || null,
      whatsAppCountryCode: draft.contract.whatsAppCountryCode || null,
      whatsApp: draft.contract.whatsApp || null,
      companyEmail: draft.contract.companyEmail || null,
      contractName: draft.contract.contractName,
      contractDate: draft.contract.contractDate || null,
      contractAmount: toOptionalNumber(draft.contract.contractAmount),
      currency: draft.contract.currency || 'KD',
      numberOfPayments: toOptionalNumber(draft.contract.numberOfPayments),
      paymentMethod: draft.contract.paymentMethod || null,
      alertListType: draft.contract.alertListType || null,
      payments: draft.contract.payments
        .filter((item) => item.amount || item.dueDate)
        .map((item) => ({
          amount: toOptionalNumber(item.amount) ?? 0,
          dueDate: item.dueDate || null,
          notificationTiming: item.notificationTiming || null,
        })),
      phases: draft.contract.phases
        .filter((item) => item.title?.trim() || item.dueDate)
        .map((item) => ({
          title: item.title?.trim() || '',
          dueDate: item.dueDate || null,
          notificationTiming: item.notificationTiming || null,
          progressPercentage: toOptionalNumber(item.progressPercentage),
        })),
    };
  }

  if (isWarrantyCategory(selectedCategory)) {
    payload.warranty = {
      brandName: draft.warranty.brandName,
      price: toOptionalNumber(draft.warranty.price),
      currency: draft.warranty.currency || 'KD',
      serialNumber: draft.warranty.serialNumber || null,
      sellerName: draft.warranty.sellerName || null,
      sellerPhoneCountryCode: draft.warranty.sellerPhoneCountryCode || null,
      sellerPhone: draft.warranty.sellerPhone || null,
      startDate: draft.warranty.startDate || null,
      expiryDate: draft.warranty.expiryDate || null,
      purchaseLocation: draft.warranty.purchaseLocation || null,
      countryOfManufacture: draft.warranty.countryOfManufacture || null,
      storeLocationUrl: draft.warranty.storeLocationUrl || null,
      expiryReminderEnabled: draft.warranty.expiryReminderEnabled,
      expiryReminderTiming: draft.warranty.expiryReminderEnabled
        ? draft.warranty.expiryReminderTiming
        : null,
      coverages: draft.warranty.coverages
        .filter((item) => item.coverageArea || item.coverageOption)
        .map((item) => ({
          coverageArea: item.coverageArea || null,
          coverageOption: item.coverageOption || null,
        })),
    };
  }

  if (isMyTripsCategory(selectedCategory)) {
    const trip = draft.trip;
    const items = [];

    trip.passports.forEach((passport) => {
      if (passport.frontFileToken) {
        items.push({
          itemType: 'passport',
          title: passport.documentType || 'Passport',
          referenceNumber: 'front',
          fileToken: passport.frontFileToken,
        });
      }
      if (passport.backFileToken) {
        items.push({
          itemType: 'passport',
          title: passport.documentType || 'Passport',
          referenceNumber: 'back',
          fileToken: passport.backFileToken,
        });
      }
    });

    trip.transportations
      .filter((item) => item.type || item.fileToken)
      .forEach((item) => {
        items.push({
          itemType: 'transportation',
          title: item.type || null,
          fileToken: item.fileToken || null,
        });
      });

    trip.accommodations
      .filter((item) => item.type || item.fileToken)
      .forEach((item) => {
        items.push({
          itemType: 'accommodation',
          title: item.type || null,
          fileToken: item.fileToken || null,
        });
      });

    trip.carRentals
      .filter((item) => item.startDate || item.endDate || item.fileToken)
      .forEach((item) => {
        items.push({
          itemType: 'car_rental',
          startDate: item.startDate || null,
          endDate: item.endDate || null,
          fileToken: item.fileToken || null,
        });
      });

    trip.clubs
      .filter((item) => item.type || item.fileToken)
      .forEach((item) => {
        items.push({
          itemType: 'club',
          title: item.type || null,
          fileToken: item.fileToken || null,
        });
      });

    trip.matches
      .filter((item) => item.type || item.fileToken)
      .forEach((item) => {
        items.push({
          itemType: 'match',
          title: item.type || null,
          fileToken: item.fileToken || null,
        });
      });

    if (trip.invoiceFileToken) {
      items.push({
        itemType: 'invoice',
        title: 'Trip Invoice',
        fileToken: trip.invoiceFileToken,
      });
    }

    payload.trip = {
      destination: trip.tripName || draft.name.trim(),
      startDate: trip.startDate || null,
      endDate: trip.returnDate || null,
      notes: [
        trip.numberOfPeople ? `People: ${trip.numberOfPeople}` : '',
        trip.travelType ? `Travel type: ${trip.travelType}` : '',
        trip.reminderTiming ? `Reminder: ${trip.reminderTiming}` : '',
        trip.notes || '',
      ]
        .filter(Boolean)
        .join('\n'),
      items,
    };
  }

  if (isMyAppointmentsCategory(selectedCategory)) {
    payload.appointment = {
      notes: draft.remarks || null,
      records: draft.appointment.records
        .filter(
          (record) =>
            record.appointmentName ||
            record.appointmentDate ||
            record.fileToken ||
            record.locationLink,
        )
        .map((record) => ({
          title: record.appointmentName || null,
          appointmentDate: record.appointmentDate || null,
          appointmentTime:
            record.hours || record.minutes
              ? `${record.hours || '00'}:${record.minutes || '00'} ${record.amPm || 'AM'}`
              : null,
          status: record.reminderTiming || null,
          notes: [record.notes, record.locationLink ? `Location: ${record.locationLink}` : '']
            .filter(Boolean)
            .join('\n'),
          fileToken: record.fileToken || null,
        })),
    };
    payload.scanWithAiOcr = draft.appointment.records.some((record) => record.scanWithAiOcr);
  }

  if (isMyMedicineCategory(selectedCategory)) {
    const records = [];

    draft.medicine.medicines
      .filter((item) => item.name || item.notes || item.fileToken)
      .forEach((item) => {
        records.push({
          section: 'Medicine',
          label: item.name || 'Medicine',
          value: [item.reminderTiming, item.notes].filter(Boolean).join(' | '),
          fileToken: item.fileToken || null,
        });
      });

    draft.medicine.patients
      .filter((item) => item.name || item.dateOfBirth || item.notes)
      .forEach((item) => {
        records.push({
          section: 'Patient',
          label: item.name || 'Patient',
          value: [item.dateOfBirth, item.notes].filter(Boolean).join(' | '),
        });
      });

    draft.medicine.schedules
      .filter((item) => item.startDate || item.endDate || item.dosage || item.photoFileToken)
      .forEach((item) => {
        records.push({
          section: 'Schedule',
          label: item.frequency || 'Schedule',
          value: [item.startDate, item.endDate, item.dosage, item.notes].filter(Boolean).join(' | '),
          fileToken: item.photoFileToken || null,
        });
      });

    payload.medicine = records;
    payload.scanWithAiOcr = draft.medicine.medicines.some((item) => item.scanWithAiOcr);
  }

  if (isCustomCategory(selectedCategory)) {
    payload.customFieldValues = Object.entries(draft.custom.values).map(([fieldId, value]) => ({
      fieldId,
      value: typeof value === 'string' ? value : value?.text || null,
      fileToken: value?.fileToken || null,
    }));
  }

  return payload;
}

export {
  defaultDocument,
  defaultContract,
  defaultWarranty,
  defaultTrip,
  defaultTripPassport,
  defaultTripCarRental,
  defaultAppointmentRecord,
  defaultMedicineItem,
  defaultPatientItem,
  defaultScheduleItem,
};
