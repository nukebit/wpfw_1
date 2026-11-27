import { apiRequest } from "./client.js";

export const getBlogPosts = signal => apiRequest("/blogposts", { signal });
export const getBlogPost = (id, signal) => apiRequest(`/blogposts/${id}`, { signal });
export const createBlogPost = body => apiRequest("/blogposts", { method: "POST", body });
export const updateBlogPost = (id, body) => apiRequest(`/blogposts/${id}`, { method: "PUT", body });
export const deleteBlogPost = id => apiRequest(`/blogposts/${id}`, { method: "DELETE" });
