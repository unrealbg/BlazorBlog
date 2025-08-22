# Blazor Blog Project

<!-- CI Status Badges -->

[![CI (main)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml?query=branch%3Amain)
[![CI (dev)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml/badge.svg?branch=dev)](https://github.com/unrealbg/BlazorBlog/actions/workflows/ci.yml?query=branch%3Adev)

[![License](https://img.shields.io/github/license/unrealbg/BlazorBlog.svg)](LICENSE.txt)
[![Last commit](https://img.shields.io/github/last-commit/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/commits)
[![Open issues](https://img.shields.io/github/issues/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/issues)
[![Open PRs](https://img.shields.io/github/issues-pr/unrealbg/BlazorBlog.svg)](https://github.com/unrealbg/BlazorBlog/pulls)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)

Live demo: https://blog.unrealbg.com/

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

Updated screenshots showcasing the current UI.

### 1) Home – Hero
![Home – Hero](https://www.unrealbg.com/blazorblog/01-home-hero.png "Home – Hero")
_The landing page hero with tag badges and primary CTAs._

### 2) Home – Latest Posts
![Home – Latest Posts](https://www.unrealbg.com/blazorblog/02-home-latest-posts.png "Home – Latest Posts")
_The Latest Posts section on the homepage and the footer navigation._

### 3) All Posts
![All Posts](https://www.unrealbg.com/blazorblog/03-all-posts.png "All Posts")
_A full list of articles with sidebar: Subscribe, Popular Posts, and Categories._

### 4) Post Details
![Post Details](https://www.unrealbg.com/blazorblog/04-post-details.png "Post Details")
_Single post view with title, metadata, author, and cover image._

### 5) Post – Footer & Subscribe
![Post – Footer & Subscribe](https://www.unrealbg.com/blazorblog/05-post-footer-subscribe.png "Post – Footer & Subscribe")
_Post footer with share actions and newsletter subscribe form._

### 6) Category – Featured Card
![Category – Featured Card](https://www.unrealbg.com/blazorblog/06-category-featured-large.png "Category – Featured Card")
_Category page highlighting a featured article with a large card preview._

### 7) Tag Page – Latest & Subscribe
![Tag Page – Latest & Subscribe](https://www.unrealbg.com/blazorblog/07-tag-page-latest-subscribe.png "Tag Page – Latest & Subscribe")
_Tag page (e.g., #javascript) with latest posts and a subscribe form._

### 8) Login
![Login](https://www.unrealbg.com/blazorblog/08-login.png "Login")
_Login screen with email, password, “Remember me,” and “Forgot password?” link._

### 9) Admin – Dashboard
![Admin – Dashboard](https://www.unrealbg.com/blazorblog/09-admin-dashboard.png "Admin – Dashboard")
_Admin console overview with quick actions for categories, posts, and subscribers._

### 10) Admin – Manage Categories
![Admin – Manage Categories](https://www.unrealbg.com/blazorblog/10-admin-manage-categories.png "Admin – Manage Categories")
_Category management table with name, slug, navbar visibility, and Edit/Delete actions._

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
