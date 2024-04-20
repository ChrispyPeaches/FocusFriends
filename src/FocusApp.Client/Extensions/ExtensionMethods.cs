using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Client.Extensions
{
    internal static class ExtensionMethods
    {
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
