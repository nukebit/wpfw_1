import { Link } from "react-router";

export default function BlogPostCard({ post, children }) {
  return <article className="card">
    <span className="meta">{post.category} · <time dateTime={post.publishedOn}>{post.publishedOn}</time></span>
    <h2>{post.title}</h2>
    <p>{post.content.length > 180 ? post.content.slice(0, 180) + "…" : post.content}</p>
    <Link className="card-link" to={`/blogposts/${post.id}`}>Read post →</Link>
    {children}
  </article>;
}
