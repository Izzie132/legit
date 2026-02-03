// https://docs.expo.dev/guides/using-eslint/
const { defineConfig } = require("eslint/config");
const expoConfig = require("eslint-config-expo/flat");

// ToDo isd - use the ghyston config
module.exports = defineConfig([
  expoConfig,
  {
    ignores: ["dist/*"],
  },
]);
