using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeBash.Core.Entities;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Infrastructure.Data.Configuration
{
    public class StrategyConfiguration : IEntityTypeConfiguration<Strategy>
    {
        public void Configure(EntityTypeBuilder<Strategy> strategyBuilder)
        {
            strategyBuilder.HasKey(x => x.Id);

            strategyBuilder
                .Property<string>("_name")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Name")
                .IsRequired();

            strategyBuilder
                .Property<int?>("_simpleMovingAverageParameter")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("SimpleMovingAverage");

            strategyBuilder
                .Property<int?>("_relativeStrengthIndexParameter")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("RelativeStrengthIndex");
        }
    }
}