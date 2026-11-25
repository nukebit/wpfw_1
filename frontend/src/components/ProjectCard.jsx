import { Link } from "react-router";

export default function ProjectCard({ project, children }) {
  return <article className="card">
    <span className="meta">{project.type}</span>
    <h2>{project.title}</h2>
    <p>{project.description.length > 180 ? project.description.slice(0, 180) + "…" : project.description}</p>
    <Link className="card-link" to={`/projects/${project.id}`}>View project →</Link>
    {children}
  </article>;
}
