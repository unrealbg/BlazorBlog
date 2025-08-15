namespace BlazorBlog.Services.Contracts
{
    using System.Threading;

    using BlazorBlog.Data.Models;

    public interface ISubscribeService
    {
        Task<string?> AddSubscriberAsync(Subscriber subscriber, CancellationToken cancellationToken = default);

        Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default);
    }
}