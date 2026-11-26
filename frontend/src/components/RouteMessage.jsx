import { useLocation } from "react-router";

// Messages passed by navigation disappear after a full page reload.
export default function RouteMessage() {
  const { state } = useLocation();
  return state?.message ? <p className="notice success" role="status">{state.message}</p> : null;
}
