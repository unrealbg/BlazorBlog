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
                await db.Database.OpenConnectionAsync(cancellationToken);
                await db.Database.CloseConnectionAsync();

                return HealthCheckResult.Healthy("Database connection is available.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database connection check failed.", ex);
            }
        }
    }
}
