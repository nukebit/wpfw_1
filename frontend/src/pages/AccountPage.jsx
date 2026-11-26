import { useState } from "react";
import { Link, useNavigate } from "react-router";
import { register } from "../api/auth.js";
import { useAuth } from "../auth/AuthContext.jsx";

export default function AccountPage({ mode }) {
  const registering = mode === "register";
  const { login, loading } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmation, setConfirmation] = useState("");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);

  async function submit(event) {
    event.preventDefault();
    setError("");
    if (registering && password !== confirmation) { setError("The passwords do not match."); return; }
    setBusy(true);
    try {
      if (registering) {
        await register({ email: email.trim(), password });
        setPassword(""); setConfirmation("");
        navigate("/login", { state: { message: "Your account was created with the Gast role. You can now sign in." } });
      } else {
        await login({ email: email.trim(), password });
        setPassword("");
        navigate("/projects", { state: { message: "You have signed in successfully." } });
      }
    } catch (error) {
      setError(error.status === 401 ? "Invalid email or password. Please try again." : error.message);
    } finally { setBusy(false); }
  }

  return <>
    <p className="eyebrow">YOUR ACCOUNT</p><h1>{registering ? "Create an account" : "Sign in"}</h1>
    {registering && <p>New accounts receive the Gast role and can view the portfolio.</p>}
    <form onSubmit={submit} aria-label={registering ? "Registration" : "Sign in"} aria-busy={busy}>
      {error && <p className="notice error" role="alert">{error}</p>}
      <div className="field"><label htmlFor="email">Email</label><input id="email" type="email" maxLength="256" autoComplete="username" required value={email} onChange={event => setEmail(event.target.value)} /></div>
      <div className="field"><label htmlFor="password">Password</label><input id="password" type="password" autoComplete={registering ? "new-password" : "current-password"} minLength={registering ? 6 : undefined} required value={password} onChange={event => setPassword(event.target.value)} aria-describedby={registering ? "password-help" : undefined} />
        {registering && <p id="password-help" className="hint">Use at least 6 characters, including an uppercase letter, lowercase letter, digit and symbol.</p>}
      </div>
      {registering && <div className="field"><label htmlFor="confirmation">Confirm password</label><input id="confirmation" type="password" autoComplete="new-password" required value={confirmation} onChange={event => setConfirmation(event.target.value)} /></div>}
      <button type="submit" disabled={busy || loading}>{busy ? "Please wait…" : registering ? "Create account" : "Sign in"}</button>
    </form>
    <Link to={registering ? "/login" : "/register"}>{registering ? "Already have an account? Sign in" : "Need an account? Register"}</Link>
  </>;
}
