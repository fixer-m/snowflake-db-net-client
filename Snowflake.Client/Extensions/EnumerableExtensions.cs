using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Snowflake.Client.Extensions
{
    public static class EnumerableExtensions
    {
        public static async Task ForEachWithThrottleAsync<T>(this IEnumerable<T> items, Func<T, Task> function, int degreeOfParallelism)
        {
            var tasks = new List<Task>();
            using (var semaphoreSlim = new SemaphoreSlim(degreeOfParallelism))
            {
                foreach (var item in items)
                {
                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await function(item).ConfigureAwait(false);
                        }
                        finally
                        {
                            semaphoreSlim.Release();
                        }
                    });
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }
    }
}