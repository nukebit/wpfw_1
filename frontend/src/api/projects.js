import { apiRequest } from "./client.js";

export const getProjects = signal => apiRequest("/projects", { signal });
export const getProject = (id, signal) => apiRequest(`/projects/${id}`, { signal });
export const createProject = body => apiRequest("/projects", { method: "POST", body });
export const updateProject = (id, body) => apiRequest(`/projects/${id}`, { method: "PUT", body });
export const deleteProject = id => apiRequest(`/projects/${id}`, { method: "DELETE" });
