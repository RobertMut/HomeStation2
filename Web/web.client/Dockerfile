#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM node:18-slim AS base
ENV NG_CLI_ANALYTICS="false"
WORKDIR "/app"
EXPOSE $WEB_HTTP_PORT
RUN npm install -g @angular/cli

FROM base AS publish
COPY ./ ./
WORKDIR "/app"
ARG BUILD_CONFIGURATION=Release
RUN npm install
RUN ng build --configuration production

FROM publish AS final
WORKDIR "/app"
ENTRYPOINT ["ng", "serve", "--port", "8443", "--host", "0.0.0.0", "--public-host", "homestation.robertmut.ovh", "--disable-host-check"]
