import { Link, NavLink, Route, Routes } from "react-router";
import HomePage from "./pages/HomePage.jsx";
import ProjectsPage from "./pages/ProjectsPage.jsx";
import ProjectPage from "./pages/ProjectPage.jsx";
import BlogPostsPage from "./pages/BlogPostsPage.jsx";
import BlogPostPage from "./pages/BlogPostPage.jsx";
import NotFoundPage from "./pages/NotFoundPage.jsx";
import AccountPage from "./pages/AccountPage.jsx";
import SessionMenu from "./components/SessionMenu.jsx";
import RouteMessage from "./components/RouteMessage.jsx";
import { useAuth } from "./auth/AuthContext.jsx";

export default function App() {
  const { error, refreshUser } = useAuth();
  return <>
    <a className="skip" href="#main-content">Skip to content</a>
    <header className="header"><div className="wrap header-content">
      <Link className="logo" to="/" aria-label="Brian Arjun home">BA<span>.</span></Link>
      <nav aria-label="Main navigation">
        <NavLink to="/" end>About me</NavLink>
        <NavLink to="/projects">Projects</NavLink>
        <NavLink to="/blogposts">Blog</NavLink>
      </nav>
      <SessionMenu />
    </div></header>
    <main className="wrap main" id="main-content" tabIndex="-1">
      {error && <div className="notice error" role="alert"><p>{error}</p><button type="button" onClick={() => refreshUser().catch(() => {})}>Check session again</button></div>}
      <RouteMessage />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/projects" element={<ProjectsPage />} />
        <Route path="/projects/:id" element={<ProjectPage />} />
        <Route path="/blogposts" element={<BlogPostsPage />} />
        <Route path="/blogposts/:id" element={<BlogPostPage />} />
        <Route path="/login" element={<AccountPage key="login" mode="login" />} />
        <Route path="/register" element={<AccountPage key="register" mode="register" />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </main>
    <footer className="footer"><div className="wrap">Brian Arjun <span>Software Engineering · WPFW portfolio</span></div></footer>
  </>;
}
