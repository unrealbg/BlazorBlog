namespace BlazorBlog.Services.Models
{
    public record PageResult<TResult>(TResult[] Results, int TotalCount);
}
