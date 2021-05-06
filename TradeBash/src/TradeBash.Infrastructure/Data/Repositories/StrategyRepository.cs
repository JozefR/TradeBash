﻿using System.Collections.Generic;
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
                .Include(x => x.StrategyStocksHistory)
                .ThenInclude(xx => xx.CalculatedStocksHistory)
                .SingleOrDefaultAsync(e => e.Id == id);
        }

        public Task<Strategy> GetByNameAsync(string name)
        {
            return _dbContext.Set<Strategy>()
                .Include(x => x.StrategyStocksHistory)
                .ThenInclude(xx => xx.CalculatedStocksHistory)
                .Include(x => x.GeneratedOrders)
                .SingleOrDefaultAsync(e => e.Name == name);
        }

        public Task<List<Strategy>> GetAllGeneratedOrdersAsync()
        {
            return _dbContext.Set<Strategy>()
                .Include(x => x.GeneratedOrders)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}