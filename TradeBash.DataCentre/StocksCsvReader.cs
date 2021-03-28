﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
                    string[] fields = csvParser.ReadFields();
                    string symbol = fields[0];
                    string name = fields[1];
                    _stocksToUpdate.Add(new (symbol, name));
                }
            }

            return _stocksToUpdate;
        }
    }

    public interface IStocksCsvReader
    {
        IList<(string symbol, string name)> LoadFile(IndexVersion version);
    }

    public enum IndexVersion
    {
        Spy100,
        Spy500,
    }
}