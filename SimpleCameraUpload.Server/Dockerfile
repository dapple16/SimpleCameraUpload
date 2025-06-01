# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

## This stage is used when running from VS in fast mode (Default for Debug configuration)
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#
## This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS with-node
#RUN apt-get update
#RUN apt-get install curl
#RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
#RUN apt-get -y install nodejs
#
#
#FROM with-node AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["SimpleCameraUpload.Server/SimpleCameraUpload.Server.csproj", "SimpleCameraUpload.Server/"]
#COPY ["simplecameraupload.client/simplecameraupload.client.esproj", "simplecameraupload.client/"]
#RUN dotnet restore "./SimpleCameraUpload.Server/SimpleCameraUpload.Server.csproj"
#COPY . .
#WORKDIR "/src/SimpleCameraUpload.Server"
#RUN dotnet build "./SimpleCameraUpload.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./SimpleCameraUpload.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "SimpleCameraUpload.Server.dll"]

# Stage 1: Build the React frontend
FROM node:20 AS frontend-builder

WORKDIR /app

# Copy only the frontend folder into container
COPY simplecameraupload.client/ ./ClientApp/

WORKDIR /app/ClientApp

RUN npm install
RUN npm run build

# Stage 2: Build the .NET backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-builder

WORKDIR /src

# Copy only the backend project
COPY SimpleCameraUpload.Server/SimpleCameraUpload.Server.csproj ./SimpleCameraUpload.Server/

RUN dotnet restore ./SimpleCameraUpload.Server/SimpleCameraUpload.Server.csproj

# Copy the remaining source code
COPY . .

WORKDIR /src/SimpleCameraUpload.Server

# Copy React build into wwwroot (or another folder depending on your configuration)
COPY --from=frontend-builder /app/ClientApp/build ./wwwroot

RUN dotnet publish -c Release -o /app/publish

# Stage 3: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=backend-builder /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "SimpleCameraUpload.Server.dll"]