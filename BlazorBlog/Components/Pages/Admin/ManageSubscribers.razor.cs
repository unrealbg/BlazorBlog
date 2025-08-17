namespace BlazorBlog.Components.Pages.Admin
{
    using Subscriber = BlazorBlog.Domain.Entities.Subscriber;

    public partial class ManageSubscribers
    {
        private bool _isLoading;
    private bool _showLoader;
        private GridItemsProvider<Subscriber>? _subscribersProvider;

        private const int PageSize = 5;
        private PaginationState _paginationState = new PaginationState()
        {
            ItemsPerPage = PageSize
        };

    [Inject] 
    ISubscribeService SubscriberService { get; set; } = default!;

        protected override void OnInitialized()
        {
            _subscribersProvider = request =>
            {
                _isLoading = true;
                _ = Task.Run(async () =>
                {
                    await Task.Delay(250);
                    if (_isLoading)
                    {
                        _showLoader = true;
                        await InvokeAsync(StateHasChanged);
                    }
                });
                return LoadSubscribersAsync(request);
            };
        }

        private async ValueTask<GridItemsProviderResult<Subscriber>> LoadSubscribersAsync(GridItemsProviderRequest<Subscriber> request)
        {
            var result = await SubscriberService.GetSubscribersAsync(request.StartIndex, request.Count ?? PageSize, CancellationToken.None);

            _isLoading = false;
            _showLoader = false;
            StateHasChanged();

            return GridItemsProviderResult.From(result.Records, result.TotalCount);
        }
    }
}
