#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR "/app"
EXPOSE $WEB_SSL_PORT
EXPOSE $WEB_HTTP_PORT
ENV SSL_PORT=5215
ENV HTTP_PORT=5216
EXPOSE $MQTT__Port
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get -y install nodejs
RUN npm install -g @angular/cli

FROM base AS publish
COPY . "/app/src"
WORKDIR "/app/src"
ARG BUILD_CONFIGURATION=Release
RUN dotnet restore
RUN dotnet publish ./HomeStation.Web/HomeStation.Web.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR "/app"
COPY --from=publish "/app/publish" .
ENTRYPOINT ["dotnet", "HomeStation.Web.dll"]