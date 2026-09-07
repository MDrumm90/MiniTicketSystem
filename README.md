URL's:

**Frontend Azure URL:**
https://witty-glacier-09fef5e03.5.azurestaticapps.net/

**Backend Azure URL:**
https://mini-ticket-api-grb7apfcdbcjhyf2.polandcentral-01.azurewebsites.net/

## Frontend

The frontend is built with **Angular 20** using standalone components and a feature-based architecture.

### Technologies

- Angular 20
- TypeScript
- SCSS
- Angular Router
- Angular HttpClient
- RxJS
- NgRx Signal Store
- Docker
- Nginx

### Project structure

```text
MiniTicketSystem.FrontEnd/
├── src/
│   ├── app/
│   │   ├── layout/
│   │   │   ├── header/
│   │   │   ├── main-layout/
│   │   │   └── sidebar/
│   │   │
│   │   └── features/
│   │       └── tickets/
│   │           ├── models/
│   │           ├── data-access/
│   │           │   └── ticket.service.ts
│   │           ├── store/
│   │           │   ├── tickets.store.ts
│   │           │   └── ticket-form.store.ts
│   │           └── pages/
│   │               ├── ticket-list/
│   │               └── ticket-form/
│   │
│   ├── environments/
│   └── ...
├── Dockerfile
├── angular.json
├── package.json
└── tsconfig.json
```

### Architecture

The frontend follows a layered feature-based approach:

```text
┌─────────────────────────────┐
│          UI Pages           │
│ TicketList / TicketForm     │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│        Signal Stores        │
│ TicketsStore / FormStore    │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│       TicketService         │
│      HTTP communication     │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│      ASP.NET Core API       │
└─────────────────────────────┘
```

Components do not communicate with the API directly. They interact with the appropriate Signal Store, while the stores use `TicketService` for HTTP communication.

This separation keeps UI, state management and data access responsibilities independent.

### State management

Application state is managed using **NgRx Signal Store**.

The state is separated into two stores based on their responsibilities.

#### TicketsStore

`TicketsStore` is responsible only for the ticket list:

- loading tickets
- loading all tickets
- server-side pagination
- searching
- filtering by status
- changing page
- changing page size
- loading and error states

Search uses RxJS `debounceTime` and `distinctUntilChanged` to prevent unnecessary API requests while typing.

#### TicketFormStore

`TicketFormStore` is responsible only for ticket creation and editing:

- loading a ticket by ID
- creating tickets
- updating tickets
- tracking loading state
- tracking saving state
- handling errors

Separating the stores prevents list-related and form-related state from being mixed together.

### Routing

Angular Router is used with lazy-loaded standalone components.

Available routes:

```text
/tickets
/tickets/new
/tickets/:id/edit
```

The same `TicketForm` component is used for both creating and editing tickets.

When editing a ticket, the ID is obtained from the route and the existing ticket is loaded from the API before populating the form.

### API communication

All communication with the backend is encapsulated in `TicketService`.

The service provides methods for:

```text
GET    /Tickets
GET    /Tickets/paged
GET    /Tickets/{id}
POST   /Tickets
PUT    /Tickets/{id}
```

The API URL is kept outside the service implementation using Angular environment configuration, allowing the frontend to use different backend URLs in different environments.

### Ticket list

The ticket list provides:

- displaying all tickets
- server-side pagination
- text search
- filtering by status
- changing page
- changing page size
- editing existing tickets

Pagination, searching and filtering are performed by the backend rather than loading the complete dataset into the browser.

### Ticket forms

The application uses reactive forms for creating and editing tickets.

The create form contains:

- Title
- Description

The edit form additionally allows changing:

- Status

Required fields are validated on the client before sending the request to the backend.

### Docker

The frontend is packaged as a Docker image using a **multi-stage Docker build**.

The first stage uses Node.js to install dependencies and build the Angular application.

The final stage uses Nginx to serve the production build as static files.

```text
Node.js
   │
   ▼
Angular production build
   │
   ▼
Nginx
   │
   ▼
Angular application
```

Build the Docker image with:

```bash
docker build -t mini-ticket-frontend .
```

The container exposes port `80`.

### Key design decisions

- **Standalone Angular components** for a modern Angular architecture.
- **Feature-based project structure** to keep ticket-related functionality together.
- **Separate Signal Stores** for list and form state.
- **Dedicated API service** to isolate HTTP communication from UI components.
- **Server-side pagination, filtering and searching** to avoid unnecessary client-side processing.
- **Lazy-loaded routes** to load feature pages when they are needed.


## Backend

The backend is implemented using **ASP.NET Core Web API on .NET 10** and follows a layered architecture with clear separation of responsibilities.

### Technologies

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- EF Core InMemory Database
- Dependency Injection
- REST API
- Docker
- OpenAPI

### Project structure

```text
MiniTicketSystem/
├── MiniTicketSystem.Api/
│   ├── Controllers/
│   ├── Program.cs
│   └── Dockerfile
│
├── MiniTicketSystem.Application/
│   ├── Interfaces/
│   ├── Services/
│   └── DTOs/
│
├── MiniTicketSystem.Domain/
│   ├── Entities/
│   └── Enums/
│
├── MiniTicketSystem.Infrastructure/
│   ├── Data/
│   └── Repositories/
│
└── MiniTicketSystem.UnitTests/
```

### Architecture

The backend follows a layered architecture:

```text
┌─────────────────────────────┐
│             API             │
│         Controllers         │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│        Application          │
│     Services / DTOs         │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│      Infrastructure         │
│ Repositories / EF Core      │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│          Database           │
│       EF Core InMemory      │
└─────────────────────────────┘
```

