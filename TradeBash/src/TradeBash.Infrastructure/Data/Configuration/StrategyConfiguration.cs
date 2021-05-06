using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Infrastructure.Data.Configuration
{
    public class StrategyConfiguration : IEntityTypeConfiguration<Strategy>
    {
        public void Configure(EntityTypeBuilder<Strategy> strategyBuilder)
        {
            strategyBuilder.HasKey(x => x.Id);

            strategyBuilder
                .Property<string>("Name")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Name")
                .IsRequired();

            strategyBuilder
                .Property<int?>("_smaShortParameter")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("SMAShort");

            strategyBuilder
                .Property<int?>("_smaLongParameter")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("SMALong");

            strategyBuilder
                .Property<int?>("_rsiParameter")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("RSI");

            strategyBuilder.OwnsMany(o => o.GeneratedOrders).ToTable("GeneratedOrders");
        }
    }
}