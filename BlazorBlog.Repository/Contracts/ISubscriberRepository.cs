namespace BlazorBlog.Repository.Contracts
{
    using System.Threading;

    using Data.Entities;
    using Data.Models;

    public interface ISubscriberRepository
    {
        Task<string?> AddSubscriberAsync(Subscriber subscriber, CancellationToken cancellationToken = default);

        Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default);
    }
}
