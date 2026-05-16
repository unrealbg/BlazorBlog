# Blazor Blog Project

<!-- CI Status Badges -->

[![CI (main)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml?query=branch%3Amain)
[![CI (dev)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml/badge.svg?branch=dev)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml?query=branch%3Adev)

[![License](https://img.shields.io/github/license/unrealbg/BlazorBlog.svg)](LICENSE.txt)
[![Last commit](https://img.shields.io/github/last-commit/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/commits)
[![Open issues](https://img.shields.io/github/issues/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/issues)
[![Open PRs](https://img.shields.io/github/issues-pr/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/pulls)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)

## Overview

Welcome to the Blazor Blog Project! This repository hosts a modern, responsive blog application built with [Blazor Web App](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) on .NET 10. The goal is to deliver fast, interactive user interfaces with a clean architecture and a practical admin workflow.

## Features

- Blazor Web App (Server render mode)
- Responsive Design
- Interactive UI with QuickGrid for admin tables
- Identity (cookie auth) with seeded Admin user
- Rich text editor for posts with direct Quill integration
- Configurable EF Core migrations and data seeding on startup
- Serilog logging (console + rolling files)
- Health endpoint: GET /health
- In-memory caching for public lists (2 min TTL) with automatic cache bust on admin changes
- HTML sanitization for user content (Ganss.Xss)
- Server-side validation with FluentValidation
- PostgreSQL persistence through EF Core and Npgsql

## Screenshots

Below are some screenshots showcasing the different aspects and features of the Blazor Blog Project.

### Main View

![Main View of Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/1f060f1d-0d88-4188-90dd-1dc6c9e99c28 "Main View")
_The main landing page of the Blazor Blog, showing an overview of the blog's layout and design._

### Recent Posts

![Recent Posts on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/3c27efce-669f-43a9-a4d6-659fd62266d0 "Recent Posts")
_Continuation of the main view, displaying the latest blog posts._

### Dashboard

![Dashboard on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/82b167f4-f1f0-4b58-9efb-511bae2869a7 "Dashboard")
_The dashboard interface for managing the blog._

### Manage Categories

![Manage Categories on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/5d25c658-2622-48f1-8dea-dc20875da0a3 "Manage Categories")
_The section for managing blog categories._

### Manage Blog Posts

![Manage Blog Posts on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/5229d02b-1f18-4ae7-9258-a005418f11f3 "Manage Blog Posts")
_Interface for managing individual blog posts._

### Manage Subscribers

![Manage Subscribers on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/d0de2709-93fd-4dc0-9b6b-76f76d75e6dc "Manage Subscribers")
_The section dedicated to managing blog subscribers._

### Create a New Blog Post

![Create New Blog Post on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/5f95e8da-1ace-4bc3-b155-20dfdfa1c3d4 "Create New Blog Post")
_The interface for creating a new blog post._

### Create a New Category

![Create New Category on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/cb22bc8d-5564-4bcd-9d0f-08b5ee342f18 "Create New Category")
_The interface for adding a new category to the blog._

## Architecture

This solution follows Clean Architecture:

- Domain: Core entities and business rules with no dependencies.
- Application: Use cases, contracts, and validators; depends only on Domain.
- Infrastructure: EF Core persistence, ASP.NET Core Identity, and service implementations; depends on Application.
- Web (BlazorBlog): UI; depends on Application and Infrastructure.

Data and Identity live under `BlazorBlog.Infrastructure.Persistence` (single `ApplicationDbContext` and `ApplicationUser`). UI helpers use Application abstractions (e.g., `IToastService`) implemented in Infrastructure.

### Projects (solution structure)

- BlazorBlog (UI)
- BlazorBlog.Infrastructure (EF Core, Identity, seeding, data services)
- BlazorBlog.Application (view models, validators, contracts)
- BlazorBlog.Domain (entities)
- BlazorBlog.AppHost (Aspire local orchestration)
- BlazorBlog.Tests (xUnit v3 + bUnit)

### Tech stack

- .NET 10 / ASP.NET Core Blazor Web App
- EF Core 10 with Npgsql/PostgreSQL
- ASP.NET Core Identity with role-based authorization
- QuickGrid, FluentValidation, Mapster, Serilog
- Tailwind CSS
- Aspire AppHost for local orchestration
- xUnit v3, bUnit, Moq, coverlet

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop or another OCI-compatible container runtime for Aspire/Docker workflows
- Recommended: Visual Studio 2022 (latest) with ASP.NET workload
- PostgreSQL. The default local connection string expects `postgres/postgres` on `localhost:5432`
- Node.js 18+ (LTS) if you plan to run the Tailwind CSS watcher during development or rely on the publish-time CSS build

### Configuration

For local development, update the connection string and Admin user settings in `BlazorBlog/appsettings.json` or user secrets:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=blazorblog;Username=postgres;Password=postgres"
  },
  "AdminUser": {
    "Name": "Admin",
    "Email": "admin@bblog.com",
    "Password": "Admin@123",
    "Role": "Admin"
  }
}
```

Production must override `AdminUser:Password`; the app refuses to start with the default `Admin@123` password outside Development.

Optional runtime settings:

```json
{
  "Database": {
    "ApplyMigrationsOnStartup": false,
    "SeedOnStartup": true
  },
  "ForwardedHeaders": {
    "KnownProxies": [ "10.0.0.10" ]
  },
  "Email": {
    "Host": "smtp.example.com",
    "Port": 587,
    "EnableSsl": true,
    "UserName": "smtp-user",
    "Password": "smtp-password",
    "SenderEmail": "no-reply@example.com",
    "SenderName": "Blazor Blog",
    "RequireConfiguredSender": true
  }
}
```

To start a local PostgreSQL container:

```bash
docker run --name blazorblog-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=blazorblog -p 5432:5432 -d postgres:16
```

### Run with Aspire

The AppHost starts PostgreSQL, waits for it to become healthy, injects `ConnectionStrings__DefaultConnection`, and starts the Blazor app with a `/health` check.

```bash
dotnet run --project BlazorBlog.AppHost/BlazorBlog.AppHost.csproj
```

The Aspire dashboard opens at `http://localhost:15053`.

The AppHost pins PostgreSQL 16 and stores data in the `blazorblog-postgres16-data` Docker volume. If you previously ran the project with PostgreSQL 17, keep the old volume for backup or remove it after exporting any local data you still need.

### Run the application directly

From the repository root:

```bash
# Runs the UI project
dotnet run --project BlazorBlog/BlazorBlog.csproj
```

### Build

```bash
dotnet restore BlazorBlog.sln
dotnet build BlazorBlog.sln
```

### CSS development (Tailwind)

- One-time setup (inside `BlazorBlog/`):

```bash
npm ci
```

- Watch and rebuild CSS during development (run in a separate terminal from `BlazorBlog/`):

```bash
npm run dev:css
```

- Build CSS once (e.g., CI/local without watcher):

```bash
npm run build:css
```

Notes:

- On publish, CSS is built automatically by an MSBuild target that runs `npx tailwindcss` (requires Node.js installed on the machine).
- The generated stylesheet is `BlazorBlog/wwwroot/app.css`.

### First run behavior

- In Development, pending EF Core migrations are applied automatically on startup
- Outside Development, set `Database:ApplyMigrationsOnStartup=true` to opt in
- Initial data is seeded via `ISeedService` (Admin role/user + default categories) when `Database:SeedOnStartup` is true

> Optional: You can still apply migrations manually with `dotnet ef database update`, but it's not required for local runs.

## Authentication and Admin user

- Cookie authentication using ASP.NET Core Identity
- Login page: `/Account/Login`
- Default local Admin credentials (change before first production run):
  - Email: `admin@bblog.com`
  - Password: `Admin@123`

## Admin area

Admin-only pages (require the `Admin` role):

- `/admin/dashboard`
- `/admin/manage-subscribers`
- `/admin/manage-users`
- `/admin/create-user`

Content management pages require `Admin` or `Editor`:

- `/admin/manage-blog-posts` (+ create/edit pages)
- `/admin/manage-categories`

## Forgot/Reset password

- Pages:
  - `/Account/ForgotPassword`
  - `/Account/ResetPassword?email=...&code=...`
- Email sending uses `IEmailSender<ApplicationUser>`
- Configure the `Email` section for SMTP delivery; without it, the development fallback logs a warning and does not send email
- Development helper: In Development the Forgot Password page displays a 'Development only' section with the generated reset link and token for easy local testing

## Health endpoint

- `GET /health` returns `{ status, timeUtc }`

## Logging

- Serilog configured via `appsettings.json`
- Console + rolling file logs in `Logs/log-*.txt`

## Tests

- Run tests from the repo root:

```bash
dotnet test BlazorBlog.sln
```

## Docker

The Dockerfile builds the Blazor app with the .NET 10 SDK image and publishes a runtime image on ASP.NET Core 10:

```bash
docker build -t blazorblog .
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=blazorblog;Username=postgres;Password=postgres" \
  -e AdminUser__Password="replace-with-a-strong-password" \
  -e Database__ApplyMigrationsOnStartup=true \
  blazorblog
```

With Compose, create a `.env` file or export variables first:

```bash
POSTGRES_PASSWORD=replace-with-a-strong-db-password
ADMIN_USER_PASSWORD=replace-with-a-strong-admin-password
```

Then run:

```bash
docker compose up --build
```

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are greatly appreciated.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/NewFeature`)
3. Commit your Changes (`git commit -m "Add some NewFeature"`)
4. Push to the Branch (`git push origin feature/NewFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

### Contact

Zhelyazko Zhelyazkov - [admin@unrealbg.com](mailto:admin@unrealbg.com)
