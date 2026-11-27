export const projectFields = [
  { name: "title", label: "Title", maxLength: 100 },
  { name: "type", label: "Type", maxLength: 50 },
  { name: "description", label: "Description", maxLength: 2000, multiline: true },
];

export const blogPostFields = [
  { name: "title", label: "Title", maxLength: 100 },
  { name: "category", label: "Category", maxLength: 50 },
  { name: "content", label: "Content", maxLength: 10000, multiline: true },
  { name: "publishedOn", label: "Publication date", type: "date" },
];

export function validateFields(fields, values) {
  const errors = {};
  for (const field of fields) {
    const value = (values[field.name] ?? "").trim();
    if (!value) errors[field.name] = `${field.label} is required.`;
    else if (field.maxLength && value.length > field.maxLength)
      errors[field.name] = `${field.label} must be ${field.maxLength} characters or fewer.`;
    else if (field.type === "date") {
      const date = new Date(value + "T00:00:00Z");
      if (!/^\d{4}-\d{2}-\d{2}$/.test(value) || Number.isNaN(date.valueOf()) ||
          date.toISOString().slice(0, 10) !== value || Number(value.slice(0, 4)) < 1)
        errors[field.name] = "Enter a valid publication date.";
    }
  }
  return errors;
}
