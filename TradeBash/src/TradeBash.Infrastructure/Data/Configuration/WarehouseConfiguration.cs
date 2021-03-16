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

            builder.Property(x => x.Id)
                .UseHiLo("orderseq", "ordering");

            /*
            builder.Ignore(x => x.DomainEvents);
            */

            builder.OwnsMany(o => o.History).ToTable("StocksHistory");
        }
    }
}