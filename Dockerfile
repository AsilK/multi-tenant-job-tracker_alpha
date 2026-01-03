# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY JobApplicationTracker.sln .
COPY src/JobApplicationTracker.Domain/JobApplicationTracker.Domain.csproj src/JobApplicationTracker.Domain/
COPY src/JobApplicationTracker.Application/JobApplicationTracker.Application.csproj src/JobApplicationTracker.Application/
COPY src/JobApplicationTracker.Infrastructure/JobApplicationTracker.Infrastructure.csproj src/JobApplicationTracker.Infrastructure/
COPY src/JobApplicationTracker.API/JobApplicationTracker.API.csproj src/JobApplicationTracker.API/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
WORKDIR /src/src/JobApplicationTracker.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "JobApplicationTracker.API.dll"]
