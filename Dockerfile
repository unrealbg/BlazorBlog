# syntax=docker/dockerfile:1.6
ARG DOTNET_VERSION=10.0

# ---- Runtime (Linux) ----
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs

# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine AS build
WORKDIR /src

# Install Node.js and npm only in the build image for Tailwind CSS.
RUN apk add --no-cache nodejs npm

# Copy project files first to keep restore layers cacheable.
COPY BlazorBlog/BlazorBlog.csproj BlazorBlog/
COPY BlazorBlog.Application/BlazorBlog.Application.csproj BlazorBlog.Application/
COPY BlazorBlog.Domain/BlazorBlog.Domain.csproj BlazorBlog.Domain/
COPY BlazorBlog.Infrastructure/BlazorBlog.Infrastructure.csproj BlazorBlog.Infrastructure/
COPY BlazorBlog.Tests/BlazorBlog.Tests.csproj BlazorBlog.Tests/
COPY BlazorBlog.AppHost/BlazorBlog.AppHost.csproj BlazorBlog.AppHost/
COPY BlazorBlog.sln ./

# Restore for the Linux runtime target.
RUN dotnet restore BlazorBlog.sln -r linux-x64

# Copy the full source tree.
COPY . .

# Publish the web project. The project file builds Tailwind CSS during publish.
RUN dotnet publish BlazorBlog/BlazorBlog.csproj -c Release -o /out -r linux-x64 --self-contained false /p:PublishTrimmed=false

# ---- Final ----
FROM base AS final
WORKDIR /app
COPY --from=build /out ./
ENTRYPOINT ["dotnet", "BlazorBlog.dll"]
