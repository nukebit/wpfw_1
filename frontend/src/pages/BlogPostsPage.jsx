import { getBlogPosts } from "../api/blogposts.js";
import BlogPostCard from "../components/BlogPostCard.jsx";
import ResourceStatus from "../components/ResourceStatus.jsx";
import useApiResource from "../hooks/useApiResource.js";

export default function BlogPostsPage() {
  const { data: posts, status, error, reload } = useApiResource(getBlogPosts, "blogposts");
  return <>
    <p className="eyebrow">DEVELOPMENT JOURNAL</p><h1>Blog posts</h1>
    <p className="lead">Notes on building applications and understanding the code behind them.</p>
    <ResourceStatus status={status} error={error} onRetry={reload} label="blog posts" />
    {status === "success" && <section className="cards" aria-label="Blog posts">{posts.map(post => <BlogPostCard key={post.id} post={post} />)}</section>}
  </>;
}
