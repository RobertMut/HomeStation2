const { env } = require('process');

const target = env.TARGET_URL

const PROXY_CONFIG = [
  {
    context: [
      "/api/**",
    ],
    target: target,
    secure: false
  }
]

module.exports = PROXY_CONFIG;
