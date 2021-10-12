using System.Threading.Tasks;

namespace DustInTheWind.CSharpPills.LifeInsideAwait.AsynchronousWork
{
    internal interface IWork
    {
        Task DoAsync(int stepIndex);
    }
}