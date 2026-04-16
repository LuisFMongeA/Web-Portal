# Web Portal — Project Overview

## About This Project

This application was developed as part of a technical evaluation exercise proposed by TecAlliance. The goal of the exercise was to implement a functional web portal allowing users to manage a personal ToDo list, with authentication based on email validation.

While the original specification provided a clear functional baseline, a deliberate decision was made to go beyond the minimum requirements and implement a solution that reflects real-world enterprise standards. The intent was not simply to fulfil the exercise, but to demonstrate architectural thinking, technical judgment, and awareness of production-grade development practices.

---

## What Was Built

The application consists of an **Angular 20+ frontend** and a **.NET backend** structured as independent microservices. The frontend communicates with three separate backend services responsible for authentication, user management, and todo management respectively.

Core features implemented:

- Email-based user registration and login
- JWT authentication with access and refresh tokens
- Session persistence across page refreshes via sessionStorage
- Personal ToDo list management with full CRUD operations
- Confirmation dialog before destructive operations
- Automatic token refresh on session expiry
- Route protection via authentication guard
- Centralized HTTP interceptor for token injection and 401 handling

---

## Beyond the Specification

Several features were introduced that were not explicitly required by the exercise brief, but were considered essential for a coherent and professional implementation:

**JWT Authentication with Refresh Tokens** — The specification required email-based validation. A full token-based authentication flow was implemented instead, including access token injection on every request and automatic refresh handling, as this reflects the standard expected in any production system.

**Microservice Architecture on the Backend** — Rather than a single monolithic API, the backend was structured as three independent microservices. This was a deliberate architectural choice to demonstrate separation of concerns at the infrastructure level.

**HTTP Interceptor** — Centralising token management and error handling in an interceptor rather than handling it per-service ensures consistency and eliminates repetition across the codebase.

**Centralised Error Messages** — All user-facing error strings are defined in a single constants file, following internationalisation best practices and making future localisation straightforward.

**Unit Testing** — A comprehensive suite of unit tests was implemented covering services, guards, interceptors, and components, using Vitest as the test runner.

**Environment-based Configuration** — API base URLs are fully externalised through Angular's environment system, ensuring that switching between development and production targets requires no code changes.

---

## Design Considerations

The application is intentionally **functionality-first**. Visual design and styling were kept minimal and functional — sufficient to validate the user interface and demonstrate correct behaviour, but not the focus of this submission. In a production context, a dedicated UI/UX phase would follow once the architectural and functional foundations are established.

---

## Areas for Further Development

The current implementation represents a solid and extensible foundation. The following areas have been identified as natural next steps should the project be continued:

- Full UI/UX design pass with a consistent design system
- End-to-end testing with Playwright or Cypress
- Token refresh queue to handle concurrent 401 responses without duplicate refresh calls
- Pagination or virtual scrolling for large todo lists
- Todo filtering and sorting capabilities
- Persistent storage on the backend replacing the current in-memory cache
- CI/CD pipeline configuration
- Accessibility audit and WCAG compliance review
- Internationalisation support

---

# Web Portal — Frontend

Angular 20+ frontend for the Web Portal personal todo management application.

---

## Tech Stack

- **Angular 20+** — Standalone components, Signals, Reactive Forms
- **TypeScript** — Strict mode
- **SCSS** — Component-scoped styles
- **Vitest** — Unit testing
- **Node.js 20+**

---

## Prerequisites

Make sure you have the following installed before running the application:

