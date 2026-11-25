import { Link } from "react-router";
export default function NotFoundPage() {
  return <><p className="eyebrow">404</p><h1>Page not found</h1><p>This address does not match a page in the portfolio.</p><Link to="/projects">Browse projects</Link></>;
}
