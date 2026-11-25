export default function ResourceStatus({ status, error, onRetry, label }) {
  if (status === "loading") return <p className="notice" role="status">Loading {label}…</p>;
  if (status === "empty") return <p className="notice" role="status">There are no {label} yet.</p>;
  if (status === "notfound") return <p className="notice" role="status">This item could not be found. Check the address or return to the overview.</p>;
  if (status === "error") return <div className="notice error" role="alert"><p>{error.message}</p><button type="button" className="secondary" onClick={onRetry}>Try again</button></div>;
  return null;
}
