import { Link } from 'react-router-dom';

export default function BackLink({ to, label = 'Back' }) {
  return (
    <Link to={to} className="tijori-back">
      <span aria-hidden="true">&larr;</span>
      {label}
    </Link>
  );
}
