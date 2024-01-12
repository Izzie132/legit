/// <reference types="vitest" />

import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";
import tsconfigPaths from "vite-tsconfig-paths";
import svgr from "vite-plugin-svgr";
import basicSsl from "@vitejs/plugin-basic-ssl";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), tsconfigPaths(), svgr(), basicSsl()],
  server: {
    port: 3000,
    strictPort: true,
    proxy: {
      "/api": { target: "https://localhost:5000", secure: false },
    },
  },
  test: {
    environment: "jsdom",
    setupFiles: ["./setupTests.ts"],
  },
});
