namespace BlazorBlog.Tests.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Infrastructure;
    using Moq;
    using Xunit;

    public class TagAndSubscribeServiceTests
    {
        [Fact]
        public async Task TagService_Forwards_All_Methods_To_Repository()
        {
            var repo = new Mock<ITagRepository>(MockBehavior.Strict);
            var svc = new TagService(repo.Object);

            repo.Setup(r => r.GetTopTagsAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<TagVm>());
            repo.Setup(r => r.GetTagsForPostAsync(12, It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<string>());
            repo.Setup(r => r.SetTagsForPostAsync(12, It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            Assert.Empty(await svc.GetTopTagsAsync(5));
            Assert.Empty(await svc.GetTagsForPostAsync(12));
            await svc.SetTagsForPostAsync(12, new[] { "x" });

            repo.VerifyAll();
        }

        [Fact]
        public async Task SubscribeService_Forwards_All_Methods_To_Repository()
        {
            var repo = new Mock<ISubscriberRepository>(MockBehavior.Strict);
            var svc = new SubscribeService(repo.Object);

            var page = new PageResult<Subscriber>(System.Array.Empty<Subscriber>(), 0);
            repo.Setup(r => r.AddSubscriberAsync(It.IsAny<Subscriber>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string?)null);
            repo.Setup(r => r.GetSubscribersAsync(0, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(page);

            var err = await svc.AddSubscriberAsync(new Subscriber { Email = "e@x.com", Name = "n" });
            Assert.Null(err);
            var subs = await svc.GetSubscribersAsync(0, 10);
            Assert.Equal(0, subs.TotalCount);

            repo.VerifyAll();
        }
    }
}
