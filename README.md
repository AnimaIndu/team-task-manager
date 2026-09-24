# Team Task Management System

A full-stack Team Task Management System built with **ASP.NET Core, React, SQL Server, and Docker**.

## Features

* JWT-based authentication
* Role-based access control

  * Admin
  * Manager
  * User
* User registration and login
* Team management
* Task creation and assignment
* Task status management

  * ToDo
  * InProgress
  * Done
* Task priority management

  * Low
  * Medium
  * High
* Task deadlines
* Task comments
* Notifications
* Dashboard with task statistics
* Swagger API documentation
* Unit/integration tests
* Docker support
* GitHub Actions CI

## Tech Stack

### Backend

* ASP.NET Core 10
* Entity Framework Core
* SQL Server
* JWT Authentication
* BCrypt password hashing
* Swagger / OpenAPI

### Frontend

* React
* Vite
* Axios
* React Router

### DevOps

* Docker
* Docker Compose
* GitHub Actions

## Project Structure

```text
team-task-manager/
├── backend/
│   ├── TaskManagement.Api/
│   └── TaskManagement.Tests/
├── frontend/
├── .github/
│   └── workflows/
│       └── ci.yml
├── docker-compose.yml
├── .gitignore
└── README.md
```

## Running Locally

### Prerequisites

* .NET 10 SDK
* Node.js 20+
* SQL Server
* Docker Desktop (optional)

### Backend

```bash
cd backend/TaskManagement.Api
dotnet run --urls "http://localhost:5001"
```

Backend:

```text
http://localhost:5001
```

Swagger:

```text
http://localhost:5001/swagger
```

### Frontend

Open another terminal:

```bash
cd frontend
npm install
npm run dev
```

Frontend:

```text
http://localhost:5173
```

## Database

The application uses SQL Server with Entity Framework Core.

The default connection is configured through:

```text
ConnectionStrings__DefaultConnection
```

For Docker, the backend connects to the SQL Server container using the Docker network.

## Docker

To run the application using Docker Compose:

```bash
docker compose up --build
```

The services are exposed at:

```text
Frontend: http://localhost:3000
Backend:  http://localhost:5001
```

## Seed Users

The application includes seeded users for testing.

| Role    | Email                                         | Password    |
| ------- | --------------------------------------------- | ----------- |
| Admin   | [admin@tms.local](mailto:admin@tms.local)     | password123 |
| Manager | [manager@tms.local](mailto:manager@tms.local) | password123 |
| User    | [user1@tms.local](mailto:user1@tms.local)     | password123 |
| User    | [user2@tms.local](mailto:user2@tms.local)     | password123 |

> These credentials are intended for local development/testing only.

## API Documentation

Swagger UI is available at:

```text
http://localhost:5001/swagger
```

The API includes endpoints for:

* Authentication
* Users
* Teams
* Tasks
* Comments
* Notifications
* Dashboard

## Testing

Run backend tests with:

```bash
dotnet test backend/TaskManagement.Tests/TaskManagement.Tests.csproj
```

## CI/CD

GitHub Actions runs automatically on:

* Pushes to `main`
* Pull requests targeting `main`

The CI workflow:

1. Restores backend dependencies
2. Builds the backend
3. Runs backend tests
4. Installs frontend dependencies
5. Builds the frontend

## Environment Variables

Frontend API configuration is stored in `.env`:

```text
VITE_API_URL=http://localhost:5001/api
```

The `.env` file is excluded from Git through `.gitignore`.

## Authentication

The application uses JWT bearer authentication.

After successful login, the frontend stores the JWT token and automatically attaches it to authenticated API requests.

## Notifications

Task-related actions can generate in-app notifications.

The backend also includes a mock email notification mechanism for development/testing.

## Author

**Anima Indu**

GitHub:

https://github.com/AnimaIndu/team-task-manager
