# Multi-Tenant Job Application Tracker

A production-ready ASP.NET Core 8 Web API for managing job applications across multiple organizations. Built with Clean Architecture principles, this system demonstrates enterprise-grade patterns including multi-tenancy, CQRS, and JWT authentication.

## Why This Project?

I built this as a showcase of modern .NET backend development practices. The goal was to create something that goes beyond a simple CRUD app — a system that handles real-world concerns like tenant isolation, role-based access, and proper separation of concerns.

The codebase reflects how I approach building maintainable software: keep the domain logic clean, push infrastructure details to the edges, and make the whole thing testable.

---

## Architecture

The solution follows Clean Architecture (sometimes called Onion or Hexagonal Architecture). Each layer has a clear responsibility:

```
┌─────────────────────────────────────────────────────────────┐
│                         API Layer                           │
│         Controllers, Middleware, Request/Response           │
├─────────────────────────────────────────────────────────────┤
│                    Infrastructure Layer                     │
│        EF Core, PostgreSQL, JWT, External Services          │
├─────────────────────────────────────────────────────────────┤
│                     Application Layer                       │
│            Use Cases (CQRS), DTOs, Interfaces               │
├─────────────────────────────────────────────────────────────┤
│                       Domain Layer                          │
│              Entities, Value Objects, Enums                 │
└─────────────────────────────────────────────────────────────┘
```

**Domain** contains business entities with zero external dependencies. No EF Core attributes, no framework coupling.

**Application** implements use cases using MediatR for CQRS. All business logic lives here, expressed through commands and queries.

**Infrastructure** handles the messy reality of databases, tokens, and external services. It implements interfaces defined in the Application layer.

**API** is just a thin layer that receives HTTP requests and dispatches them to MediatR handlers.

---

## Tech Stack

| Category | Technology |
|----------|------------|
| Framework | ASP.NET Core 8 |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer Tokens |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Testing | xUnit, Moq, FluentAssertions |
| Container | Docker & Docker Compose |

---

## Multi-Tenancy Approach

Each organization (tenant) gets complete data isolation through a shared database with discriminator columns. Every tenant-specific entity includes a `TenantId`, and EF Core global query filters ensure queries only return data belonging to the current tenant.

Tenant resolution happens in middleware:
1. First, check the `X-Tenant-Id` header
2. If not present, extract from the JWT `tenant_id` claim

This gives flexibility — public endpoints can specify tenant via header, while authenticated users automatically work within their tenant context.

---

## Authentication Flow

The system uses short-lived access tokens (1 hour) paired with long-lived refresh tokens (7 days).

**Registration:**
```
POST /api/auth/register
Header: X-Tenant-Id: {tenant-guid}
Body: { "email": "...", "password": "...", "firstName": "...", "lastName": "..." }

Response: { "accessToken": "...", "refreshToken": "...", "user": {...} }
```

**Login:**
```
POST /api/auth/login
Header: X-Tenant-Id: {tenant-guid}
Body: { "email": "...", "password": "..." }

Response: { "accessToken": "...", "refreshToken": "...", "user": {...} }
```

Passwords are hashed with BCrypt. The JWT contains user ID, email, role, and tenant ID — making it possible to authorize requests without hitting the database on every call.

**Role-Based Access:**
- `Admin` — Full system access
- `HR` — Manage jobs and review applications
- `Candidate` — Apply for jobs, view own applications

---

## Running the Project

### With Docker (recommended)

```bash
git clone https://github.com/AsilK/multi-tenant-job-tracker_alpha.git
cd multi-tenant-job-tracker_alpha

docker-compose up -d
```

That's it. The API will be available at `http://localhost:8080` with Swagger UI at the root.

### Local Development

If you prefer running without Docker:

```bash
# Start PostgreSQL
docker run -d --name postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=JobApplicationTracker \
  -p 5432:5432 \
  postgres:16-alpine

# Apply migrations
dotnet ef database update \
  --project src/JobApplicationTracker.Infrastructure \
  --startup-project src/JobApplicationTracker.API

# Run the API
cd src/JobApplicationTracker.API
dotnet run
```

API available at `http://localhost:5000`

---

## API Examples

### Create a Job Posting

```http
POST /api/jobs
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Senior .NET Developer",
  "description": "Join our team...",
  "department": "Engineering",
  "location": "Berlin / Remote",
  "type": "FullTime",
  "minSalary": 70000,
  "maxSalary": 95000
}
```

### List Jobs (public endpoint)

```http
GET /api/jobs?pageSize=10&department=Engineering
X-Tenant-Id: {tenant-guid}
```

### Get Job Details

```http
GET /api/jobs/{id}
X-Tenant-Id: {tenant-guid}
```

---

## Project Structure

```
src/
├── JobApplicationTracker.Domain/        # Entities, enums, domain exceptions
├── JobApplicationTracker.Application/   # CQRS handlers, DTOs, interfaces
├── JobApplicationTracker.Infrastructure/# EF Core, JWT, PostgreSQL
└── JobApplicationTracker.API/           # Controllers, middleware

tests/
└── JobApplicationTracker.Application.Tests/  # Unit tests with xUnit & Moq
```

---

## Running Tests

```bash
dotnet test
```

Currently covers:
- User registration (success + validation failures)
- User login (correct credentials, wrong password, deactivated accounts)
- Job creation (authorization, tenant context)

---

## Configuration

Environment variables (or `appsettings.json`):

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `Jwt__Secret` | Signing key (min 32 chars) |
| `Jwt__Issuer` | Token issuer identifier |
| `Jwt__Audience` | Token audience identifier |

---

## What's Next

This is a foundation. Future additions could include:
- Application submission endpoints
- Interview scheduling
- Email notifications
- Admin dashboard
- Rate limiting

---

## Contact

Built by Asil Karasu

Feel free to explore the code, raise issues, or reach out with questions.
