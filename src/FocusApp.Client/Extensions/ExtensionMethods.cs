namespace FocusApp.Client.Extensions
{
    internal static class ExtensionMethods
    {
        // Source: https://stackoverflow.com/a/53460032
        public static Task RunInNewThread(this Action action)
        {
            TaskCompletionSource<bool> taskCompletionSource = new();

            Thread thread = new Thread(() =>
            {
                try
                {
                    action();
                    taskCompletionSource.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.TrySetException(ex);
                }
            });

            thread.Start();

            return taskCompletionSource.Task;
        }
    }
}
