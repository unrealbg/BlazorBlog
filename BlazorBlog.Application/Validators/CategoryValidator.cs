namespace BlazorBlog.Application.Validators
{
    using BlazorBlog.Domain.Entities;
    using FluentValidation;

    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Slug)
                .MaximumLength(75);
        }
    }
}
