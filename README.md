# Blazor Blog Project

<!-- CI Status Badges -->

[![CI (main)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml?query=branch%3Amain)
[![CI (dev)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml/badge.svg?branch=dev)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml?query=branch%3Adev)

[![License](https://img.shields.io/github/license/unrealbg/BlazorBlog.svg)](LICENSE.txt)
[![Last commit](https://img.shields.io/github/last-commit/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/commits)
[![Open issues](https://img.shields.io/github/issues/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/issues)
[![Open PRs](https://img.shields.io/github/issues-pr/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/pulls)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Release](https://img.shields.io/github/v/release/unrealbg/BlazorBlog?include_prereleases&label=release)](https://github.com/unrealbg/BlazorBlog/releases)

[![Coverage (main)](https://codecov.io/gh/unrealbg/BlazorBlog/branch/main/graph/badge.svg)](https://app.codecov.io/gh/unrealbg/BlazorBlog/branch/main)
[![Coverage (dev)](https://codecov.io/gh/unrealbg/BlazorBlog/branch/dev/graph/badge.svg)](https://app.codecov.io/gh/unrealbg/BlazorBlog/branch/dev)

## Overview

Welcome to the Blazor Blog Project! This repository hosts a modern, responsive blog application, built using the latest [Blazor Web App](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) template. Our goal is to leverage Blazor's capabilities to deliver fast and interactive user interfaces, ensuring an engaging and seamless experience for our users.

## Features

- Blazor Web App (Server render mode)
- Responsive Design
- Interactive UI with QuickGrid for admin tables
- Identity (cookie auth) with seeded Admin user
- Rich text editor for posts (Blazored.TextEditor / Quill)
- Auto-apply EF Core migrations and data seeding on startup
- Serilog logging (console + rolling files)
- Health endpoint: GET /health
- In-memory caching for public lists (2 min TTL) with automatic cache bust on admin changes
- HTML sanitization for user content (Ganss.Xss)
- Server-side validation with FluentValidation

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
- Infrastructure: EF Core persistence, Identity, and service implementations; depends on Application.
- Web (BlazorBlog): UI; depends on Application and Infrastructure.

Data and Identity live under `BlazorBlog.Infrastructure.Persistence` (single `ApplicationDbContext` and `ApplicationUser`). UI helpers use Application abstractions (e.g., `IToastService`) implemented in Infrastructure.

### Projects (solution structure)

- BlazorBlog (UI)
- BlazorBlog.Infrastructure (EF Core, Identity, seeding, data services)
- BlazorBlog.Application (view models, validators, contracts)
- BlazorBlog.Domain (entities)
- BlazorBlog.Tests (xUnit + bUnit)

## Getting Started

### Prerequisites

- .NET 9 SDK
- Recommended: Visual Studio 2022 (latest) with ASP.NET workload
- SQL Server (default is `(localdb)\\MSSQLLocalDB`). Change the connection string if needed
- Node.js 18+ (LTS) if you plan to run the Tailwind CSS watcher during development or rely on the publish-time CSS build

### Configuration

Update the connection string and Admin user settings in `BlazorBlog/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BlazorBlog;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;"
  },
  "AdminUser": {
    "Name": "Admin",
    "Email": "admin@bblog.com",
    "Password": "Admin@123",
    "Role": "Admin"
  }
}
```

### Run the application

From the repository root:

```bash
# Runs the UI project
dotnet run --project BlazorBlog/BlazorBlog.csproj
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

- Pending EF Core migrations are applied automatically on startup
- Initial data is seeded via `ISeedService` (Admin role/user + default categories)

> Optional: You can still apply migrations manually with `dotnet ef database update`, but it's not required for local runs.

## Authentication and Admin user

- Cookie authentication using ASP.NET Core Identity
- Login page: `/Account/Login`
- Default Admin credentials (change in appsettings before first run):
  - Email: `admin@bblog.com`
  - Password: `Admin@123`

## Admin area

Admin-only pages (require the `Admin` role):

- `/admin/dashboard`
- `/admin/manage-blog-posts` (+ create/edit pages)
- `/admin/manage-categories`
- `/admin/manage-subscribers`

## Forgot/Reset password

- Pages:
  - `/Account/ForgotPassword`
  - `/Account/ResetPassword?email=...&code=...`
- Email sending uses `IEmailSender<ApplicationUser>`. The default is a no-op sender for local development
- Development helper: In Development the Forgot Password page displays a 'Development only' section with the generated reset link and token for easy local testing

## Health endpoint

- `GET /health` returns `{ status, timeUtc }`

## Logging

- Serilog configured via `appsettings.json`
- Console + rolling file logs in `Logs/log-*.txt`

## Tests

- Run tests from the repo root:

```bash
 dotnet test
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
