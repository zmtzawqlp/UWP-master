
using System;
using System.Threading;
using System.Threading.Tasks;

namespace  MyUWPToolkit.Util
{
    public static class TplExtensions
    {
        public static void Forget(this Task task) { }

        public static Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            ValidationHelper.ArgumentNotNull(task, nameof(task));

            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
                return task;

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            return WithCancellationSlow(task, cancellationToken);
        }

        public static Task<TResult> WithCancellation<TResult>(this Task<TResult> task, CancellationToken cancellationToken)
        {
            ValidationHelper.ArgumentNotNull(task, nameof(task));

            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
                return task;

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<TResult>(cancellationToken);

            return WithCancellationSlow(task, cancellationToken);
        }

        private static async Task WithCancellationSlow(Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);
            }
        }

        private static async Task<TResult> WithCancellationSlow<TResult>(Task<TResult> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);
            }
            return await task.ConfigureAwait(false);
        }

        //public static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        //{
        //    // Short-circuit #1: infinite timeout or task already completed
        //    if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
        //    {
        //        // Either the task has already completed or timeout will never occur.
        //        // No proxy necessary.
        //        return task;
        //    }

        //    // tcs.Task will be returned as a proxy to the caller
        //    var tcs = new TaskCompletionSource<object>();

        //    // Short-circuit #2: zero timeout
        //    if (millisecondsTimeout == 0)
        //    {
        //        // We've already timed out.
        //        tcs.SetException(new TimeoutException());
        //        return tcs.Task;
        //    }

        //    // Set up a timer to complete after the specified timeout period
        //    var timer = new Timer(state =>
        //    {
        //        // Recover your state information
        //        var myTcs = (TaskCompletionSource<object>)state;

        //        // Fault our proxy with a TimeoutException
        //        myTcs.TrySetException(new TimeoutException());
        //    }, tcs, millisecondsTimeout, Timeout.Infinite);

        //    // Wire up the logic for what happens when source task completes
        //    task.ContinueWith((antecedent, state) =>
        //    {
        //        // Recover our state data
        //        var (Timer, Tcs) = ((Timer, TaskCompletionSource<object>))state;

        //        // Cancel the Timer
        //        Timer.Dispose();

        //        // Marshal results to proxy
        //        MarshalTaskResults(antecedent, Tcs);
        //    },
        //    (timer, tcs),
        //    CancellationToken.None,
        //    TaskContinuationOptions.ExecuteSynchronously,
        //    TaskScheduler.Default);

        //    return tcs.Task;
        //}

        //public static Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsTimeout)
        //{
        //    // Short-circuit #1: infinite timeout or task already completed
        //    if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
        //    {
        //        // Either the task has already completed or timeout will never occur.
        //        // No proxy necessary.
        //        return task;
        //    }

        //    // tcs.Task will be returned as a proxy to the caller
        //    var tcs = new TaskCompletionSource<TResult>();

        //    // Short-circuit #2: zero timeout
        //    if (millisecondsTimeout == 0)
        //    {
        //        // We've already timed out.
        //        tcs.SetException(new TimeoutException());
        //        return tcs.Task;
        //    }

        //    // Set up a timer to complete after the specified timeout period
        //    var timer = new Timer(state =>
        //    {
        //        // Recover your state information
        //        var myTcs = (TaskCompletionSource<TResult>)state;

        //        // Fault our proxy with a TimeoutException
        //        myTcs.TrySetException(new TimeoutException());
        //    }, tcs, millisecondsTimeout, Timeout.Infinite);

        //    // Wire up the logic for what happens when source task completes
        //    task.ContinueWith((antecedent, state) =>
        //    {
        //        // Recover our state data
        //        var (Timer, Tcs) = ((Timer, TaskCompletionSource<TResult>))state;

        //        // Cancel the Timer
        //        Timer.Dispose();

        //        // Marshal results to proxy
        //        MarshalTaskResults(antecedent, Tcs);
        //    },
        //    (timer, tcs),
        //    CancellationToken.None,
        //    TaskContinuationOptions.ExecuteSynchronously,
        //    TaskScheduler.Default);

        //    return tcs.Task;
        //}

        private static void MarshalTaskResults<TResult>(Task source, TaskCompletionSource<TResult> proxy)
        {
            switch (source.Status)
            {
                case TaskStatus.Faulted:
                    proxy.TrySetException(source.Exception);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    var castedSource = source as Task<TResult>;
                    proxy.TrySetResult(castedSource == null ?
                        default(TResult) : // source is a Task
                        castedSource.Result); // source is a Task<TResult>
                    break;
            }
        }
    }
}
