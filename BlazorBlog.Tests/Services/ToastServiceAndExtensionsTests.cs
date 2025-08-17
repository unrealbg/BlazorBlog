namespace BlazorBlog.Tests.Services
{
    using BlazorBlog.Application.UI;
    using BlazorBlog.Infrastructure;
    using Xunit;

    public class ToastServiceAndExtensionsTests
    {
        [Fact]
        public void ToastService_Raises_OnShow_For_ShowToast_And_RemoveAll()
        {
            var svc = new ToastService();
            int calls = 0;
            ToastLevel lastLevel = default;
            string lastMessage = string.Empty;
            string lastHeading = string.Empty;
            int? lastDuration = null;

            svc.OnShow += (lvl, msg, hdg, dur) => { calls++; lastLevel = lvl; lastMessage = msg; lastHeading = hdg; lastDuration = dur; };

            svc.ShowToast(ToastLevel.Success, "hello", "hi", 2500);
            Assert.Equal(1, calls);
            Assert.Equal(ToastLevel.Success, lastLevel);
            Assert.Equal("hello", lastMessage);
            Assert.Equal("hi", lastHeading);
            Assert.Equal(2500, lastDuration);

            svc.RemoveAll();
            Assert.Equal(2, calls);
            Assert.Equal(string.Empty, lastMessage);
        }

        [Theory]
        [InlineData(2, 2000)]
        [InlineData(null, null)]
        [InlineData(0, null)]
        public void ToastExtensions_Converts_Seconds_To_Milliseconds(int? seconds, int? expectedMs)
        {
            var svc = new ToastService();
            int? seen = null;
            svc.OnShow += (_, _, _, dur) => seen = dur;

            // Explicitly call the extension method to avoid instance overload
            ToastServiceExtensions.ShowToast(svc, ToastLevel.Info, "m", "h", seconds);
            Assert.Equal(expectedMs, seen);
        }

        [Fact]
        public void ToastExtensions_Overflow_Sets_Max_Day()
        {
            var svc = new ToastService();
            int? seen = null;
            svc.OnShow += (_, _, _, dur) => seen = dur;

            // int.MaxValue seconds would overflow when multiplied by 1000
            ToastServiceExtensions.ShowToast(svc, ToastLevel.Info, "m", "h", int.MaxValue);
            Assert.Equal(24 * 60 * 60 * 1000, seen);
        }
    }
}
