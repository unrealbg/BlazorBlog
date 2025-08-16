namespace BlazorBlog.Infrastructure
{
    using System.Threading;

    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Infrastructure.Contracts;

    public class SubscribeService : ISubscribeService
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public SubscribeService(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task<string?> AddSubscriberAsync(Subscriber subscriber, CancellationToken cancellationToken = default)
        {
            return await _subscriberRepository.AddSubscriberAsync(subscriber, cancellationToken);
        }

        public async Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _subscriberRepository.GetSubscribersAsync(startIndex, pageSize, cancellationToken);
        }
    }
}
