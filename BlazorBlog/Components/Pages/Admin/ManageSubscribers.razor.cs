namespace BlazorBlog.Components.Pages.Admin
{
    public partial class ManageSubscribers
    {
        private bool _isLoading;
        private GridItemsProvider<Subscriber>? _subscribersProvider;

        private const int PageSize = 5;
        private PaginationState _paginationState = new PaginationState()
        {
            ItemsPerPage = PageSize
        };

        [Inject] 
        ISubscribeService SubscriberService { get; set; }

        protected override void OnInitialized()
        {
            _subscribersProvider = async request =>
            {
                _isLoading = true;
                StateHasChanged();

                var result = await SubscriberService.GetSubscribersAsync(request.StartIndex, request.Count ?? PageSize);

                _isLoading = false;
                StateHasChanged();

                return GridItemsProviderResult.From(result.Records, result.TotalCount);
            };
        }
    }
}
