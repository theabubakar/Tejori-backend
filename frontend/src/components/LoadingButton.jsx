export default function LoadingButton({
  type = 'button',
  variant = 'primary',
  loading = false,
  disabled = false,
  children,
  onClick,
}) {
  const className =
    variant === 'dark'
      ? 'tijori-btn-dark'
      : variant === 'outline'
        ? 'tijori-btn-outline'
        : 'tijori-btn-primary';

  return (
    <button
      type={type}
      className={className}
      disabled={loading || disabled}
      onClick={onClick}
    >
      {loading ? 'Please wait...' : children}
    </button>
  );
}
