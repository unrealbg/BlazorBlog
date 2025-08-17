namespace BlazorBlog.Application.UI
{
    /// <summary>
    /// Convenience helpers for showing toasts with duration specified in seconds.
    /// Does not change the IToastService contract; just forwards to the existing overload with milliseconds.
    /// </summary>
    public static class ToastServiceExtensions
    {
        /// <summary>
        /// Show a toast with a custom duration in seconds.
        /// </summary>
        /// <param name="service">The toast service.</param>
        /// <param name="level">Toast level.</param>
        /// <param name="message">Toast message.</param>
        /// <param name="heading">Optional heading.</param>
        /// <param name="durationSeconds">Duration in seconds. If null or less than or equal to zero, service defaults are used.</param>
        public static void ShowToast(this IToastService service, ToastLevel level, string message, string heading, int? durationSeconds)
        {
            if (service is null) throw new ArgumentNullException(nameof(service));

            int? ms = null;
            if (durationSeconds.HasValue && durationSeconds.Value > 0)
            {
                try
                {
                    checked
                    {
                        ms = durationSeconds.Value * 1000;
                    }
                }
                catch (OverflowException)
                {
                    ms = 24 * 60 * 60 * 1000;
                }
            }

            service.ShowToast(level, message, heading, ms);
        }

        /// <summary>
        /// Show a toast with a custom duration in seconds (message-only overload).
        /// </summary>
        public static void ShowToast(this IToastService service, ToastLevel level, string message, int? durationSeconds)
            => ShowToast(service, level, message, string.Empty, durationSeconds);
    }
}
