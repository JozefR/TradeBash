using System.Threading.Tasks;
using TradeBash.SharedKernel;

namespace TradeBash.SharedKernel.Interfaces
{
    public interface IHandle<in T> where T : BaseDomainEvent
    {
        Task Handle(T domainEvent);
    }
}