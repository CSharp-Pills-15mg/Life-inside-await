using System.Threading;
using System.Threading.Tasks;
using DustInTheWind.CSharpPills.LifeInsideAwait.Utils;

namespace DustInTheWind.CSharpPills.LifeInsideAwait.AsynchronousWork
{
    internal class Work3_WithContinueWithAndSynchronizationContext : IWork
    {
        public virtual Task DoAsync(int stepIndex)
        {
            // before await
            ContextInfo.Display("Before await", stepIndex);

            // async call

            SynchronizationContext capturedSynchronizationContext = SynchronizationContext.Current;

            return Task.Delay(1000).ContinueWith(t =>
            {
                if (capturedSynchronizationContext == null)
                {
                    Continuation();
                }
                else
                {
                    ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(false);

                    void SendOrPostCallback(object state)
                    {
                        Continuation();
                        manualResetEventSlim.Set();
                    }

                    capturedSynchronizationContext.Post(SendOrPostCallback, null);

                    manualResetEventSlim.Wait();
                }
            });

            // after await
            void Continuation()
            {
                ContextInfo.Display("After await", stepIndex);
            }
        }
    }
}