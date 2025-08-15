namespace BlazorBlog.Services.Contracts;

public interface ISlugService
{
    string GenerateSlug(string input);
}
