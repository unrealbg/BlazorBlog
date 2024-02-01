namespace BlazorBlog.Data.Models
{
    public record PageResult<TResult>(TResult[] Records, int TotalCount);
}
