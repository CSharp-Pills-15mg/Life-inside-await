using System.Threading;

namespace DustInTheWind.CSharpPills.LifeInsideAwait
{
    internal class CustomSynchronizationContext : SynchronizationContext
    {
        public override void Send(SendOrPostCallback d, object state)
        {
            d.Invoke(state);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                SetSynchronizationContext(this);
                d.Invoke(x);
            }, state);
        }

        public override SynchronizationContext CreateCopy()
        {
            return new CustomSynchronizationContext();
        }
    }
}