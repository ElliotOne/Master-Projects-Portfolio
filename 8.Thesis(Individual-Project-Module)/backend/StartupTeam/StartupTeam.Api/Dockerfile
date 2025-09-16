# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["StartupTeam.Api/StartupTeam.Api.csproj", "StartupTeam.Api/"]
COPY ["Modules/StartupTeam.Module.JobManagement/StartupTeam.Module.JobManagement.csproj", "Modules/StartupTeam.Module.JobManagement/"]
COPY ["Modules/StartupTeam.Module.UserManagement/StartupTeam.Module.UserManagement.csproj", "Modules/StartupTeam.Module.UserManagement/"]
COPY ["Shared/StartupTeam.Shared/StartupTeam.Shared.csproj", "Shared/StartupTeam.Shared/"]
COPY ["Modules/StartupTeam.Module.MatchingManagement/StartupTeam.Module.MatchingManagement.csproj", "Modules/StartupTeam.Module.MatchingManagement/"]
COPY ["Modules/StartupTeam.Module.PortfolioManagement/StartupTeam.Module.PortfolioManagement.csproj", "Modules/StartupTeam.Module.PortfolioManagement/"]
COPY ["Modules/StartupTeam.Module.TeamManagement/StartupTeam.Module.TeamManagement.csproj", "Modules/StartupTeam.Module.TeamManagement/"]
RUN dotnet restore "./StartupTeam.Api/./StartupTeam.Api.csproj"

# Copy source code and build the application
COPY . .
WORKDIR "/src/StartupTeam.Api"
RUN dotnet build "./StartupTeam.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StartupTeam.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StartupTeam.Api.dll"]