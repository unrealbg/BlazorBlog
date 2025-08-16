namespace BlazorBlog.Application.Validators
{
    using BlazorBlog.Application.Models;
    using FluentValidation;

    public class BlogPostVmValidator : AbstractValidator<BlogPostVm>
    {
        public BlogPostVmValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("The title is required.")
                .MaximumLength(100);

            RuleFor(x => x.Image)
                .NotEmpty().WithMessage("The image is required.")
                .MaximumLength(100);

            RuleFor(x => x.Introduction)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("The content is required.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Please select a valid category.");
        }
    }
}
