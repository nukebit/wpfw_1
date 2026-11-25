import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  // Proxy API calls through one browser origin for the Identity cookie.
  server: {
    host: "127.0.0.1", port: 5173, strictPort: true,
    proxy: { "/api": { target: "http://localhost:5074", changeOrigin: true } },
  },
});
