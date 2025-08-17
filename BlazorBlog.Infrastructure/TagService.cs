namespace BlazorBlog.Infrastructure
{
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Infrastructure.Contracts;

    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;

        public TagService(ITagRepository repo)
        {
            _repo = repo;
        }

    public Task<BlazorBlog.Application.Models.TagVm[]> GetTopTagsAsync(int count = 20, CancellationToken cancellationToken = default)
            => _repo.GetTopTagsAsync(count, cancellationToken);

        public Task<string[]> GetTagsForPostAsync(int postId, CancellationToken cancellationToken = default)
            => _repo.GetTagsForPostAsync(postId, cancellationToken);

        public Task SetTagsForPostAsync(int postId, string[] tags, CancellationToken cancellationToken = default)
            => _repo.SetTagsForPostAsync(postId, tags, cancellationToken);
    }
}
