namespace BlazorBlog.Repository.Contracts
{
    using Data.Entities;
    using Data.Models;

    public interface ISubscriberRepository
    {
        Task<string?> AddSubscriberAsync(Subscriber subscriber);
        Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize);
    }
}
