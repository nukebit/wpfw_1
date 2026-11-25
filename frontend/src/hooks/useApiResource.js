import { useEffect, useState } from "react";

export default function useApiResource(load, resourceKey) {
  const [result, setResult] = useState({ key: null, status: "loading", data: null, error: null });
  const [attempt, setAttempt] = useState(0);
  useEffect(() => {
    const controller = new AbortController();
    setResult({ key: resourceKey, status: "loading", data: null, error: null });
    load(controller.signal).then(data => {
      if (!controller.signal.aborted) {
        setResult({ key: resourceKey, status: Array.isArray(data) && !data.length ? "empty" : "success", data, error: null });
      }
    }).catch(error => {
      if (!controller.signal.aborted) {
        setResult({ key: resourceKey, status: error.status === 404 ? "notfound" : "error", data: null, error });
      }
    });
    // Discard an older request when the selected id changes or the page closes.
    return () => controller.abort();
  }, [load, resourceKey, attempt]);
  return {
    ...(result.key === resourceKey ? result : { status: "loading", data: null, error: null }),
    reload: () => setAttempt(value => value + 1),
  };
}
