import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AlertMessage from '../../components/AlertMessage';
import FormField from '../../components/FormField';
import LoadingButton from '../../components/LoadingButton';
import AddBucketLayout, { BucketSummaryCard } from '../../components/add-bucket/AddBucketLayout';
import { useAddBucket } from '../../context/AddBucketContext';

const PAYMENT_NOTIFICATION_OPTIONS = [
  'Get notified 1 Day before',
  'Get notified 5 Days before',
  'Get notified 10 Days before',
];

const PAYMENT_METHOD_OPTIONS = ['Cash', 'EasyPaisa', 'Cheque'];

function DynamicSection({ title, items, onAdd, renderItem }) {
  return (
    <section className="add-bucket-form-section">
      <h3 className="add-bucket-form-section-title">{title}</h3>
      {items.map((item, index) => renderItem(item, index))}
      <button type="button" className="add-bucket-inline-add" onClick={onAdd}>
        + {title.includes('payment') ? 'Add step payment' : 'Add new phase'}
      </button>
    </section>
  );
}

export default function AddBucketContractDetailsPage() {
  const navigate = useNavigate();
  const { draft, selectedCategory, updateDraft } = useAddBucket();
  const [errors, setErrors] = useState({});
  const contract = draft.contract;

  function updateContract(partial) {
    updateDraft({ contract: { ...contract, ...partial } });
  }

  function validate() {
    const nextErrors = {};
    if (!contract.contractName.trim()) nextErrors.contractName = 'Contract name is required.';
    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  }

  function handleNext(event) {
    event.preventDefault();
    if (!validate()) return;
    navigate('/add-bucket/contract/documents');
  }

  return (
    <AddBucketLayout title="About contract" subtitle="Fill out below the informations about the contract." step={2}>
      <BucketSummaryCard draft={draft} selectedCategory={selectedCategory} />

      <form className="add-bucket-card add-bucket-form-card" onSubmit={handleNext} noValidate>
        <section className="add-bucket-form-section">
          <h3 className="add-bucket-form-section-title">Details of contractor</h3>
          <FormField label="Name of the Company" name="companyName" value={contract.companyName} onChange={(e) => updateContract({ companyName: e.target.value })} placeholder="Enter the Name of company (Optional)" />
          <FormField label="Representative Company Name" name="representativeName" value={contract.representativeName} onChange={(e) => updateContract({ representativeName: e.target.value })} placeholder="Enter the Representative Name (Optional)" />
          <div className="tijori-phone-row mb-3">
            <select className="tijori-select" value={contract.companyPhoneCountryCode} onChange={(e) => updateContract({ companyPhoneCountryCode: e.target.value })}>
              <option value="+965">+965</option>
              <option value="+92">+92</option>
            </select>
            <input className="tijori-input" value={contract.companyPhone} onChange={(e) => updateContract({ companyPhone: e.target.value })} placeholder="Phone number (Optional)" />
          </div>
          <div className="tijori-phone-row mb-3">
            <select className="tijori-select" value={contract.whatsAppCountryCode} onChange={(e) => updateContract({ whatsAppCountryCode: e.target.value })}>
              <option value="+965">+965</option>
              <option value="+92">+92</option>
            </select>
            <input className="tijori-input" value={contract.whatsApp} onChange={(e) => updateContract({ whatsApp: e.target.value })} placeholder="WhatsApp number (Optional)" />
          </div>
          <FormField label="Email of the Company" name="companyEmail" value={contract.companyEmail} onChange={(e) => updateContract({ companyEmail: e.target.value })} placeholder="Email (Optional)" />
        </section>

        <section className="add-bucket-form-section">
          <h3 className="add-bucket-form-section-title">Details of contract</h3>
          <FormField label="Name of the Contract" name="contractName" value={contract.contractName} onChange={(e) => updateContract({ contractName: e.target.value })} placeholder="Name of the contract" required error={errors.contractName} />
          <FormField label="Date of the Contract" name="contractDate" type="date" value={contract.contractDate} onChange={(e) => updateContract({ contractDate: e.target.value })} />
          <FormField label="Amount of the Contract" name="contractAmount" value={contract.contractAmount} onChange={(e) => updateContract({ contractAmount: e.target.value })} placeholder="Amount of the Contract" />
        </section>

        <section className="add-bucket-form-section">
          <h3 className="add-bucket-form-section-title">Details of payments</h3>
          <FormField label="Number of payments" name="numberOfPayments" value={contract.numberOfPayments} onChange={(e) => updateContract({ numberOfPayments: e.target.value })} placeholder="Enter a number" />
          <FormField label="Method of payment" name="paymentMethod">
            <select
              id="paymentMethod"
              name="paymentMethod"
              className="tijori-select w-100"
              value={contract.paymentMethod}
              onChange={(e) => updateContract({ paymentMethod: e.target.value })}
            >
              <option value="">Select method of payment</option>
              {PAYMENT_METHOD_OPTIONS.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
          </FormField>
        </section>

        <DynamicSection
          title="Contract payments"
          items={contract.payments}
          onAdd={() => updateContract({ payments: [...contract.payments, { amount: '', dueDate: '', notificationTiming: 'Get notified 1 Day before' }] })}
          renderItem={(item, index) => (
            <div key={`payment-${index}`} className="add-bucket-repeat-block">
              <div className="add-bucket-repeat-label">{index + 1} payment</div>
              <FormField label="Amount of the payment" name={`payment-amount-${index}`} value={item.amount} onChange={(e) => {
                const payments = [...contract.payments];
                payments[index] = { ...payments[index], amount: e.target.value };
                updateContract({ payments });
              }} />
              <FormField label="Due date of payment" name={`payment-due-${index}`} type="date" value={item.dueDate} onChange={(e) => {
                const payments = [...contract.payments];
                payments[index] = { ...payments[index], dueDate: e.target.value };
                updateContract({ payments });
              }} />
              <FormField label="When I get notified" name={`payment-notify-${index}`}>
                <select
                  id={`payment-notify-${index}`}
                  name={`payment-notify-${index}`}
                  className="tijori-select w-100"
                  value={item.notificationTiming}
                  onChange={(e) => {
                    const payments = [...contract.payments];
                    payments[index] = { ...payments[index], notificationTiming: e.target.value };
                    updateContract({ payments });
                  }}
                >
                  {PAYMENT_NOTIFICATION_OPTIONS.map((option) => (
                    <option key={option} value={option}>
                      {option}
                    </option>
                  ))}
                </select>
              </FormField>
            </div>
          )}
        />

        <DynamicSection
          title="Achievements details"
          items={contract.phases}
          onAdd={() => updateContract({ phases: [...contract.phases, { title: '', dueDate: '', notificationTiming: 'Get notified 1 Day before', progressPercentage: '' }] })}
          renderItem={(item, index) => (
            <div key={`phase-${index}`} className="add-bucket-repeat-block">
              <FormField label={`${index + 1} Phase title`} name={`phase-title-${index}`} value={item.title} onChange={(e) => {
                const phases = [...contract.phases];
                phases[index] = { ...phases[index], title: e.target.value };
                updateContract({ phases });
              }} placeholder="Land topography" />
              <FormField label="Due date" name={`phase-due-${index}`} type="date" value={item.dueDate} onChange={(e) => {
                const phases = [...contract.phases];
                phases[index] = { ...phases[index], dueDate: e.target.value };
                updateContract({ phases });
              }} />
              <FormField label="Percentage of achievement" name={`phase-progress-${index}`} value={item.progressPercentage} onChange={(e) => {
                const phases = [...contract.phases];
                phases[index] = { ...phases[index], progressPercentage: e.target.value };
                updateContract({ phases });
              }} />
            </div>
          )}
        />

        <LoadingButton type="submit">Next</LoadingButton>
      </form>
    </AddBucketLayout>
  );
}
