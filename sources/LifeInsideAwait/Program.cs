using System;
using System.Threading;
using System.Threading.Tasks;
using DustInTheWind.ConsoleTools;
using DustInTheWind.CSharpPills.LifeInsideAwait.AsynchronousWork;

namespace DustInTheWind.CSharpPills.LifeInsideAwait
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            SynchronizationContext synchronizationContext = new CustomSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);

            try
            {
                await DoSomethingAsync();
            }
            finally
            {
                Console.WriteLine("Done");
            }
        }

        private static async Task DoSomethingAsync()
        {
            IWork work = new Work3_WithContinueWithAndSynchronizationContext();

            for (int i = 0; i < 4; i++)
            {
                await work.DoAsync(i);
                HorizontalLine.QuickDisplay();
            }
        }
    }
}