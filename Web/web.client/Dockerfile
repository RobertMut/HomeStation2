#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM node:18-slim AS base
ENV NG_CLI_ANALYTICS="false"
WORKDIR "/app"
RUN npm install -g @angular/cli

FROM base AS publish
ARG TARGET_URL
ENV TARGET_URL=$TARGET_URL
COPY . .
WORKDIR "/app/web.client"
ARG BUILD_CONFIGURATION=Release
RUN npm install
RUN ng build --configuration production --optimization

FROM nginx:stable-alpine AS final
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=publish /app/dist/web.client/browser /usr/share/nginx/html