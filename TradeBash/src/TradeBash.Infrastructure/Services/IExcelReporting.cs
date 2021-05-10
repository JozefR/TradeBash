using System.Threading.Tasks;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Infrastructure.Services
{
    public interface IExcelReporting
    {
        Task GenerateAsync(Strategy strategy);
    }
}