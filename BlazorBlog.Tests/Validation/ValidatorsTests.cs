namespace BlazorBlog.Tests.Validation
{
    using BlazorBlog.Application.Models;
    using BlazorBlog.Application.Validators;
    using BlazorBlog.Domain.Entities;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Xunit;

    public class ValidatorsTests
    {
        [Fact]
        public void BlogPostVmValidator_Validates_Fields()
        {
            var v = new BlogPostVmValidator();
            var invalid = new BlogPostVm();
            var res = v.Validate(invalid);
            Assert.Contains(res.Errors, e => e.PropertyName == nameof(BlogPostVm.Title));
            Assert.Contains(res.Errors, e => e.PropertyName == nameof(BlogPostVm.Image));
            Assert.Contains(res.Errors, e => e.PropertyName == nameof(BlogPostVm.Introduction));
            Assert.Contains(res.Errors, e => e.PropertyName == nameof(BlogPostVm.Content));
            Assert.Contains(res.Errors, e => e.PropertyName == nameof(BlogPostVm.CategoryId));

            var valid = new BlogPostVm
            {
                Title = new string('a', 10),
                Image = "img.png",
                Introduction = "intro",
                Content = "content",
                CategoryId = 1
            };
            Assert.True(v.Validate(valid).IsValid);
        }

        [Fact]
        public void CategoryValidator_Validates_Fields()
        {
            var v = new CategoryValidator();
            var invalid = new Category();
            var res = v.Validate(invalid);
            Assert.Contains(res.Errors, e => e.PropertyName == nameof(Category.Name));

            var valid = new Category { Name = "C#", Slug = new string('s', 10) };
            Assert.True(v.Validate(valid).IsValid);
        }

        [Fact]
        public void SubscriberValidator_Validates_Fields()
        {
            var v = new SubscriberValidator();
            var invalid = new Subscriber();
            var res = v.Validate(invalid);
            Assert.Contains(res.Errors, e => e.PropertyName == nameof(Subscriber.Email));
            Assert.Contains(res.Errors, e => e.PropertyName == nameof(Subscriber.Name));

            var valid = new Subscriber { Email = "a@b.com", Name = "John" };
            Assert.True(v.Validate(valid).IsValid);
        }

        [Fact]
        public void SiteSettingsVm_Rejects_Unsafe_Hero_Links()
        {
            var settings = SiteSettingsVm.CreateDefault();
            settings.HomeHeroPrimaryButtonUrl = "javascript:alert(1)";

            var errors = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(settings, new ValidationContext(settings), errors, validateAllProperties: true);

            Assert.False(valid);
            Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SiteSettingsVm.HomeHeroPrimaryButtonUrl)));
        }
    }
}
