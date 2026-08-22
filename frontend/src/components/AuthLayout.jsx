export default function AuthLayout({ children, wide = false }) {
  return (
    <div className="tijori-page">
      <div className={`tijori-shell ${wide ? 'tijori-shell-wide' : ''}`}>
        <div className="tijori-card">{children}</div>
      </div>
    </div>
  );
}
