using System.Collections.Generic;

namespace TradeBash.DataCentre
{
    public interface IStocksCsvReader
    {
        IList<(string symbol, string name)> LoadFile(IndexVersion version);
        IList<(string symbol, string name)> GetAllToUpdate();
    }
}