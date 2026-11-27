import ContentForm from "./ContentForm.jsx";
import { blogPostFields } from "./contentFields.js";

export default function BlogPostForm(props) {
  return <ContentForm {...props} fields={blogPostFields} label="blog post" />;
}
