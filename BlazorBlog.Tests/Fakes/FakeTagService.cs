namespace BlazorBlog.Tests.Fakes
{
    using System.Threading;
    using System.Threading.Tasks;

    using BlazorBlog.Application.Models;
    using BlazorBlog.Infrastructure.Contracts;

    public class FakeTagService : ITagService
    {
        public Task<TagVm[]> GetTopTagsAsync(int count, CancellationToken cancellationToken = default)
            => Task.FromResult(System.Array.Empty<TagVm>());

        public Task<string[]> GetTagsForPostAsync(int postId, CancellationToken cancellationToken = default)
            => Task.FromResult(System.Array.Empty<string>());

        public Task SetTagsForPostAsync(int postId, string[] tags, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
