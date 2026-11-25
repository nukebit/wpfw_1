import { useParams } from "react-router";
export default function ProjectPage() {
  const { id } = useParams();
  return <><h1>Project</h1><p role="status">Loading project {id}.</p></>;
}
