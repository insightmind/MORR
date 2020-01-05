module.exports = {
  "roots": [
    "<rootDir>/src"
  ],
  "testMatch": [
    "**/__tests__/**/*.+(ts|tsx|js)",
    "**/?(*.)+(spec|test).+(ts|tsx|js)"
  ],
  "collectCoverageFrom": [
    "src/**/*.+(ts|tsx|js|jsx)",
    "!**/node_modules/**",
    "!**/__mock__/**",
    "!**/index.ts"
  ],
  "transform": {
    "^.+\\.(ts|tsx)$": "ts-jest"
  },
  "setupFiles": [
    "./src/__mock__/client.ts"
  ],
  "testEnvironment" : "node"
}