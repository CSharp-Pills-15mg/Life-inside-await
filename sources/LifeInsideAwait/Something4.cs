using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitContext.Demo
{
    internal class Something4 : ISomething
    {
        private ContextCallback? postActionCallback;

        private readonly SendOrPostCallback postCallback = state =>
        {
            ((Action)state)();
        };

        private SynchronizationContext capturedSynchronizationContext;

        /// <summary>
        /// Capture <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="stepIndex"></param>
        /// <returns></returns>
        public virtual Task DoAsync(int stepIndex)
        {
            // before await
            ContextInfo.Display("Before await", stepIndex);

            // async call

            capturedSynchronizationContext = SynchronizationContext.Current;
            ExecutionContext capturedExecutionContext = ExecutionContext.Capture();

            return Task.Delay(1000).ContinueWith(t =>
            {
                if (capturedSynchronizationContext == null)
                {
                    Continuation();
                }
                else
                {
                    //System.Runtime.CompilerServices.TaskAwaiter
                    //SynchronizationContextAwaitTaskContinuation
                    //ExecutionContext.RunInternal

                    //if (capturedSynchronizationContext.IsWaitNotificationRequired())
                    //    capturedSynchronizationContext.Send(state => Continuation(), null);
                    //else
                    //capturedSynchronizationContext.Post(state => Continuation(), null);

                    Action state = new Action(Continuation);

                    if (capturedExecutionContext == null)
                    {
                        // If there's no captured context, just run the callback directly.
                        postCallback(state);
                    }
                    else
                    {
                        // Otherwise, use the captured context to do so.
                        //ExecutionContext.RunInternal(capturedExecutionContext, postCallback, state);
                        ExecutionContext.Run(capturedExecutionContext, GetPostActionCallback(), state);
                    }
                }
            });

            ContextCallback GetPostActionCallback() => postActionCallback ??= PostAction;


            // after await
            void Continuation()
            {
                ContextInfo.Display("After await", stepIndex);
            }
        }

        private void PostAction(object? state)
        {
            capturedSynchronizationContext.Post(postCallback, state);
        }
    }
}