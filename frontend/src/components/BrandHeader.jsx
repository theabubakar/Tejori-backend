export default function BrandHeader({ subtitle }) {
  return (
    <div className="tijori-logo-wrap">
      <div className="tijori-logo-icon" aria-hidden="true">
        &#128274;
      </div>
      <h1 className="tijori-brand">tijori+</h1>
      {subtitle && <p className="tijori-tagline">{subtitle}</p>}
    </div>
  );
}
