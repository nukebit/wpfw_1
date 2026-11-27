import assert from "node:assert/strict";
import { test } from "node:test";
import { projectFields, blogPostFields, validateFields } from "../src/components/contentFields.js";

test("project form rejects whitespace and values beyond DTO limits", () => {
  const errors = validateFields(projectFields, { title: " ", type: "x".repeat(51), description: "Valid" });
  assert.ok(errors.title);
  assert.ok(errors.type);
  assert.equal(errors.description, undefined);
});

test("blog form rejects impossible dates and requires content", () => {
  const errors = validateFields(blogPostFields, { title: "Entry", category: "Learning", content: "", publishedOn: "2026-02-30" });
  assert.ok(errors.content);
  assert.ok(errors.publishedOn);
});

test("valid project and blog DTOs pass frontend validation", () => {
  assert.deepEqual(validateFields(projectFields, { title: "Project", type: "Web", description: "Description" }), {});
  assert.deepEqual(validateFields(blogPostFields, { title: "Post", category: "Development", content: "Notes", publishedOn: "2026-11-27" }), {});
});
