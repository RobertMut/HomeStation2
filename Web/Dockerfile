#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
RUN apt-get -y update
RUN apt-get -y install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get -y install nodejs libkrb5-dev
RUN npm install -y -g @angular/cli

FROM base AS publish
WORKDIR "/app"
COPY . .
ARG BUILD_CONFIGURATION=Release
RUN dotnet restore
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
ARG ConnectionString
WORKDIR "/app"
COPY --from=publish "/app/publish" .
ENV Database__ConnectionString=$ConnectionString
ENTRYPOINT ["dotnet", "HomeStation.Web.dll"]