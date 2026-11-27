import { createContext, useCallback, useContext, useEffect, useRef, useState } from "react";
import * as authApi from "../api/auth.js";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const requestVersion = useRef(0);

  const refreshUser = useCallback(async signal => {
    const version = ++requestVersion.current;
    setError("");
    try {
      const current = await authApi.getMe(signal);
      if (!signal?.aborted && version === requestVersion.current) setUser(current);
      return current;
    } catch (error) {
      if (signal?.aborted || version !== requestVersion.current) return;
      if (error.status === 401) { setUser(null); return null; }
      setError(error.message);
      throw error;
    } finally {
      if (!signal?.aborted && version === requestVersion.current) setLoading(false);
    }
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    refreshUser(controller.signal).catch(() => {});
    return () => controller.abort();
  }, [refreshUser]);

  async function login(credentials) {
    await authApi.login(credentials);
    await refreshUser();
  }

  async function logout() {
    try { await authApi.logout(); }
    catch (error) { if (error.status !== 401) throw error; }
    // Invalidate an earlier session request before removing the user.
    requestVersion.current += 1;
    setUser(null);
    setError("");
  }

  const canEdit = user?.roles.some(role => ["Editor", "Admin"].includes(role)) ?? false;
  return <AuthContext.Provider value={{ user, loading, error, canEdit, login, logout, refreshUser }}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used inside AuthProvider.");
  return context;
}
