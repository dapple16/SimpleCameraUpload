# Stage 1: Build the React frontend
FROM node:20 AS frontend-builder

WORKDIR /app

COPY simplecameraupload.client ./simplecameraupload.client
WORKDIR /app/simplecameraupload.client
ENV DOCKER=true
RUN npm install
RUN npm run build

# Stage 2: Build the .NET backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-builder

WORKDIR /src

COPY SimpleCameraUpload.Server/SimpleCameraUpload.Server.csproj ./SimpleCameraUpload.Server/
RUN dotnet restore ./SimpleCameraUpload.Server/SimpleCameraUpload.Server.csproj

COPY . .

# ✅ Copy the frontend build output into backend's wwwroot
COPY --from=frontend-builder /app/simplecameraupload.client/build ./SimpleCameraUpload.Server/wwwroot

WORKDIR /src/SimpleCameraUpload.Server

RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=backend-builder /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "SimpleCameraUpload.Server.dll"]
