namespace BlazorBlog.Infrastructure.Persistence.Repositories
{
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Application.Models;
    using BlazorBlog.Application.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;

    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SubscriberRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<string?> AddSubscriberAsync(Subscriber subscriber, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var existingSubscriber = await context.Subscribers
                .FirstOrDefaultAsync(s => s.Email == subscriber.Email, cancellationToken);

            if (existingSubscriber != null)
            {
                return "You are already subscribed.";
            }

            var entity = new BlazorBlog.Infrastructure.Persistence.Entities.Subscriber
            {
                Email = subscriber.Email,
                Name = subscriber.Name,
                SubscribedOn = DateTime.UtcNow
            };

            await context.Subscribers.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return null;
        }

        public async Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var totalRecords = await context.Subscribers.CountAsync(cancellationToken);

            var records = await context.Subscribers
                .AsNoTracking()
                .OrderByDescending(s => s.SubscribedOn)
                .Skip(startIndex)
                .Take(pageSize)
                .Select(s => new Subscriber
                {
                    Id = s.Id,
                    Email = s.Email,
                    Name = s.Name,
                    SubscribedOn = s.SubscribedOn
                })
                .ToArrayAsync(cancellationToken);

            return new PageResult<Subscriber>(records, totalRecords);
        }
    }

}
