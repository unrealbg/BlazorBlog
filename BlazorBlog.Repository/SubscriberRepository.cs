namespace BlazorBlog.Repository
{
    using Data.Entities;
    using Data.Models;
    using Data;
    using Contracts;
    using Microsoft.EntityFrameworkCore;

    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SubscriberRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<string?> AddSubscriberAsync(Subscriber subscriber)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
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
            await using var context = await _contextFactory.CreateDbContextAsync();
            var totalRecords = await context.Subscribers.CountAsync();

            var records = await context.Subscribers
                .AsNoTracking()
                .OrderByDescending(s => s.SubscribedOn)
                .Skip(startIndex)
                .Take(pageSize)
                .ToArrayAsync();

            return new PageResult<Subscriber>(records, totalRecords);
        }
    }

}
