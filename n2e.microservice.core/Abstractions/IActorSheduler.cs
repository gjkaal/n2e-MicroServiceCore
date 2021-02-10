using System.Threading;
using System.Threading.Tasks;

namespace n2e.MicroService.Core.Abstractions
{
    public interface IActorSheduler
    {        
        int Register(IActor ts);
        Task Start(int timerInterval, CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
    }
}
