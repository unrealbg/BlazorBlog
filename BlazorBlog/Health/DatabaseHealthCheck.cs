namespace BlazorBlog.Health
{
    using BlazorBlog.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public sealed class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DatabaseHealthCheck(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var db = await _contextFactory.CreateDbContextAsync(cancellationToken);
                var canConnect = await db.Database.CanConnectAsync(cancellationToken);

                return canConnect
                    ? HealthCheckResult.Healthy("Database connection is available.")
                    : HealthCheckResult.Unhealthy("Database connection is not available.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database connection check failed.", ex);
            }
        }
    }
}
