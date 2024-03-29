﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeBash.Core.Entities.Warehouse;

namespace TradeBash.Infrastructure.Data.Repositories
{
    public class StockRepository : EfRepository, IStockRepository
    {
        public StockRepository(AppDbContext dbContext) : base(dbContext) { }

        public Task<Stock> GetBySymbolAsync(string symbol)
        {
            return _dbContext.Set<Stock>()
                .Include(x => x.History)
                .SingleOrDefaultAsync(e => e.Symbol == symbol);
        }
    }
}