import assert from "node:assert/strict";
import { afterEach, test } from "node:test";
import { apiRequest, ApiError } from "../src/api/client.js";
import { createProject, updateProject, deleteProject } from "../src/api/projects.js";
import { createBlogPost, updateBlogPost, deleteBlogPost } from "../src/api/blogposts.js";

const originalFetch = globalThis.fetch;
afterEach(() => { globalThis.fetch = originalFetch; });

test("JSON writes include the cookie and serialize the DTO", async () => {
  globalThis.fetch = async (url, options) => {
    assert.equal(url, "/api/projects");
    assert.equal(options.credentials, "include");
    assert.equal(options.headers["Content-Type"], "application/json");
    assert.deepEqual(JSON.parse(options.body), { title: "Example" });
    return new Response(JSON.stringify({ id: 7 }), { status: 201 });
  };
  assert.deepEqual(await createProject({ title: "Example" }), { id: 7 });
});

test("registration and updates accept successful empty responses", async () => {
  globalThis.fetch = async () => new Response(null, { status: 201 });
  assert.equal(await apiRequest("/auth/register", { method: "POST", body: {} }), null);
  globalThis.fetch = async () => new Response(null, { status: 204 });
  assert.equal(await apiRequest("/projects/1", { method: "PUT", body: {} }), null);
});

test("401 and 403 retain status and explain the permission problem", async () => {
  for (const status of [401, 403]) {
    globalThis.fetch = async () => new Response(null, { status });
    await assert.rejects(apiRequest("/projects", { method: "POST", body: {} }), error =>
      error instanceof ApiError && error.status === status &&
      (status === 401 ? /sign in/i : /permission/i).test(error.message));
  }
});

test("server field errors remain available to the form", async () => {
  const errors = { Title: ["Title is required."] };
  globalThis.fetch = async () => new Response(JSON.stringify({ errors }), { status: 400 });
  await assert.rejects(apiRequest("/projects"), error =>
    error.status === 400 && error.errors.Title[0] === errors.Title[0]);
});

test("aborted detail requests preserve AbortError and their signal", async () => {
  const controller = new AbortController();
  globalThis.fetch = async (_url, options) => {
    assert.equal(options.signal, controller.signal);
    throw new DOMException("Aborted", "AbortError");
  };
  await assert.rejects(apiRequest("/projects/1", { signal: controller.signal }), { name: "AbortError" });
});

test("network failures explain how to retry", async () => {
  globalThis.fetch = async () => { throw new TypeError("offline"); };
  await assert.rejects(apiRequest("/projects"), error => error.status === 0 && /backend/.test(error.message));
});

test("both resources use the correct POST PUT DELETE routes", async () => {
  const requests = [];
  globalThis.fetch = async (url, options) => {
    requests.push([url, options.method]);
    return new Response(null, { status: 204 });
  };
  await createProject({}); await updateProject(2, {}); await deleteProject(2);
  await createBlogPost({}); await updateBlogPost(3, {}); await deleteBlogPost(3);
  assert.deepEqual(requests, [
    ["/api/projects", "POST"], ["/api/projects/2", "PUT"], ["/api/projects/2", "DELETE"],
    ["/api/blogposts", "POST"], ["/api/blogposts/3", "PUT"], ["/api/blogposts/3", "DELETE"],
  ]);
});
