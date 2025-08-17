namespace BlazorBlog.Infrastructure.Contracts
{
    using System.Threading;

    using BlazorBlog.Application.Models;

    public interface ITagService
    {
        Task<TagVm[]> GetTopTagsAsync(int count, CancellationToken cancellationToken = default);

        Task<string[]> GetTagsForPostAsync(int postId, CancellationToken cancellationToken = default);

        Task SetTagsForPostAsync(int postId, string[] tags, CancellationToken cancellationToken = default);
    }
}
