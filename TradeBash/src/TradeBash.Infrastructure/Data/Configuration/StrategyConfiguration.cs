using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeBash.Core.Entities;

namespace TradeBash.Infrastructure.Data.Configuration
{
    public class StrategyConfiguration : IEntityTypeConfiguration<Strategy>
    {
        public void Configure(EntityTypeBuilder<Strategy> strategyBuilder)
        {
            /*strategyBuilder.HasKey(x => x.Id);

            strategyBuilder.Property(x => x.Id)
                .UseHiLo("orderseq", "ordering");

            strategyBuilder.Ignore(x => x.DomainEvents);*/

            strategyBuilder
                .Property<int>("_simpleMovingAverageParameter")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("SimpleMovingAverage")
                .IsRequired();

            strategyBuilder
                .Property<int>("_relativeStrengthIndexParameter")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("RelativeStrengthIndex")
                .IsRequired();
        }
    }
}