- [Node.js 20+](https://nodejs.org/)
- [Angular CLI 20+](https://angular.dev/tools/cli)

```bash
npm install -g @angular/cli@latest
```

---

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd front-portal
```

### 2. Install dependencies

```bash
npm install
```

### 3. Configure environment

The environment file is located at `src/environments/environment.development.ts`. Update the API URLs to match your local backend services:

```typescript
export const environment = {
  authApiUrl: 'https://localhost:<AUTH_PORT>/api',
  todoApiUrl: 'https://localhost:<TODO_PORT>/api',
  userApiUrl: 'https://localhost:<USER_PORT>/api'
};
```

Replace `<AUTH_PORT>`, `<TODO_PORT>` and `<USER_PORT>` with the ports your .NET microservices are running on.

### 4. Run the application

```bash
ng serve
```

The application will be available at `http://localhost:4200`.

> **Note:** The backend services must be running before starting the frontend. Refer to the backend README for instructions on how to start them.

---

## Project Structure

```
src/app/
├── core/                        # Singleton services, guards, interceptors
│   ├── constants/               # Centralized error messages
│   ├── guards/                  # Auth guard
│   ├── interceptors/            # HTTP interceptor (Bearer token + 401 handling)
│   └── services/                # AuthService
├── shared/                      # Reusable components
│   └── components/
│       └── header/              # Navigation header
├── features/
│   ├── auth/                    # Authentication feature
│   │   ├── models/              # AuthResponse, User interfaces
│   │   └── pages/login-page/    # Login & Sign Up page
│   ├── todo/                    # Todo management feature
│   │   ├── models/              # Todo interface
│   │   ├── services/            # TodoService
│   │   ├── pages/todo-page/     # Smart page component
│   │   └── components/          # TodoList, TodoItem (dumb components)
│   └── about/                   # Static about page
│       └── pages/about-page/
└── environments/                # Environment configuration
```

---

## Architecture Decisions

### Standalone Components
The application uses Angular 20+ standalone components throughout — no NgModules.

### Signal-based State Management
All application state is managed with Angular Signals. RxJS Observables from `HttpClient` are kept isolated within services and never exposed to components directly.

### Smart / Dumb Component Pattern
- **Pages** (`pages/`) — routable, smart components that inject services and own state coordination
- **Components** (`components/`) — presentational, dumb components that communicate exclusively via `input()` / `output()`

### HTTP Interceptor
All outgoing requests automatically include the `Authorization: Bearer <token>` header. On a `401` response, the interceptor attempts a token refresh. If the refresh fails, the user is logged out and redirected to the login page.

### Session Persistence
Authentication tokens and user email are stored in `sessionStorage` — session persists across page refreshes but is cleared when the browser tab is closed.

---

## Available Routes

| Route | Access | Description |
|-------|--------|-------------|
| `/login` | Public | Sign in / Sign up |
| `/home` | Protected | Todo list management |
| `/about` | Protected | About the application |

All protected routes are guarded by `authGuard`. Unauthenticated users are redirected to `/login`.

---

## Running Tests

```bash
npm test
```

Tests are written with **Vitest** and **Angular Testing Library**. Coverage includes:

- `AuthService` — authentication state, session management
- `authGuard` — route protection logic
- `authInterceptor` — token injection and 401 handling
- `LoginPage` — form validation, sign in / sign up flows
- `TodoPage` — todo CRUD operations
- `TodoItem` — edit mode, delete confirmation

---

## Building for Production

```bash
ng build --configuration=production
```

Output will be generated in the `dist/` directory. Update `src/environments/environment.ts` with your production API URLs before building.


# TodoPortal — Backend

## Overview

The backend is built with **.NET 8** and follows a **microservices architecture**. Each service is independently deployable and owns its own domain, data, and business logic.

| Service | Responsibility | Port |
|---|---|---|
| **AuthService** | Issues and validates JWT tokens signed with RSA | 7003 |
| **UserService** | User registration and login | 7001 |
| **TodoService** | Todo CRUD operations | 7002 |

---

## Architecture Decisions

### Microservices
Each service is fully independent with no shared libraries or models. This avoids coupling and ensures each service can evolve independently.

### Authentication Flow
1. The user registers via `POST /api/user/create`
2. The user logs in via `POST /api/user/login` — UserService verifies the user exists and delegates token generation to AuthService
3. The frontend receives an `accessToken` (15 min) and a `refreshToken` (1 hour)
4. Every request to TodoService includes `Authorization: Bearer {accessToken}`
5. TodoService middleware validates the token against AuthService on every request

### JWT with RSA
AuthService generates an RSA key pair on startup and signs all tokens with the private key. Other services obtain the public key via `GET /api/auth/public-key` at startup to verify tokens locally without calling AuthService on every request.

### In-Memory Persistence
All services use in-memory storage (`Dictionary`) for simplicity. The repository pattern with interfaces ensures this can be swapped for a real database (e.g. EF Core + SQL Server) by only changing the infrastructure layer.

### Token Blacklist
Invalidated tokens are stored in `IMemoryCache` with an expiration equal to the token's remaining lifetime. This avoids manual cleanup — expired tokens are automatically evicted.

### Refresh Token Rotation
Every call to `/api/auth/refresh` invalidates the used refresh token and issues a new one, preventing reuse in case of token theft.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Any IDE (Visual Studio, Visual Studio Code, Rider)

---

## Project Structure

```
TodoPortal.sln
│
├── src/
│   ├── TodoPortal.AuthService/       # JWT issuance and validation
│   ├── TodoPortal.UserService/       # User registration and login
│   └── TodoPortal.TodoService/       # Todo management
│
└── tests/
    ├── TodoPortal.UnitTests/         # xUnit + Moq unit tests
    └── TodoPortal.IntegrationTests/  # WebApplicationFactory integration tests
```

Each service follows the same internal structure:

```
Service/
├── Controllers/      # HTTP layer
├── Models/
│   ├── Entities/     # Domain models
│   ├── Interfaces/   # Service and repository abstractions
│   └── DTOs/         # Request and response contracts
├── Services/         # Business logic
└── Infrastructure/
    ├── Repositories/ # In-memory persistence
    └── Clients/      # HTTP clients for inter-service communication
```

---

## Running the Services

The services must be started in the following order, as UserService and TodoService depend on AuthService being available at startup.

### 1. AuthService
```bash
cd src/TodoPortal.AuthService
dotnet run
# Runs on https://localhost:7003
```

### 2. UserService
```bash
cd src/TodoPortal.UserService
dotnet run
# Runs on https://localhost:7001
```

### 3. TodoService
```bash
cd src/TodoPortal.TodoService
dotnet run
# Runs on https://localhost:7002
```

Or run all three from the solution root using Visual Studio's multiple startup projects configuration.

---

## API Reference

### AuthService — `https://localhost:7003`

| Method | Endpoint | Description | Body |
|---|---|---|---|
| POST | `/api/auth/token` | Generate access and refresh tokens | `{ "email": "..." }` |
| POST | `/api/auth/refresh` | Refresh an expired access token | `{ "refreshToken": "..." }` |
| POST | `/api/auth/invalidate` | Invalidate an access token | `{ "accessToken": "..." }` |
| POST | `/api/auth/validate` | Check if a token is valid | `{ "accessToken": "..." }` |
| GET | `/api/auth/public-key` | Get the RSA public key in PEM format | — |

### UserService — `https://localhost:7001`

| Method | Endpoint | Description | Body |
|---|---|---|---|
| POST | `/api/user/create` | Register a new user | `{ "email": "..." }` |
| POST | `/api/user/login` | Login and receive tokens | `{ "email": "..." }` |

### TodoService — `https://localhost:7002`

All endpoints require `Authorization: Bearer {accessToken}` header.

| Method | Endpoint | Description | Body / Params |
|---|---|---|---|
| GET | `/api/todo` | Get all todos for the authenticated user | — |
| GET | `/api/todo/{id}` | Get a specific todo | — |
| POST | `/api/todo` | Create a new todo | `{ "userId": "...", "description": "..." }` |
| PUT | `/api/todo/{id}` | Update a todo | `{ "id": "...", "userId": "...", "description": "..." }` |
| DELETE | `/api/todo/{id}` | Delete a todo | — |
| PATCH | `/api/todo/{id}/done` | Toggle done status | `?newValue=true` |

---

## Running the Tests

### Unit Tests
```bash
cd tests/TodoPortal.UnitTests
dotnet test
```

### Integration Tests
```bash
cd tests/TodoPortal.IntegrationTests
dotnet test
```

Integration tests use `WebApplicationFactory` to spin up each service in memory — no running instances are required.

---

## Configuration

Each service has an `appsettings.json` with the following relevant keys:

**UserService & TodoService:**
```json
{
  "Auth": {
    "BaseAddress": "https://localhost:7003",
    "GetToken": "/api/auth/token",
    "ValidateToken": "/api/auth/validate"
  }
}
```
