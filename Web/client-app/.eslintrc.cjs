module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  plugins: ["vitest"],
  extends: [
    "plugin:vitest/recommended",
    "@ghyston/eslint-config-ghyston",
    "@ghyston/eslint-config-ghyston-react",
  ],
  parserOptions: {
    project: "tsconfig.json",
  },
  rules: {
    // This should be turned on for projects, and he unused modules either used or removed from the codebase
    "import/no-unused-modules": "off",
  },
};
