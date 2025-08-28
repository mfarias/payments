# Use the official .NET 9 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["Payments.Api/Payments.Api.csproj", "Payments.Api/"]
COPY ["Payments/Payments.csproj", "Payments/"]

# Restore dependencies
RUN dotnet restore "Payments.Api/Payments.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Payments.Api"
RUN dotnet build "Payments.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Payments.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Payments.Api.dll"]
