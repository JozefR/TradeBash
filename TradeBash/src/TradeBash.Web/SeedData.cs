using TradeBash.Core.Entities;
using TradeBash.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace TradeBash.Web
{
    public static class SeedData
    {
        public static readonly ToDoItem ToDoItem1 = new ToDoItem
        {
            Title = "Get Sample Working",
            Description = "Try to get the sample to build."
        };
        public static readonly ToDoItem ToDoItem2 = new ToDoItem
        {
            Title = "Review Solution",
            Description = "Review the different projects in the solution and how they relate to one another."
        };
        public static readonly ToDoItem ToDoItem3 = new ToDoItem
        {
            Title = "Run and Review Tests",
            Description = "Make sure all the tests run and review what they are doing."
        };

        public static readonly Stock Stock1 = new Stock
        {
            Id = 1,
            Date = DateTime.Now,
            Close = 100,
            Label = "Apple",
            Open = 96,
            Symbol = "Appl"
        };

        public static readonly Stock Stock2 = new Stock
        {
            Id = 2,
            Date = DateTime.Now,
            Close = 103,
            Label = "Apple",
            Open = 99,
            Symbol = "Appl"
        };

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>(), null))
            {
                PopulateTestData(dbContext);
            }
        }
        
        public static void PopulateTestData(AppDbContext dbContext)
        {
            foreach (var item in dbContext.ToDoItems)
            {
                dbContext.Remove(item);
            }

            dbContext.SaveChanges();
            dbContext.ToDoItems.Add(ToDoItem1);
            dbContext.ToDoItems.Add(ToDoItem2);
            dbContext.ToDoItems.Add(ToDoItem3);

            dbContext.SaveChanges();
            
            foreach (var item in dbContext.Stocks)
            {
                dbContext.Remove(item);
            }

            dbContext.SaveChanges();
            dbContext.Stocks.Add(Stock1);
            dbContext.Stocks.Add(Stock2);

            dbContext.SaveChanges();
        }
    }
}
