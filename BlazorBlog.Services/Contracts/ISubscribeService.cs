using BlazorBlog.Data.Models;

namespace BlazorBlog.Services.Contracts;

public interface ISubscribeService
{
    Task<string?> AddSubscriberAsync(Subscriber subscriber);

    Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize);
}