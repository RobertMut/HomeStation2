var process = require('process');

const PROXY_CONFIG = [
  {
    context: [
      "/api/**",
    ],
    target: process.env.TARGET_URL || "http://homestationApi/",
    secure: false,
  }
]

module.exports = PROXY_CONFIG;
