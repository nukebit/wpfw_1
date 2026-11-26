import { useState } from "react";
import { useAuth } from "../auth/AuthContext.jsx";
import useApiResource from "../hooks/useApiResource.js";
import ResourceStatus from "./ResourceStatus.jsx";

export default function ResourceManager({ resourceKey, title, eyebrow, description, label, api, Form, renderCard }) {
  const { canEdit, refreshUser } = useAuth();
  const { data: items, status, error, reload } = useApiResource(api.getAll, resourceKey);
  // undefined closes the form; null creates an item; an object edits an item.
  const [editing, setEditing] = useState(undefined);
  const [busy, setBusy] = useState(false);
  const [actionError, setActionError] = useState("");
  const [serverErrors, setServerErrors] = useState({});
  const [message, setMessage] = useState("");

  function openForm(item) {
    setEditing(item); setActionError(""); setServerErrors({}); setMessage("");
  }

  async function reportError(error) {
    setActionError(error.message); setServerErrors(error.errors ?? {});
    if (error.status === 401 || error.status === 403) await refreshUser().catch(() => {});
  }

  async function saveItem(values) {
    setBusy(true); setActionError(""); setServerErrors({}); setMessage("");
    try {
      if (editing) await api.update(editing.id, values);
      else await api.create(values);
      setEditing(undefined);
      reload();
      setMessage(`The ${label} was saved. The overview is being refreshed from the database.`);
    } catch (error) { await reportError(error); }
    finally { setBusy(false); }
  }

  async function removeItem(item) {
    // Native confirmation supports keyboard use and prevents accidental deletion.
    if (!window.confirm(`Delete "${item.title}"? This cannot be undone.`)) return;
    setBusy(true); setActionError(""); setServerErrors({}); setMessage("");
    try {
      await api.remove(item.id);
      if (editing?.id === item.id) setEditing(undefined);
      reload();
      setMessage(`The ${label} was deleted.`);
    } catch (error) { await reportError(error); }
    finally { setBusy(false); }
  }

  return <>
    <div className="page-heading"><div><p className="eyebrow">{eyebrow}</p><h1>{title}</h1></div>
      {canEdit && <button type="button" disabled={busy} onClick={() => openForm(null)}>New {label}</button>}
    </div>
    <p className="lead">{description}</p>
    {actionError && <p className="notice error" role="alert">{actionError}</p>}
    {message && <p className="notice success" role="status">{message}</p>}
    {canEdit && editing !== undefined && <Form key={editing?.id ?? "new"} item={editing} onSave={saveItem} onCancel={() => setEditing(undefined)} busy={busy} serverErrors={serverErrors} />}
    <ResourceStatus status={status} error={error} onRetry={reload} label={title.toLowerCase()} />
    {status === "success" && <section className="cards" aria-label={title}>{items.map(item => renderCard(item, canEdit && <div className="actions">
      <button type="button" className="secondary" disabled={busy} onClick={() => openForm(item)} aria-label={`Edit ${item.title}`}>Edit</button>
      <button type="button" className="danger" disabled={busy} onClick={() => removeItem(item)} aria-label={`Delete ${item.title}`}>Delete</button>
    </div>))}</section>}
  </>;
}