The Domain layer contains the core business entities and enums and does not depend on infrastructure or API concerns.

### Domain

The Domain layer contains the core `Ticket` entity and `TicketStatus` enum.

```text
TicketStatus
├── Open
├── InProgress
└── Closed
```

The domain model is kept independent from infrastructure and presentation concerns.

### Application layer

The Application layer contains the application logic and abstractions used by the API.

The ticket functionality is separated into query and command responsibilities:

```text
ITicketQueryService
        │
        ├── GetAllAsync()
        ├── GetPagedAsync(...)
        └── GetByIdAsync(...)

ITicketCommandService
        │
        ├── CreateAsync(...)
        └── UpdateAsync(...)
```

This separation keeps read and write operations independent and makes the individual services easier to test.

### Repository pattern

Database access is encapsulated behind the `ITicketRepository` abstraction.

```text
Application
     │
     ▼
ITicketRepository
     │
     ▼
TicketRepository
     │
     ▼
TicketDbContext
     │
     ▼
EF Core InMemory
```

Application services depend on the repository abstraction rather than directly accessing Entity Framework Core.

This reduces coupling between business logic and data-access implementation.

### DTOs

The API uses dedicated DTOs instead of exposing domain entities directly.

Main DTOs include:

- `TicketDto` - representation returned by the API
- `TicketInsertDto` - data required to create a ticket
- `TicketUpdateDto` - data required to update a ticket
- `PagedResult<T>` - paginated API response

This prevents API contracts from being tightly coupled to the domain model.

### REST API

The API exposes the following endpoints:

```text
GET    /Tickets
GET    /Tickets/paged
GET    /Tickets/{id}
POST   /Tickets
PUT    /Tickets/{id}
```

#### Get all tickets

```http
GET /Tickets
```

Returns all tickets without pagination.

#### Get paginated tickets

```http
GET /Tickets/paged?page=1&pageSize=10
```

Supports:

- pagination
- status filtering
- text search

Example:

```http
GET /Tickets/paged?status=1&search=login&page=1&pageSize=10
```

Pagination and filtering are performed on the server side.

#### Get ticket by ID

```http
GET /Tickets/{id}
```

Returns a single ticket by its ID.

#### Create ticket

```http
POST /Tickets
```

Request body:

```json
{
  "title": "Example ticket",
  "description": "Ticket description"
}
```

#### Update ticket

```http
PUT /Tickets/{id}
```

Request body:

```json
{
  "id": "ticket-id",
  "title": "Updated ticket",
  "description": "Updated description",
  "status": 1
}
```

### Dependency Injection

The application uses ASP.NET Core built-in dependency injection.

Services and repositories are registered in `Program.cs`:

```text
ITicketRepository
        ↓
TicketRepository

ITicketQueryService
        ↓
TicketQueryService

ITicketCommandService
        ↓
TicketCommandService
```

This allows implementations to be replaced easily, which is particularly useful for unit testing.

### Database

For this recruitment project the application uses **EF Core InMemory**:

```csharp
options.UseInMemoryDatabase("InMem");
```

The database is seeded when the application starts.

The InMemory database was intentionally chosen because the application is a self-contained demonstration and does not require an external database.

Data is therefore not persistent between application restarts.

### CORS

CORS is configured through application configuration rather than hardcoded in the middleware.

Allowed frontend origins are defined in configuration:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  }
}
```

This allows the frontend origin to be changed without modifying the application code.

### Error handling

The API uses appropriate HTTP status codes for common scenarios, including:

```text
200 OK
201 Created
400 Bad Request
404 Not Found
```

Validation is applied to incoming DTOs before processing requests.

### Unit testing

The project contains a separate unit test project:

```text
MiniTicketSystem.UnitTests/
```

The main focus is testing application logic independently from the API and database infrastructure.

In particular, `TicketQueryService` can be tested using mocked repository dependencies.

This allows business logic to be verified without requiring a running database or HTTP server.

### Docker

The backend is packaged as a Docker image using a **multi-stage Docker build**.

The build process consists of:

```text
.NET SDK 10
     │
     ▼
Restore dependencies
     │
     ▼
Build
     │
     ▼
Publish
     │
     ▼
.NET ASP.NET 10 runtime
     │
     ▼
ASP.NET Core API
```

The final image contains only the published application and the ASP.NET Core runtime, rather than the complete .NET SDK.

Build the image from the solution root:

```bash
docker build -f MiniTicketSystem.Api/Dockerfile -t mini-ticket-api .
```

Run the container:

```bash
docker run --name mini-ticket-api -p 8080:8080 mini-ticket-api
```

The API is then available at:

```text
http://localhost:8080
```

### Key design decisions

- **Layered architecture** to separate API, application logic, domain and infrastructure.
- **Query/Command separation** to keep read and write responsibilities independent.
- **Repository pattern** to isolate database access.
- **DTOs** to keep API contracts independent from domain entities.
- **Dependency Injection** to reduce coupling and simplify testing.
- **Server-side pagination and filtering** to avoid unnecessary data processing on the client.
- **EF Core InMemory** for a self-contained recruitment demonstration.
- **Configuration-based CORS** to support different frontend environments.
- **Multi-stage Docker build** to create a smaller production runtime image.
- **Unit tests** focused on application logic rather than infrastructure.
- **Environment-based API configuration** to avoid hardcoded backend URLs.
- **Docker + Nginx** for a reproducible production-style frontend deployment.
