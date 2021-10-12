using System.Threading.Tasks;
using DustInTheWind.CSharpPills.LifeInsideAwait.Utils;

namespace DustInTheWind.CSharpPills.LifeInsideAwait.AsynchronousWork
{
    internal class Work2_WithContinueWith : IWork
    {
        public virtual Task DoAsync(int stepIndex)
        {
            // before await
            ContextInfo.Display("Before await", stepIndex);

            // async call
            return Task.Delay(1000).ContinueWith(t =>
            {
                Continuation();
            });

            // after await
            void Continuation()
            {
                ContextInfo.Display("After await", stepIndex);
            }
        }
    }
}