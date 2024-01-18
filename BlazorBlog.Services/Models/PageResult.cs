namespace BlazorBlog.Services.Models
{
    public record PageResult<TResult>(TResult[] Records, int TotalCount);
}
