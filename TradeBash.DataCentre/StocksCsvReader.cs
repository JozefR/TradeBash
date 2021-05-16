using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace TradeBash.DataCentre
{
    public class StocksCsvReader : IStocksCsvReader
    {
        private readonly string _filePath;
        private readonly IList<(string symbol, string name)> _stocksToUpdate;

        public StocksCsvReader()
        {
            _stocksToUpdate = new List<(string symbol, string name)>();
            _filePath = "StaticResources/{0}.txt";
        }

        public IList<(string symbol, string name)> LoadFile(IndexVersion version)
        {
            var path = String.Format(CultureInfo.InvariantCulture, _filePath, version);
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.SetDelimiters(",");
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    var fields = csvParser.ReadFields();
                    var symbol = fields[0];
                    var name = fields[1];
                    _stocksToUpdate.Add(new (symbol, name));
                }
            }

            return _stocksToUpdate;
        }

        public IList<(string symbol, string name)> GetAllToUpdate()
        {
            var stocksToUpdate = _stocksToUpdate.GroupBy(x => x.symbol)
                .Select(x => x.First())
                .OrderBy(x => x.symbol)
                .ToList();

            return stocksToUpdate;
        }
    }

    public interface IStocksCsvReader
    {
        IList<(string symbol, string name)> LoadFile(IndexVersion version);
        IList<(string symbol, string name)> GetAllToUpdate();
    }

    public enum IndexVersion
    {
        Spy100,
        Spy500,
        QQQ,
        VTV
    }
}