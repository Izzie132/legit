import ghyston from "@ghyston/eslint-config-ghyston";
import ghystonReact from "@ghyston/eslint-config-ghyston-react";
import vitest from "eslint-plugin-vitest";

export default [
  ...ghyston,
  ...ghystonReact,
  vitest.configs.recommended,
  { ignores: ["src/api/ApiClient.ts"] },
];
