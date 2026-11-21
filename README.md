## **Architecture Layers**

| Layer | Responsibility |
|---|---|
| Domain | Contains entities, value objects, enums, and business rules. |
| Application | Contains application logic, abstractions, validation, and use cases. |
| Infrastructure | Implements persistence, repositories, Identity, and external concerns. |
| API | Exposes RESTful endpoints and handles HTTP requests and responses. |
| UnitTests | Tests domain behavior and application logic. |

## **Technologies Used**

- **Language:** C#
- **Framework:** .NET 10
- **Backend:** ASP.NET Core Web API
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity & JWT
- **MediatR:** 12.5.0
- **Validation:** FluentValidation
- **API Documentation:** OpenAPI & Scalar
- **Testing:** xUnit
- **Mocking:** Moq 4.18.4
- **Assertions:** FluentAssertions 7.0.0

## **Getting Started**

### **Prerequisites**

Make sure you have the following installed:

- .NET 10 SDK
- SQL Server
- Visual Studio 2022 or Visual Studio Code
- Git

### **1. Clone the Repository**

```bash
git clone https://github.com/SalmaHani2/DonationHub.git
cd DonationHub
```

### **2. Configure User Secrets**

The project uses User Secrets to store sensitive configuration values.

Configure the required settings in the API project, including:

- Database connection string.
- JWT secret key and token settings.
- Seed admin credentials.

Example:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
```

Add the remaining settings using the configuration keys defined in the project.

### **3. Apply Database Migrations**

Run the following command from the solution directory, adjusting the project paths if necessary:

```bash
dotnet ef database update --project path/to/DonationHub.Infrastructure.csproj --startup-project path/to/DonationHub.Api.csproj
```

### **4. Build the Project**

```bash
dotnet build
```

### **5. Run the API**

```bash
dotnet run --project path/to/DonationHub.Api.csproj
```

### **6. Explore the API**

Once the application is running, access the Scalar API documentation at:

```text
/scalar/v1
```

## **API Endpoints**

All API endpoints use the following base route:

```text
/api/v1
```

### **Authentication**

| Method | Endpoint | Description |
|---|---|---|
| POST | `/auth/register` | Register a new user. |
| POST | `/auth/login` | Authenticate a user. |
| POST | `/auth/refresh` | Refresh access token. |
| POST | `/auth/logout` | Logout the current user. |
| GET | `/auth/me` | Get the current authenticated user. |
| GET | `/admin/staff` | Get staff members. |

### **Categories**

| Method | Endpoint | Description |
|---|---|---|
| GET | `/categories` | Get all categories. |
| GET | `/categories/{id}` | Get category details. |
| POST | `/categories` | Create a category. |
| PUT | `/categories/{id}` | Update a category. |
| DELETE | `/categories/{id}` | Delete a category. |

### **Campaigns**

| Method | Endpoint | Description |
|---|---|---|
| GET | `/campaigns` | Get campaigns. |
| GET | `/campaigns/success-stories` | Get successful campaigns. |
| GET | `/campaigns/{id}` | Get campaign details. |
| POST | `/campaigns` | Create a campaign. |
| PUT | `/campaigns/{id}` | Update a campaign. |
| DELETE | `/campaigns/{id}` | Delete a campaign. |

**Note:** Some endpoints require authentication and specific roles. Authorization rules are enforced by the API.

## **Development Practices**

The project follows several software engineering principles and practices:

- **Clean Architecture:** Separation of concerns and dependency management.
- **Vertical Slices:** Organizing application features around their functionality.
- **CQRS:** Separating command operations from query operations.
- **Repository Pattern & Unit of Work:** Managing persistence operations.
- **Rich Domain Model:** Keeping business rules inside domain entities.
- **Value Objects:** Representing domain concepts such as Money.
- **Result Pattern:** Handling expected failures without relying on exceptions.
- **ProblemDetails:** Providing standardized API error responses.
- **Dependency Injection:** Managing application dependencies.
- **TimeProvider:** Making time-dependent business logic testable.
- **Optimistic Concurrency:** Handling concurrent updates.
- **Soft Delete:** Supporting logical deletion where required.

## **Unit Testing**

The project includes a dedicated unit testing project:

```text
DonationHub.UnitTests
```

Tests follow the **Arrange – Act – Assert (AAA)** pattern.

### **Testing Libraries**

- xUnit
- Moq
- FluentAssertions

### **Run Tests**

From the solution directory, execute:

```bash
dotnet test
```

## **Design Decisions**

This section explains the reasoning behind some of the architectural and implementation decisions made in DonationHub.

### **1. Why Do Read Operations Bypass the Repository?**

DonationHub uses repositories and Unit of Work for write operations, while read operations query the `DbContext` directly.

This decision was made because:

- Read operations often require specific filtering, sorting, pagination, and DTO projections.
- Using `AsNoTracking()` improves read performance because EF Core does not track entities that will not be modified.
- Direct queries avoid unnecessary repository abstractions for queries that do not need entity modification.
- It allows read operations to retrieve exactly the required data without loading entire entities.

### **2. Why Use Result Pattern Instead of Exceptions?**

The Result Pattern is used to handle expected business and validation failures.

This decision was made because:

- Business failures are expected outcomes of application operations, not exceptional situations.
- `Result` and `Result<T>` explicitly represent success or failure.
- Centralized error codes make error handling consistent across the application.
- Errors can be mapped to appropriate HTTP status codes.
- Exceptions remain reserved for unexpected failures and exceptional situations.

### **3. Why Use Clean Architecture?**

Clean Architecture separates the application into independent layers with clear responsibilities.

This provides:

- Separation of concerns.
- Better maintainability and testability.
- Reduced coupling between business logic and infrastructure.
- The ability to change infrastructure implementations without changing domain rules.

### **4. Why Use TimeProvider?**

`TimeProvider` is used instead of directly depending on the system clock.

This makes time-dependent business rules easier to test because tests can provide a controlled time rather than relying on the actual current date and time.

### **5. Why Use Value Objects?**

Value Objects represent domain concepts such as `Money`.

They encapsulate validation and business rules, prevent invalid values from entering the domain, and compare values based on their contents rather than object identity.
