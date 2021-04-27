using System.Threading.Tasks;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Infrastructure.Data.Repositories
{
    public interface IStrategyRepository
    {
        Task<Strategy> GetByIdAsync(int id);
        Task<Strategy> GetByNameAsync(string name);
    }
}