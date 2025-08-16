namespace BlazorBlog.Application.Validators
{
    using BlazorBlog.Domain.Entities;
    using FluentValidation;

    public class SubscriberValidator : AbstractValidator<Subscriber>
    {
        public SubscriberValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(25);
        }
    }
}
