export default function FormField({
  label,
  name,
  type = 'text',
  value,
  onChange,
  placeholder,
  required = false,
  error,
  autoComplete,
  children,
}) {
  return (
    <div className="mb-3">
      {label && (
        <label htmlFor={name} className="tijori-label">
          {label}
          {required && <span className="tijori-required"> *</span>}
        </label>
      )}
      {children || (
        <input
          id={name}
          name={name}
          type={type}
          className="tijori-input"
          value={value}
          onChange={onChange}
          placeholder={placeholder}
          autoComplete={autoComplete}
        />
      )}
      {error && <div className="tijori-field-error">{error}</div>}
    </div>
  );
}
