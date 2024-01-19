# Blazor Blog Project

## Overview

Welcome to the Blazor Blog Project! This repository hosts a modern, responsive blog application, built using the latest [Blazor Web App](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) template. Our goal is to leverage Blazor's capabilities to deliver fast and interactive user interfaces, ensuring an engaging and seamless experience for our users.

## Features

- **Blazor Web App**: Utilizes the latest Blazor framework for building interactive web UIs using C# instead of JavaScript.
- **Responsive Design**: Ensures a smooth experience across various devices and screen sizes.
- **Interactive UI**: Offers dynamic user interactions with minimal latency.

## Screenshots

Below are some screenshots showcasing the different aspects and features of the Blazor Blog Project.

### Main View
![Main View of Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/1f060f1d-0d88-4188-90dd-1dc6c9e99c28 "Main View")
*The main landing page of the Blazor Blog, showing an overview of the blog's layout and design.*

### Recent Posts
![Recent Posts on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/3c27efce-669f-43a9-a4d6-659fd62266d0 "Recent Posts")
*Continuation of the main view, displaying the latest blog posts.*

### Dashboard
![Dashboard on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/82b167f4-f1f0-4b58-9efb-511bae2869a7 "Dashboard")
*The dashboard interface for managing the blog.*

### Manage Categories
![Manage Categories on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/5d25c658-2622-48f1-8dea-dc20875da0a3 "Manage Categories")
*The section for managing blog categories.*

### Manage Blog Posts
![Manage Blog Posts on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/5229d02b-1f18-4ae7-9258-a005418f11f3 "Manage Blog Posts")
*Interface for managing individual blog posts.*

### Manage Subscribers
![Manage Subscribers on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/d0de2709-93fd-4dc0-9b6b-76f76d75e6dc "Manage Subscribers")
*The section dedicated to managing blog subscribers.*

### Create a New Blog Post
![Create New Blog Post on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/5f95e8da-1ace-4bc3-b155-20dfdfa1c3d4 "Create New Blog Post")
*The interface for creating a new blog post.*

### Create a New Category
![Create New Category on Blazor Blog](https://github.com/unrealbg/BlazorBlog/assets/3398536/cb22bc8d-5564-4bcd-9d0f-08b5ee342f18 "Create New Category")
*The interface for adding a new category to the blog.*

## Getting Started

To get started with the Blazor Blog project, follow these steps:

### Prerequisites

- Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.
- Recommended: Visual Studio 2022 or later with the ASP.NET and web development workload.
- A SQL Server instance for the database (or adjust the database provider as per your choice).

### Clone the Repository

```bash
git clone https://github.com/unrealbg/BlazorBlog.git
```

## Configure Admin User
Before running the application, you need to configure the AdminUser settings in the appsettings.json file:
```bash
"AdminUser": {
  "Name": "Admin",
  "Email": "admin@bblog.com",
  "Password": "Admin@123",
  "Role": "Admin"
}
```
This step is crucial to ensure the application initializes correctly with an admin user account.

## Database Migrations

Before running the application, ensure that the database is up-to-date with the latest migrations:

1. **Navigate to the Project Directory**
   - Go to the folder where the `*.csproj` file is located.

2. **Update the Database**
   - Run the following command to apply the latest migrations:

     ```bash
     dotnet ef database update
     ```

### Run the Application
- Navigate to the project directory.
- Run the following command:
```bash
dotnet run
```

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".

Don't forget to give the project a star! Thanks again!

1. **Fork the Project**
2. **Create your Feature Branch** (`git checkout -b feature/NewFeature`)
3. **Commit your Changes** (`git commit -m 'Add some NewFeature'`)
4. **Push to the Branch** (`git push origin feature/NewFeature`)
5. **Open a Pull Request**

## License

Distributed under the MIT License. See `LICENSE` for more information.

### Contact
Zhelyazko Zhelyazkov -  [admin@unrealbg.com](mailto:admin@unrealbg.com)

Project Link:  [https://github.com/unrealbg/BlazorBlog](https://github.com/unrealbg/BlazorBlog)
