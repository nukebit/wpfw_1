import { apiRequest } from "./client.js";

export const register = body => apiRequest("/auth/register", { method: "POST", body });
export const login = body => apiRequest("/auth/login", { method: "POST", body });
export const logout = () => apiRequest("/auth/logout", { method: "POST" });
export const getMe = signal => apiRequest("/auth/me", { signal });
