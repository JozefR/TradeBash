using System.Threading.Tasks;
using TradeBash.SharedKernel;

namespace TradeBash.SharedKernel.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(BaseDomainEvent domainEvent);
    }
}