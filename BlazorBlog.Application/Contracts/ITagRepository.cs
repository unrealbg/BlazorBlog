namespace BlazorBlog.Application.Contracts
{
    using System.Threading;

    using BlazorBlog.Application.Models;

    public interface ITagRepository
    {
        Task<TagVm[]> GetTopTagsAsync(int count, CancellationToken cancellationToken = default);

        Task<string[]> GetTagsForPostAsync(int postId, CancellationToken cancellationToken = default);

        Task SetTagsForPostAsync(int postId, string[] tags, CancellationToken cancellationToken = default);
    }
}
