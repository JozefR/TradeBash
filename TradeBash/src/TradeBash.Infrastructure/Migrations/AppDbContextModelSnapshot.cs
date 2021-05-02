﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TradeBash.Infrastructure.Data;

namespace TradeBash.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.CalculatedStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Close")
                        .HasColumnType("float");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<double>("Open")
                        .HasColumnType("float");

                    b.Property<double?>("RSI")
                        .HasColumnType("float");

                    b.Property<double?>("SMALong")
                        .HasColumnType("float");

                    b.Property<double?>("SMAShort")
                        .HasColumnType("float");

                    b.Property<int?>("StrategyStockId")
                        .HasColumnType("int");

                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StrategyStockId");

                    b.ToTable("CalculatedStock");
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.GeneratedOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetInvestedPercentage")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CloseDate")
                        .HasColumnType("datetime2");

                    b.Property<double?>("ClosePrice")
                        .HasColumnType("float");

                    b.Property<DateTime>("OpenDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("OpenPrice")
                        .HasColumnType("float");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<double?>("ProfitLoss")
                        .HasColumnType("float");

                    b.Property<int?>("StrategyId")
                        .HasColumnType("int");

                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StrategyId");

                    b.ToTable("GeneratedOrder");
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.Strategy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Budget")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("_relativeStrengthIndexParameter")
                        .HasColumnName("RelativeStrengthIndex")
                        .HasColumnType("int");

                    b.Property<int?>("_simpleMovingAverageParameter")
                        .HasColumnName("SimpleMovingAverage")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Strategies");
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.StrategyStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StrategyId")
                        .HasColumnType("int");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StrategyId");

                    b.ToTable("StrategyStock");
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Warehouse.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.CalculatedStock", b =>
                {
                    b.HasOne("TradeBash.Core.Entities.Strategy.StrategyStock", null)
                        .WithMany("OrderedStocksHistory")
                        .HasForeignKey("StrategyStockId");
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.GeneratedOrder", b =>
                {
                    b.HasOne("TradeBash.Core.Entities.Strategy.Strategy", null)
                        .WithMany("GeneratedOrders")
                        .HasForeignKey("StrategyId");
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.StrategyStock", b =>
                {
                    b.HasOne("TradeBash.Core.Entities.Strategy.Strategy", null)
                        .WithMany("StocksHistory")
                        .HasForeignKey("StrategyId");
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Warehouse.Stock", b =>
                {
                    b.OwnsMany("TradeBash.Core.Entities.Warehouse.StockHistory", "OrderedHistory", b1 =>
                        {
                            b1.Property<int>("StockId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<double>("Change")
                                .HasColumnType("float");

                            b1.Property<double>("ChangeOverTime")
                                .HasColumnType("float");

                            b1.Property<double>("ChangePercent")
                                .HasColumnType("float");

                            b1.Property<double>("Close")
                                .HasColumnType("float");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("datetime2");

                            b1.Property<double>("FClose")
                                .HasColumnType("float");

                            b1.Property<double>("FHigh")
                                .HasColumnType("float");

                            b1.Property<double>("FLow")
                                .HasColumnType("float");

                            b1.Property<double>("FOpen")
                                .HasColumnType("float");

                            b1.Property<double>("FVolume")
                                .HasColumnType("float");

                            b1.Property<double>("High")
                                .HasColumnType("float");

                            b1.Property<string>("Label")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<double>("Low")
                                .HasColumnType("float");

                            b1.Property<double>("MarketChangeOverTime")
                                .HasColumnType("float");

                            b1.Property<double>("Open")
                                .HasColumnType("float");

                            b1.Property<double>("UClose")
                                .HasColumnType("float");

                            b1.Property<double>("UHigh")
                                .HasColumnType("float");

                            b1.Property<double>("ULow")
                                .HasColumnType("float");

                            b1.Property<double>("UOpen")
                                .HasColumnType("float");

                            b1.Property<double>("UVolume")
                                .HasColumnType("float");

                            b1.Property<double>("Volume")
                                .HasColumnType("float");

                            b1.HasKey("StockId", "Id");

                            b1.ToTable("StocksHistory");

                            b1.WithOwner()
                                .HasForeignKey("StockId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
