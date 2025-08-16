namespace BlazorBlog.Application.Models
{
    public sealed class PageResult<T>
    {
        public PageResult(T[] records, int totalCount)
        {
            Records = records;
            TotalCount = totalCount;
        }

        public T[] Records { get; }

        public int TotalCount { get; }
    }
}
