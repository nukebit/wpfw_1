export class ApiError extends Error {
  constructor(status, message, errors = {}) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.errors = errors;
  }
}

export async function apiRequest(path, { body, ...options } = {}) {
  let response;
  try {
    response = await fetch(`/api${path}`, {
      ...options,
      credentials: "include",
      headers: { ...(body !== undefined ? { "Content-Type": "application/json" } : {}), ...options.headers },
      ...(body !== undefined ? { body: JSON.stringify(body) } : {}),
    });
  } catch (error) {
    if (error.name === "AbortError") throw error;
    throw new ApiError(0, "The API could not be reached. Check that the backend is running and try again.");
  }

  // Registration (201) and successful updates (204) can have an empty body.
  const text = await response.text();
  let data = null;
  if (text) {
    try { data = JSON.parse(text); }
    catch {
      if (response.ok) throw new ApiError(response.status, "The API returned an unexpected response.");
    }
  }
  if (!response.ok) {
    const messages = {
      401: "You are not signed in, or your session has expired. Please sign in again.",
      403: "You do not have permission. Only an Editor or Admin can change portfolio content.",
      404: "This item could not be found. It may have been removed.",
      409: "An account with this email already exists.",
    };
    const validation = data?.errors ? Object.values(data.errors).flat().join(" ") : "";
    throw new ApiError(response.status,
      messages[response.status] || validation || data?.detail || data?.title || "The request failed. Please try again.",
      data?.errors ?? {});
  }
  return data;
}
