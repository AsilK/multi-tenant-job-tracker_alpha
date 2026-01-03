# Job Application Tracking System

A multi-tenant ASP.NET Core Web API for tracking job applications, built with Clean Architecture principles.

## 🏗️ Architecture

This project follows **Clean Architecture** with four main layers:

```
┌─────────────────────────────────────────────────────────┐
│                      API Layer                          │
│  (Controllers, Middleware, Swagger)                     │
├─────────────────────────────────────────────────────────┤
│                 Application Layer                       │
│  (CQRS Commands/Queries, DTOs, Validators)              │
├─────────────────────────────────────────────────────────┤
│                Infrastructure Layer                     │
│  (EF Core, PostgreSQL, JWT, Repositories)               │
├─────────────────────────────────────────────────────────┤
│                    Domain Layer                         │
│  (Entities, Enums, Exceptions, Interfaces)              │
└─────────────────────────────────────────────────────────┘
```

## 🚀 Features

- **Multi-Tenancy**: Data isolation using TenantId discriminator with EF Core global query filters
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Role-Based Authorization**: Admin, HR, and Candidate roles with different permissions
- **CQRS Pattern**: Separate read/write operations using MediatR
- **Validation**: Request validation with FluentValidation
- **Docker Support**: Containerized deployment with PostgreSQL

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker & Docker Compose](https://www.docker.com/get-started)
- [PostgreSQL](https://www.postgresql.org/) (or use Docker)

## 🛠️ Getting Started

### Using Docker (Recommended)

```bash
# Start the application and database
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

The API will be available at: http://localhost:8080

### Local Development

1. **Start PostgreSQL** (or use Docker):
   ```bash
   docker run -d --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:16-alpine
   ```

2. **Apply Database Migrations**:
   ```bash
   cd src/JobApplicationTracker.API
   dotnet ef database update --project ../JobApplicationTracker.Infrastructure
   ```

3. **Run the API**:
   ```bash
   dotnet run
   ```

## 📚 API Documentation

Swagger UI is available at the root URL when running in Development mode:
- **Local**: http://localhost:5000
- **Docker**: http://localhost:8080

### Key Endpoints

| Endpoint | Method | Description | Auth Required |
|----------|--------|-------------|---------------|
| `/api/auth/register` | POST | Register a new user | No (X-Tenant-Id required) |
| `/api/auth/login` | POST | Login and get tokens | No (X-Tenant-Id required) |
| `/api/jobs` | GET | List jobs (paginated) | No |
| `/api/jobs/{id}` | GET | Get job details | No |
| `/api/jobs` | POST | Create a job posting | Yes (Admin/HR) |

### Multi-Tenancy

Include the tenant ID in request headers:
```
X-Tenant-Id: your-tenant-guid-here
```

### Authentication

Include the JWT token in the Authorization header:
```
Authorization: Bearer your-jwt-token-here
```

## 🗃️ Database Schema

```
Tenants
├── Users (Admin, HR, Candidate)
├── Jobs
│   └── Applications
│       ├── ApplicationStatuses (history)
│       └── Interviews
```

## 👥 User Roles

| Role | Permissions |
|------|-------------|
| **Admin** | Full access within tenant |
| **HR** | Manage jobs, view/update applications, schedule interviews |
| **Candidate** | Apply for jobs, view own applications |

## 🧪 Running Tests

```bash
dotnet test
```

## 📁 Project Structure

```
JobApplicationTracker/
├── src/
│   ├── JobApplicationTracker.Domain/        # Core domain entities
│   ├── JobApplicationTracker.Application/   # Business logic (CQRS)
│   ├── JobApplicationTracker.Infrastructure/# Data access, external services
│   └── JobApplicationTracker.API/           # Controllers, middleware
├── tests/
│   └── ...
├── docker-compose.yml
├── Dockerfile
└── README.md
```

## 🔧 Configuration

Configuration is stored in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=JobApplicationTracker;..."
  },
  "Jwt": {
    "Secret": "your-secret-key-at-least-32-chars",
    "Issuer": "JobApplicationTracker",
    "Audience": "JobApplicationTrackerUsers"
  }
}
```

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

Built with ❤️ using ASP.NET Core 8, Entity Framework Core, and PostgreSQL
