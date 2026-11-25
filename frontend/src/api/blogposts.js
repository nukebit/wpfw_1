import { apiRequest } from "./client.js";

export const getBlogPosts = signal => apiRequest("/blogposts", { signal });
export const getBlogPost = (id, signal) => apiRequest(`/blogposts/${id}`, { signal });
