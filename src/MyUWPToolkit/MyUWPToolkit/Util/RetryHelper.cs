using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyUWPToolkit.Util
{
    /// <summary>
    /// 重试帮助类
    /// </summary>
    public static class RetryHelper
    {
        public static async Task RunWithDelayAsync(
           Func<Task> asyncFunc,
           CancellationToken ct = default(CancellationToken),
           Func<Exception, bool> throwFilter = null,
           int millisecondsDelay = 100,
           int retries = 3,
           bool throwOnFail = true)
        {
            ct.ThrowIfCancellationRequested();

            int attempts = 0;

            while (true)
            {
                attempts++;

                try
                {
                    await asyncFunc();
                    return;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex) when (throwFilter != null && throwFilter(ex))
                {
                    throw;
                }
                catch
                {
                    if (attempts > retries)
                    {
                        if (throwOnFail)
                            throw;

                        return;
                    }

                    if (attempts > 1)
                        millisecondsDelay *= 2;
                }

                await Task.Delay(millisecondsDelay, ct);
            }
        }

        public static async Task<TResult> RunWithDelayAsync<TResult>(
            Func<Task<TResult>> asyncFunc,
            CancellationToken ct = default(CancellationToken),
            Func<Exception, bool> throwFilter = null,
            int millisecondsDelay = 100,
            int retries = 3,
            bool throwOnFail = true)
        {
            ct.ThrowIfCancellationRequested();

            int attempts = 0;

            while (true)
            {
                attempts++;

                try
                {
                    return await asyncFunc();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex) when (throwFilter != null && throwFilter(ex))
                {
                    throw;
                }
                catch
                {
                    if (attempts > retries)
                    {
                        if (throwOnFail)
                            throw;

                        return default(TResult);
                    }

                    if (attempts > 1)
                        millisecondsDelay *= 2;
                }

                await Task.Delay(millisecondsDelay, ct);
            }
        }
    }
}
