namespace BlazorBlog.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(75)]
        public string Slug { get; set; }

        public bool ShowOnNavBar { get; set; }

        public Category Clone() => (this.MemberwiseClone() as Category)!;

        public static Category[] GetSeedCategories()
        {
            return new[]
            {
                new Category { Name = "Blazor", Slug = "blazor", ShowOnNavBar = true },
                new Category { Name = "C#", Slug = "csharp", ShowOnNavBar = true },
                new Category { Name = "CSS", Slug = "css", ShowOnNavBar = true },
                new Category { Name = "Docker", Slug = "docker", ShowOnNavBar = false },
                new Category { Name = "Git", Slug = "git", ShowOnNavBar = false },
                new Category { Name = "HTML", Slug = "html", ShowOnNavBar = true },
                new Category { Name = "JavaScript", Slug = "javascript", ShowOnNavBar = true },
                new Category { Name = "Linux", Slug = "linux", ShowOnNavBar = false },
                new Category { Name = "SQL", Slug = "sql", ShowOnNavBar = true },
                new Category { Name = "Visual Studio", Slug = "visual-studio", ShowOnNavBar = false },
                new Category { Name = "Windows", Slug = "windows", ShowOnNavBar = false },
                new Category { Name = "Xamarin", Slug = "xamarin", ShowOnNavBar = true },
                new Category { Name = "XAML", Slug = "xaml", ShowOnNavBar = true },
                new Category { Name = "XML", Slug = "xml", ShowOnNavBar = true },
                new Category { Name = "MVC", Slug = "mvc", ShowOnNavBar = true }
            };
        }
    }
}
