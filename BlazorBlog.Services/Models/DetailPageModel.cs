namespace BlazorBlog.Services.Models;

public record DetailPageModel(BlogPost BlogPost, BlogPost[] RelatedPosts)
{
    public static DetailPageModel Empty() => new(default, []);

    public bool IsEmpty => BlogPost is null;
}