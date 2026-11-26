import { useId, useRef, useState } from "react";
import { validateFields } from "./contentFields.js";

export default function ContentForm({ item, fields, label, onSave, onCancel, busy, serverErrors }) {
  const prefix = useId();
  const formRef = useRef(null);
  const [values, setValues] = useState(() => Object.fromEntries(fields.map(field => [field.name, item?.[field.name] ?? ""])));
  const [errors, setErrors] = useState({});

  async function submit(event) {
    event.preventDefault();
    const errors = validateFields(fields, values);
    setErrors(errors);
    if (Object.keys(errors).length) {
      formRef.current.elements.namedItem(Object.keys(errors)[0])?.focus();
      return;
    }
    // Send only the editable DTO fields, with whitespace removed.
    await onSave(Object.fromEntries(fields.map(field => [field.name, values[field.name].trim()])));
  }

  return <form ref={formRef} onSubmit={submit} noValidate aria-label={`${item ? "Edit" : "New"} ${label}`} aria-busy={busy}>
    <h2>{item ? "Edit" : "New"} {label}</h2>
    {fields.map(field => {
      const serverKey = Object.keys(serverErrors ?? {}).find(key => key.toLowerCase() === field.name.toLowerCase());
      const error = errors[field.name] || (serverKey ? serverErrors[serverKey].join(" ") : "");
      const id = `${prefix}-${field.name}`;
      const props = {
        id, name: field.name, value: values[field.name], required: true,
        maxLength: field.maxLength, disabled: busy,
        "aria-invalid": Boolean(error), "aria-describedby": error ? `${id}-error` : undefined,
        onChange: event => {
          setValues({ ...values, [field.name]: event.target.value });
          setErrors({ ...errors, [field.name]: "" });
        },
      };
      return <div className="field" key={field.name}>
        <label htmlFor={id}>{field.label}</label>
        {field.multiline ? <textarea {...props} rows="7" /> : <input {...props} type={field.type ?? "text"} />}
        {error && <p className="field-error" id={`${id}-error`}>{error}</p>}
      </div>;
    })}
    <div className="actions"><button type="submit" disabled={busy}>{busy ? "Saving…" : "Save"}</button><button type="button" className="secondary" disabled={busy} onClick={onCancel}>Cancel</button></div>
  </form>;
}
