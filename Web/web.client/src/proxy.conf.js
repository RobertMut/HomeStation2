const PROXY_CONFIG = [
  {
    context: [
      "/api/**",
    ],
    target: "http://homestationApi:9180/",
    secure: false,
  }
]

module.exports = PROXY_CONFIG;
