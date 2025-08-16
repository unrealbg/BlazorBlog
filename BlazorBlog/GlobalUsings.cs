// Global using directives

global using BlazorBlog.Components;

// Infrastructure identity types needed by UI (e.g., ApplicationUser)
global using BlazorBlog.Infrastructure.Persistence;

// Decoupled services/utilities
global using BlazorBlog.Infrastructure.Common;
global using BlazorBlog.Application.UI;

// Service contracts from Infrastructure layer (Category, Subscribe, etc.)
global using BlazorBlog.Infrastructure.Contracts;

// Application layer contracts and models (Blog posts public API)
global using BlazorBlog.Application.Models;
global using BlazorBlog.Application.Contracts;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.QuickGrid;
global using Microsoft.AspNetCore.Identity;

global using Ganss.Xss;

global using Blazored.TextEditor;

global using System.ComponentModel.DataAnnotations;
global using System.Security.Claims;

global using Mapster;

global using FluentValidation;