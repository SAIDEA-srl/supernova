# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["examples/EuracMonitoringService/EuracMonitoringService.csproj", "examples/EuracMonitoringService/"]
COPY ["models/dotnet/OrangeButton.Models/OrangeButton.Models.csproj", "models/dotnet/OrangeButton.Models/"]
COPY ["models/dotnet/Supernova.Messages.Models/Supernova.Messages.Models.csproj", "models/dotnet/Supernova.Messages.Models/"]
COPY ["models/dotnet/Supernova.Models/Supernova.Models.csproj", "models/dotnet/Supernova.Models/"]
RUN dotnet restore "./examples/EuracMonitoringService/EuracMonitoringService.csproj"
COPY . .
WORKDIR "/src/examples/EuracMonitoringService"
RUN dotnet build "./EuracMonitoringService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EuracMonitoringService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN mkdir -p /app/publish/data/reports
RUN chown $APP_UID -R /app/publish/data

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EuracMonitoringService.dll"]