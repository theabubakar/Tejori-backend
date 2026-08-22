const ICON_MAP = {
  contract: 'bucket-icon-contract',
  warranties: 'bucket-icon-warranties',
  insurance: 'bucket-icon-insurance',
  'personal-doc': 'bucket-icon-personal-doc',
  'my-trips': 'bucket-icon-my-trips',
  'my-appointments': 'bucket-icon-my-appointments',
  'my-medicine': 'bucket-icon-my-medicine',
  custom: 'bucket-icon-custom',
  'custom-sub': 'bucket-icon-custom',
};

export default function CategoryIcon({ iconKey, className = '' }) {
  const symbolId = ICON_MAP[iconKey?.toLowerCase()] || 'bucket-icon-custom';

  return (
    <svg className={`add-bucket-category-svg ${className}`.trim()} aria-hidden="true">
      <use href={`/bucket-icons.svg#${symbolId}`} />
    </svg>
  );
}
