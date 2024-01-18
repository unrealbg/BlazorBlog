namespace BlazorBlog.Services
{
    public class SubscribeService : ISubscribeService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SubscribeService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<string?> AddSubscriberAsync(Subscriber subscriber)
        {
            using var context = _contextFactory.CreateDbContext();

            var existingSubscriber = await context.Subscribers
                .FirstOrDefaultAsync(s => s.Email == subscriber.Email);

            if (existingSubscriber != null)
            {
                return "You are already subscribed.";
            }

            subscriber.SubscribedOn = DateTime.UtcNow;

            await context.Subscribers.AddAsync(subscriber);
            await context.SaveChangesAsync();

            return null;
        }

        public async Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Subscribers
                .AsNoTracking()
                .OrderByDescending(s => s.SubscribedOn);

            var totalRecords = await query.CountAsync();

            var records = await query
                .Skip(startIndex)
                .Take(pageSize)
                .ToArrayAsync();

            return new PageResult<Subscriber>(records, totalRecords);
        }
    }
}
