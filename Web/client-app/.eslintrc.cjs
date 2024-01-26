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
    // Can be removed once this PR is merged: https://github.com/GhystonSoftware/eslint-config-ghyston/pull/52
    "@typescript-eslint/no-confusing-void-expression": [
      "warn",
      { ignoreArrowShorthand: true },
    ],
    // Can be removed once this PR is merged: https://github.com/GhystonSoftware/eslint-config-ghyston/pull/45
    "@typescript-eslint/promise-function-async": "off",
  },
  overrides: [
    {
      files: "*.{jsx,tsx}",
      rules: {
        // Can be removed once this PR is merged: https://github.com/GhystonSoftware/eslint-config-ghyston/pull/54
        "@typescript-eslint/explicit-module-boundary-types": "off",
      },
    },
  ],
};
