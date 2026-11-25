import { getProjects } from "../api/projects.js";
import ProjectCard from "../components/ProjectCard.jsx";
import ResourceStatus from "../components/ResourceStatus.jsx";
import useApiResource from "../hooks/useApiResource.js";

export default function ProjectsPage() {
  const { data: projects, status, error, reload } = useApiResource(getProjects, "projects");
  return <>
    <p className="eyebrow">SELECTED WORK</p><h1>Projects</h1>
    <p className="lead">Applications, technical choices and things I have learned.</p>
    <ResourceStatus status={status} error={error} onRetry={reload} label="projects" />
    {status === "success" && <section className="cards" aria-label="Projects">{projects.map(project => <ProjectCard key={project.id} project={project} />)}</section>}
  </>;
}
