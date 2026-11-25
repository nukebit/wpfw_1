import { apiRequest } from "./client.js";

export const getProjects = signal => apiRequest("/projects", { signal });
export const getProject = (id, signal) => apiRequest(`/projects/${id}`, { signal });
