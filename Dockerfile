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

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get update \
    && apt-get install -y nodejs \
    && node --version

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

# Copy published app files
COPY --from=backend-builder /app/publish .

# Copy certificate into container
COPY cert/simplecameraupload.client.pfx ./cert/simplecameraupload.client.pfx

# Set environment variables for Kestrel HTTPS binding
ENV ASPNETCORE_URLS="https://+ :5010"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/cert/simplecameraupload.client.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"

EXPOSE 5010

ENTRYPOINT ["dotnet", "SimpleCameraUpload.Server.dll"]
