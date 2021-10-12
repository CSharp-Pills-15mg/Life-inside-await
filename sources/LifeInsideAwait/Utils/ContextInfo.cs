using System;
using System.Threading;

namespace DustInTheWind.CSharpPills.LifeInsideAwait.Utils
{
    internal static class ContextInfo
    {
        public static void Display(string title, int id)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            string synchronizationContext = SynchronizationContext.Current?.GetType().FullName ?? "<null>";

            Console.WriteLine($"{title} ({id})");
            Console.WriteLine($"    - Thread ({id}): {threadId}");
            Console.WriteLine($"    - SynchronizationContext ({id}): {synchronizationContext}");
        }
    }
}