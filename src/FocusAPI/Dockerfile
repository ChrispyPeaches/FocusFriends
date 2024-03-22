#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Set environment variable for ASP.NET Core environment
ENV ASPNETCORE_ENVIRONMENT=https

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FocusAPI/FocusAPI.csproj", "src/FocusAPI/"]
COPY ["src/FocusCore/FocusCore.csproj", "src/FocusCore/"]
RUN dotnet restore "./src/FocusAPI/./FocusAPI.csproj"
COPY . .
WORKDIR "/src/src/FocusAPI"
RUN dotnet build "./FocusAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FocusAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FocusAPI.dll"]

# Copy SSL certificate and private key
COPY ["certs/domain.cert.pem", "/app/certs/"]
COPY ["certs/private.key.pem", "/app/certs/"]

# Set entrypoint with SSL certificate and private key paths
ENTRYPOINT ["dotnet", "FocusAPI.dll", "--ssl-cert=/app/certs/domain.cert.pem", "--ssl-key=/app/certs/private.key.pem"]