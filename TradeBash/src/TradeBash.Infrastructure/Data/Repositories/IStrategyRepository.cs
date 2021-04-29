using System.Collections.Generic;
using System.Threading.Tasks;
using TradeBash.Core.Entities.Strategy;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Infrastructure.Data.Repositories
{
    public interface IStrategyRepository : IRepository
    {
        Task<Strategy> GetByIdAsync(int id);
        Task<Strategy> GetByNameAsync(string name);
        Task<List<Strategy>> GetAllAsync();
    }
}