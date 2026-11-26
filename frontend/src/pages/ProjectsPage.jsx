import * as projectsApi from "../api/projects.js";
import ProjectCard from "../components/ProjectCard.jsx";
import ProjectForm from "../components/ProjectForm.jsx";
import ResourceManager from "../components/ResourceManager.jsx";

const api = {
  getAll: projectsApi.getProjects, create: projectsApi.createProject,
  update: projectsApi.updateProject, remove: projectsApi.deleteProject,
};

export default function ProjectsPage() {
  return <ResourceManager resourceKey="projects" title="Projects" eyebrow="SELECTED WORK"
    description="Applications, technical choices and things I have learned."
    label="project" api={api} Form={ProjectForm}
    renderCard={(project, actions) => <ProjectCard key={project.id} project={project}>{actions}</ProjectCard>} />;
}
