namespace BlazorBlog.Application.Contracts
{
    using System.Threading;

    using BlazorBlog.Application.Models;
    using BlazorBlog.Domain.Entities;

    public interface ISubscriberRepository
    {
        Task<string?> AddSubscriberAsync(Subscriber subscriber, CancellationToken cancellationToken = default);

        Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default);
    }
}
