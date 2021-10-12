using System.Threading.Tasks;
using DustInTheWind.CSharpPills.LifeInsideAwait.Utils;

namespace DustInTheWind.CSharpPills.LifeInsideAwait.AsynchronousWork
{
    internal class Work1_WithRealAsyncAwait : IWork
    {
        public virtual async Task DoAsync(int stepIndex)
        {
            // before await
            ContextInfo.Display("Before await", stepIndex);

            // async call
            await Task.Delay(1000).ConfigureAwait(true);

            // after await
            ContextInfo.Display("After await", stepIndex);
        }
    }
}