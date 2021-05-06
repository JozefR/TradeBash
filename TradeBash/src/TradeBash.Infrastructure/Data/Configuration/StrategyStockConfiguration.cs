using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Infrastructure.Data.Configuration
{
    public class StrategyStockConfiguration : IEntityTypeConfiguration<StrategyStock>
    {
        public void Configure(EntityTypeBuilder<StrategyStock> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsMany(o => o.CalculatedStocksHistory).ToTable("CalculatedStocksHistory");
        }
    }
}