namespace BlazorBlog.Health
{
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public sealed class DiskSpaceHealthCheck : IHealthCheck
    {
        private const long DefaultMinimumFreeBytes = 100L * 1024L * 1024L;

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DiskSpaceHealthCheck(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var contentRoot = Path.GetFullPath(_environment.ContentRootPath);
                var rootPath = Path.GetPathRoot(contentRoot);

                if (string.IsNullOrWhiteSpace(rootPath))
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Unable to resolve the content root drive."));
                }

                var drive = new DriveInfo(rootPath);
                if (!drive.IsReady)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("The content root drive is not ready."));
                }

                var minimumFreeBytes = _configuration.GetValue<long?>("HealthChecks:MinimumFreeDiskBytes")
                    ?? DefaultMinimumFreeBytes;

                var data = new Dictionary<string, object>
                {
                    ["path"] = contentRoot,
                    ["drive"] = drive.Name,
                    ["availableFreeBytes"] = drive.AvailableFreeSpace,
                    ["minimumFreeBytes"] = minimumFreeBytes
                };

                return Task.FromResult(
                    drive.AvailableFreeSpace >= minimumFreeBytes
                        ? HealthCheckResult.Healthy("Disk space is sufficient.", data)
                        : HealthCheckResult.Unhealthy("Available disk space is below the configured threshold.", data: data));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Disk space check failed.", ex));
            }
        }
    }
}
