var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("16")
    .WithDataVolume("blazorblog-postgres16-data");

var database = postgres.AddDatabase("DefaultConnection", "blazorblog");

builder.AddProject<Projects.BlazorBlog>("web")
    .WithReference(database)
    .WaitFor(database)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

builder.Build().Run();
