using System.Threading.Tasks;
using TradeBash.SharedKernel.Interfaces;
using TradeBash.SharedKernel;

namespace TradeBash.UnitTests
{
    public class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task Dispatch(BaseDomainEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
