# Contributing to BlazorBlog

Thanks for taking the time to contribute!

## Prerequisites
- .NET 9 SDK
- Node.js (for Tailwind build on publish)
- IDE: VS 2022 17.10+ or VS Code

## Getting started
1. Fork and clone the repo.
2. Create a feature branch off `dev`.
3. Restore/build:
   - dotnet restore
   - dotnet build
4. Run locally (adjust as needed):
   - dotnet run --project BlazorBlog/BlazorBlog.csproj

## Project layout
- BlazorBlog (Blazor Server/WASM host)
- BlazorBlog.Application (app services, DTOs)
- BlazorBlog.Domain (entities)
- BlazorBlog.Infrastructure (EF Core, data access)
- BlazorBlog.Tests (unit/bUnit tests)

## Code style
- Nullable enabled; avoid null! where possible.
- Prefer async/await, cancellation tokens in services.
- Use EF Core AsNoTracking for read-only queries, map with Mapster.
- Validate inputs with FluentValidation.
- Log with Serilog (structured logs).
- Keep components small; prefer partial class .razor.cs for logic.

Run formatting and tests before pushing:
- dotnet format --verify-no-changes
- dotnet test

## EF Core migrations
- Add: dotnet ef migrations add <Name> -s BlazorBlog -p BlazorBlog.Infrastructure
- Update DB: dotnet ef database update -s BlazorBlog -p BlazorBlog.Infrastructure
- Ensure new indexes (e.g., Slug, PublishedAt) are covered by migrations.

## Commit/PR guidelines
- Reference issues (Fixes #123).
- Keep PRs focused and small.
- Include screenshots for UI changes.
- Update docs/README when behavior changes.

## Security
If you discover a vulnerability, please do not open a public issue. Email the maintainer or use private channels.

## Discussions
Use GitHub Discussions for Q&A and ideas.
