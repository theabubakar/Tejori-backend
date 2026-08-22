export default function AlertMessage({ type = 'error', message, errors = [] }) {
  if (!message && (!errors || errors.length === 0)) {
    return null;
  }

  const className =
    type === 'success' ? 'tijori-alert tijori-alert-success' : 'tijori-alert tijori-alert-error';

  return (
    <div className={className} role="alert">
      {message && <div>{message}</div>}
      {errors?.length > 0 && (
        <ul className="mb-0 mt-2 ps-3">
          {errors.map((item) => (
            <li key={item}>{item}</li>
          ))}
        </ul>
      )}
    </div>
  );
}
