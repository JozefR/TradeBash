using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeBash.Core.Entities.Warehouse;

namespace TradeBash.Infrastructure.Data.Configuration
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsMany(o => o.OrderedHistory).ToTable("StocksHistory");
        }
    }
}