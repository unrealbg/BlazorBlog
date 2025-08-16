namespace BlazorBlog.Infrastructure.Contracts
{
    public interface ISlugService
    {
        string GenerateSlug(string input);
    }
}
