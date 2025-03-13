#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM node:18-slim AS base
ENV NG_CLI_ANALYTICS="false"
WORKDIR "/app"
RUN npm install -g @angular/cli

FROM base AS publish
ARG TARGET_URL
ARG HREF
ENV TARGET_URL=$TARGET_URL
COPY . .
WORKDIR "/app/web.client"
ARG BUILD_CONFIGURATION=Release
RUN npm install
RUN ng build --configuration production --optimization --base-href $HREF

FROM nginx:stable-alpine AS final
ARG HREF
RUN rm -rf /etc/nginx/nginx.conf
COPY ./nginx.conf /etc/nginx/nginx.conf
RUN sed "s|%ARGLOCATION%|$HREF|g" /etc/nginx/nginx.conf
COPY --from=publish /app/dist/web.client/browser /usr/share/nginx/html$HREF