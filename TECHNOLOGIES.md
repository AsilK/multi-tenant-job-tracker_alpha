# Behind the Tech: Stack & Architecture

This document goes into the details of the technology choices I made for the Multi-Tenant Job Application Tracker. Instead of just listing tools, I'll explain the reasoning behind the architecture and why these specific components were chosen to build a production-ready system.

## The Core: .NET 8
I chose .NET 8 as the backbone of this project because it's currently the most performant and stable version of the framework. For a backend that handles multi-tenancy and complex business logic, .NET's strong typing and robust dependency injection system are invaluable. It allows the API to be fast, cross-platform, and highly maintainable.

## Architectural Choice: Clean Architecture
One of the main goals for this project was long-term maintainability. I implemented Clean Architecture (or Onion Architecture) to ensure that the core business logic remains independent of external tools like databases or UI frameworks.

The solution is split into four distinct layers. The **Domain** stands at the heart of the application, containing entities and business rules with zero external dependencies. The **Application** layer is where the use cases live, coordinating data flow using the CQRS pattern. **Infrastructure** handles the outside world, including databases, authentication, and external services, while the **API** serves as a thin entry point for incoming HTTP requests.

## Data Isolation & Multi-Tenancy
For a SaaS-style application, data isolation is non-negotiable. I opted for a shared database with discriminator columns to manage this. I used Entity Framework Core 8 not just as an ORM, but to enforce security. By using Global Query Filters, the system automatically filters every database query by the current `TenantId`. This approach prevents developers from accidentally leaking data between organizations. For the database itself, PostgreSQL was the obvious choice due to its proven reliability and advanced feature set.

## Application Patterns & Logic
I decided to leverage MediatR to implement the CQRS pattern, which allowed me to pull the logic out of controllers and into dedicated handlers. This keeps the code much cleaner and ensures that each operation, like adding a job or registering a user, has its own isolated, testable class.

Instead of cluttering these handlers with manual checks, I used FluentValidation to define rules for incoming data. It sits directly in the MediatR pipeline and stops invalid requests before they even touch the business logic. I also brought in AutoMapper to handle the transformation between database entities and the DTOs we send back to the API, which keeps the mapping code concise and less error-prone.

## Security & Identity
To keep the API stateless and scalable, I implemented JWT authentication. The tokens carry the user’s role and tenant context, allowing the system to authorize requests without hitting the database every time. Proper security also means never storing plain-text passwords, so I used BCrypt to ensure all passwords are salted and hashed using modern standards.

## DevOps & Environment
I wanted this project to be "clonable and runnable" in seconds. By containerizing both the API and the database with Docker and Docker Compose, I ensured that anyone can get the system running locally with a single command, matching the production environment perfectly.

## Testing & Documentation
Professional software requires proof that it works, so I wrote unit tests for the core logic using xUnit and Moq. This allows for verifying business rules in isolation from the database. Finally, I used Swagger to provide an interactive portal where developers can explore and test the API endpoints directly from the browser.
