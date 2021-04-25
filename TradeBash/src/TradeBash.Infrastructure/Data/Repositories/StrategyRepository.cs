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
                .Include(x => x.StocksHistory)
                .ThenInclude(xx => xx.CalculatedStocksHistory)
                .SingleOrDefaultAsync(e => e.Id == id);
        }
    }
}