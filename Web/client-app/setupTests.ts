import { afterEach } from "vitest";
import { cleanup } from "@testing-library/react";
import "@testing-library/jest-dom/vitest";
import { clearToastsForTesting } from "./src/components/ui/use-toast";

// runs a cleanup after each test case (e.g. clearing jsdom)
afterEach(() => {
  clearToastsForTesting();
  cleanup();
});
