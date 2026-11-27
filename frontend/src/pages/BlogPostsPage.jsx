import * as blogPostsApi from "../api/blogposts.js";
import BlogPostCard from "../components/BlogPostCard.jsx";
import BlogPostForm from "../components/BlogPostForm.jsx";
import ResourceManager from "../components/ResourceManager.jsx";

const api = {
  getAll: blogPostsApi.getBlogPosts, create: blogPostsApi.createBlogPost,
  update: blogPostsApi.updateBlogPost, remove: blogPostsApi.deleteBlogPost,
};

export default function BlogPostsPage() {
  return <ResourceManager resourceKey="blogposts" title="Blog posts" eyebrow="DEVELOPMENT JOURNAL"
    description="Notes on building applications and understanding the code behind them."
    label="blog post" api={api} Form={BlogPostForm}
    renderCard={(post, actions) => <BlogPostCard key={post.id} post={post}>{actions}</BlogPostCard>} />;
}
