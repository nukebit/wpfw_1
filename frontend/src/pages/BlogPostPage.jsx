import { useParams } from "react-router";
export default function BlogPostPage() {
  const { id } = useParams();
  return <><h1>Blog post</h1><p role="status">Loading blog post {id}.</p></>;
}
