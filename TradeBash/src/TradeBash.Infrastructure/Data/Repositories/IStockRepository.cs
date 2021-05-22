using System.Threading.Tasks;
using JetBrains.Annotations;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Infrastructure.Data.Repositories
{
    public interface IStockRepository : IRepository
    {
        Task<Stock> GetBySymbolAsync(string symbol);
    }
}