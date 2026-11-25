import { useCallback } from "react";
import { Link, useParams } from "react-router";
import { getProject } from "../api/projects.js";
import { ApiError } from "../api/client.js";
import ResourceStatus from "../components/ResourceStatus.jsx";
import useApiResource from "../hooks/useApiResource.js";

export default function ProjectPage() {
  const { id } = useParams();
  const load = useCallback(signal => /^\d+$/.test(id) && Number(id) > 0 && Number(id) <= 2147483647
    ? getProject(id, signal) : Promise.reject(new ApiError(404, "Project not found.")), [id]);
  const { data: project, status, error, reload } = useApiResource(load, id);
  return <>
    <Link to="/projects">← All projects</Link>
    <ResourceStatus status={status} error={error} onRetry={reload} label="project" />
    {status === "success" && <article className="prose"><p className="eyebrow">{project.type}</p><h1>{project.title}</h1><p className="content">{project.description}</p></article>}
  </>;
}
