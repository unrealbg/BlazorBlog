namespace BlazorBlog.Services
{
    using Data.Models;
    using Repository.Contracts;

    public class SubscribeService : ISubscribeService
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public SubscribeService(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task<string?> AddSubscriberAsync(Subscriber subscriber)
        {
            return await _subscriberRepository.AddSubscriberAsync(subscriber);
        }

        public async Task<PageResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize)
        {
            return await _subscriberRepository.GetSubscribersAsync(startIndex, pageSize);
        }
    }
}
