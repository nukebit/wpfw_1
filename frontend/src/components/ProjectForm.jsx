import ContentForm from "./ContentForm.jsx";
import { projectFields } from "./contentFields.js";

export default function ProjectForm(props) {
  return <ContentForm {...props} fields={projectFields} label="project" />;
}
