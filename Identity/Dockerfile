# Base image for running the app (.NET 10 preview)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage (.NET 10 preview SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all referenced project files so restore works correctly
COPY ["Identity/Identity.csproj", "Identity/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["ServiceDefaults/ServiceDefaults.csproj", "ServiceDefaults/"]

RUN dotnet restore "./Identity/Identity.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/Identity"

RUN dotnet build "./Identity.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.dll"]
