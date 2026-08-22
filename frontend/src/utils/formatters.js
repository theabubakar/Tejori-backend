export function formatBytes(bytes) {
  if (!bytes || bytes <= 0) return '0 MB';

  const units = ['B', 'KB', 'MB', 'GB', 'TB'];
  let value = bytes;
  let unitIndex = 0;

  while (value >= 1024 && unitIndex < units.length - 1) {
    value /= 1024;
    unitIndex += 1;
  }

  const precision = unitIndex === 0 ? 0 : value >= 10 ? 0 : 1;
  return `${value.toFixed(precision)} ${units[unitIndex]}`;
}

export function formatStorageLabel(usedBytes, totalBytes) {
  return `${formatBytes(usedBytes)} of ${formatBytes(totalBytes)}`;
}

export function formatStoragePercentage(percentageUsed, usedBytes = 0) {
  const value = Number(percentageUsed ?? 0);
  if (usedBytes > 0 && value > 0 && value < 1) {
    return `${value.toFixed(1)}%`;
  }
  return `${Math.round(value)}%`;
}

export function getStorageProgressWidth(percentageUsed, usedBytes = 0) {
  const value = Number(percentageUsed ?? 0);
  if (usedBytes > 0 && value > 0 && value < 1) {
    return Math.max(value, 1);
  }
  return Math.min(value, 100);
}

export function matchesSearchQuery(value, query) {
  if (!query) return true;
  if (value === null || value === undefined) return false;
  return String(value).toLowerCase().includes(query);
}

export function formatDate(value) {
  return new Intl.DateTimeFormat('en-GB', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  }).format(new Date(value));
}

export function formatAmount(amount, currency, progressPercentage) {
  const formattedAmount = new Intl.NumberFormat('en-US', {
    maximumFractionDigits: 0,
  }).format(amount);

  return `${formattedAmount} ${currency} (${progressPercentage}%)`;
}

export function getInitials(fullName) {
  if (!fullName) return 'U';

  return fullName
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase() || '')
    .join('');
}

export function bucketIcon(iconKey) {
  switch (iconKey?.toLowerCase()) {
    case 'warranties':
      return 'shield';
    case 'insurance':
      return 'shield';
    case 'contract':
    case 'contracts':
    default:
      return 'folder';
  }
}
