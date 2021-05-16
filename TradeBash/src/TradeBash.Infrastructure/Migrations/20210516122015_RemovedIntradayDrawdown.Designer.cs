﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TradeBash.Infrastructure.Data;

namespace TradeBash.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210516122015_RemovedIntradayDrawdown")]
    partial class RemovedIntradayDrawdown
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<int?>("_rsiParameter")
                        .HasColumnName("RSI")
                        .HasColumnType("int");

                    b.Property<int?>("_smaLongParameter")
                        .HasColumnName("SMALong")
                        .HasColumnType("int");

                    b.Property<int?>("_smaShortParameter")
                        .HasColumnName("SMAShort")
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

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.Strategy", b =>
                {
                    b.OwnsMany("TradeBash.Core.Entities.Strategy.GeneratedOrder", "GeneratedOrders", b1 =>
                        {
                            b1.Property<int>("StrategyId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("AdditionallyBoughtPositions")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("BudgetInvestedPercentage")
                                .HasColumnType("int");

                            b1.Property<DateTime?>("CloseDate")
                                .HasColumnType("datetime2");

                            b1.Property<double?>("ClosePrice")
                                .HasColumnType("float");

                            b1.Property<double>("CumulatedCapital")
                                .HasColumnType("float");

                            b1.Property<double>("DrawdownPercentage")
                                .HasColumnType("float");

                            b1.Property<DateTime>("OpenDate")
                                .HasColumnType("datetime2");

                            b1.Property<double>("OpenPrice")
                                .HasColumnType("float");

                            b1.Property<int>("Position")
                                .HasColumnType("int");

                            b1.Property<double?>("ProfitLoss")
                                .HasColumnType("float");

                            b1.Property<string>("Symbol")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("StrategyId", "Id");

                            b1.ToTable("GeneratedOrders");

                            b1.WithOwner()
                                .HasForeignKey("StrategyId");
                        });
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Strategy.StrategyStock", b =>
                {
                    b.HasOne("TradeBash.Core.Entities.Strategy.Strategy", null)
                        .WithMany("StrategyStocksHistory")
                        .HasForeignKey("StrategyId");

                    b.OwnsMany("TradeBash.Core.Entities.Strategy.CalculatedStock", "CalculatedStocksHistory", b1 =>
                        {
                            b1.Property<int>("StrategyStockId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<double>("Close")
                                .HasColumnType("float");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("datetime2");

                            b1.Property<double>("Low")
                                .HasColumnType("float");

                            b1.Property<double>("Open")
                                .HasColumnType("float");

                            b1.Property<double?>("RSI")
                                .HasColumnType("float");

                            b1.Property<double?>("SMALong")
                                .HasColumnType("float");

                            b1.Property<double?>("SMAShort")
                                .HasColumnType("float");

                            b1.Property<string>("Symbol")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("StrategyStockId", "Id");

                            b1.ToTable("CalculatedStocksHistory");

                            b1.WithOwner()
                                .HasForeignKey("StrategyStockId");
                        });
                });

            modelBuilder.Entity("TradeBash.Core.Entities.Warehouse.Stock", b =>
                {
                    b.OwnsMany("TradeBash.Core.Entities.Warehouse.StockHistory", "History", b1 =>
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
