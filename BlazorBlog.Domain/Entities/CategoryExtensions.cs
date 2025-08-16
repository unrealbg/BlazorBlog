namespace BlazorBlog.Domain.Entities
{
    public static class CategoryExtensions
    {
        public static Category Clone(this Category source) => new()
                                                                  {
                                                                      Id = source.Id,
                                                                      Name = source.Name,
                                                                      Slug = source.Slug,
                                                                      ShowOnNavBar = source.ShowOnNavBar
                                                                  };
    }
}
