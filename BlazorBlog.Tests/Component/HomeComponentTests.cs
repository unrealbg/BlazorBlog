namespace BlazorBlog.Tests.Component
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Bunit;
    using Xunit;
    using BlazorBlog.Components.Pages;
    using BlazorBlog.Tests.Fakes;

    using Microsoft.Extensions.DependencyInjection;
    using BlazorBlog.Application.Models;

    public class HomeComponentTests
    {
        [Fact]
        public void Home_Render_DoesNotThrow()
        {
            using var ctx = new TestContext();
            // Minimal DI to render; the component renders before OnInitializedAsync completes
            ctx.Services.AddSingleton<BlazorBlog.Application.Contracts.IBlogPostService>(new FakeBlogPostService());
            ctx.Services.AddSingleton<BlazorBlog.Infrastructure.Contracts.ITagService>(new FakeTagService());
            ctx.Services.AddSingleton<BlazorBlog.Infrastructure.Contracts.ISubscribeService, DummySubscribeService>();
            // Register validator used by SubscribeBox
            ctx.Services.AddScoped<FluentValidation.IValidator<BlazorBlog.Domain.Entities.Subscriber>, DummySubscriberValidator>();

            var cut = ctx.RenderComponent<Home>();
            Assert.NotNull(cut.Instance);
        }
    }

    // Minimal implementations to satisfy DI for SubscribeBox
    internal class DummySubscribeService : BlazorBlog.Infrastructure.Contracts.ISubscribeService
    {
        public Task<string?> AddSubscriberAsync(BlazorBlog.Domain.Entities.Subscriber subscriber, CancellationToken cancellationToken = default)
            => Task.FromResult<string?>(null);

        public Task<PageResult<BlazorBlog.Domain.Entities.Subscriber>> GetSubscribersAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default)
            => Task.FromResult(new PageResult<BlazorBlog.Domain.Entities.Subscriber>(Array.Empty<BlazorBlog.Domain.Entities.Subscriber>(), 0));
    }

    internal class GenericNoopValidator<T> : FluentValidation.IValidator<T>
    {
        public FluentValidation.Results.ValidationResult Validate(FluentValidation.IValidationContext context)
            => new FluentValidation.Results.ValidationResult();

        public Task<FluentValidation.Results.ValidationResult> ValidateAsync(FluentValidation.IValidationContext context, CancellationToken cancellation = default)
            => Task.FromResult(new FluentValidation.Results.ValidationResult());

        public FluentValidation.Results.ValidationResult Validate(FluentValidation.ValidationContext<T> context)
            => new FluentValidation.Results.ValidationResult();

        public Task<FluentValidation.Results.ValidationResult> ValidateAsync(FluentValidation.ValidationContext<T> context, CancellationToken cancellation = default)
            => Task.FromResult(new FluentValidation.Results.ValidationResult());

        public FluentValidation.Results.ValidationResult Validate(T instance)
            => new FluentValidation.Results.ValidationResult();

        public Task<FluentValidation.Results.ValidationResult> ValidateAsync(T instance, CancellationToken cancellation = default)
            => Task.FromResult(new FluentValidation.Results.ValidationResult());

        public FluentValidation.IValidatorDescriptor CreateDescriptor() => throw new NotImplementedException();

        public bool CanValidateInstancesOfType(Type type) => true;
    }

    internal sealed class DummySubscriberValidator : GenericNoopValidator<BlazorBlog.Domain.Entities.Subscriber>
    {
    }
}
