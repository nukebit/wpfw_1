import { useCallback } from "react";
import { Link, useParams } from "react-router";
import { getBlogPost } from "../api/blogposts.js";
import { ApiError } from "../api/client.js";
import ResourceStatus from "../components/ResourceStatus.jsx";
import useApiResource from "../hooks/useApiResource.js";

export default function BlogPostPage() {
  const { id } = useParams();
  const load = useCallback(signal => /^\d+$/.test(id) && Number(id) > 0 && Number(id) <= 2147483647
    ? getBlogPost(id, signal) : Promise.reject(new ApiError(404, "Blog post not found.")), [id]);
  const { data: post, status, error, reload } = useApiResource(load, id);
  return <>
    <Link to="/blogposts">← All blog posts</Link>
    <ResourceStatus status={status} error={error} onRetry={reload} label="blog post" />
    {status === "success" && <article className="prose"><p className="eyebrow">{post.category}</p><h1>{post.title}</h1><p className="meta">Published <time dateTime={post.publishedOn}>{post.publishedOn}</time></p><div className="content">{post.content}</div></article>}
  </>;
}
