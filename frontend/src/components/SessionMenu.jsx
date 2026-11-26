import { useState } from "react";
import { Link } from "react-router";
import { useAuth } from "../auth/AuthContext.jsx";

export default function SessionMenu() {
  const { user, loading, logout } = useAuth();
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  async function handleLogout() {
    setBusy(true); setError(""); setMessage("");
    try { await logout(); setMessage("You have signed out."); }
    catch (error) { setError(error.message); }
    finally { setBusy(false); }
  }

  return <div className="session">
    {loading ? <span role="status">Checking session…</span> : user
      ? <><span>{user.email} · {user.roles.join(", ")}</span> <button className="secondary" type="button" disabled={busy} onClick={handleLogout}>{busy ? "Signing out…" : "Sign out"}</button></>
      : <div className="actions"><Link to="/login">Sign in</Link><Link to="/register">Create account</Link></div>}
    {error && <p role="alert">{error}</p>}
    {!user && message && <p role="status">{message}</p>}
  </div>;
}
