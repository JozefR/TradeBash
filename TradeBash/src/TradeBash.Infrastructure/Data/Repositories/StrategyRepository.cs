using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Infrastructure.Data.Repositories
{
    public class StrategyRepository : EfRepository, IStrategyRepository
    {
        public StrategyRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<Strategy> GetByIdAsync(int id)
        {
            return _dbContext.Set<Strategy>()
                .Include(x => x.StrategyStockHistory)
                .ThenInclude(xx => xx.OrderedStocksHistory)
                .SingleOrDefaultAsync(e => e.Id == id);
        }

        public Task<Strategy> GetByNameAsync(string name)
        {
            return _dbContext.Set<Strategy>()
                .Include(x => x.StrategyStockHistory)
                .ThenInclude(xx => xx.OrderedStocksHistory)
                .Include(x => x.GeneratedOrders)
                .SingleOrDefaultAsync(e => e.Name == name);
        }

        public Task<List<Strategy>> GetAllAsync()
        {
            return _dbContext.Set<Strategy>()
                .Include(x => x.StrategyStockHistory)
                .ThenInclude(xx => xx.OrderedStocksHistory)
                .Include(x => x.GeneratedOrders)
                .ToListAsync();
        }
    }
}