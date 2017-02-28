using StockSharp.Algo.Candles;
using StockSharp.Algo.Storages;
using StockSharp.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SampleQuik
{
    class StorageHistory
    {
        string storagePath;
        StorageRegistry storage;
        public Security security { get; set; }
        public TimeSpan timespan { get; set; }  //TimeSpan.FromMinutes(5)
        public IEnumerable<Candle> candles { get; private set; }
        public IEnumerable<Trade> trades { get; private set; }
        public StorageHistory()
        {
            XElement settings = XElement.Load("settings.xml");
            storagePath = @settings.Element("storage-path").Value;
            storage = new StorageRegistry() { DefaultDrive = new LocalMarketDataDrive { Path = storagePath } };
        }
        public void loadCandles()
        {
            //StorageFormats storageFormat = (StorageFormats)Enum.Parse(typeof(StorageFormats), settings.Element("storage-format").Value.ToUpper());
            /*  var security = new Security
              {
                  Id = "SIH7@FORTS"
                  PriceStep = 1m,
                  Decimals = 1,
                  VolumeStep = 1
              };*/
            var dataStorage = storage.GetCandleStorage(typeof(TimeFrameCandle), security, timespan, null, StorageFormats.Csv);
            candles = dataStorage.Load(new DateTime(2017, 1, 20, 0, 0, 0), DateTime.Today + TimeSpan.FromMinutes(1000));
        }
        public void loadTrades()
        {
            var dataStorage = storage.GetTradeStorage(security, null, StorageFormats.Csv);
           // trades=dataStorage.Load(new DateTime(2017, 2, 13, 0, 0, 0), DateTime.Today + TimeSpan.FromMinutes(1000));
            trades = dataStorage.Load(new DateTime(2017, 2, 9, 0, 0, 0), new DateTime(2017, 2, 10, 0, 0, 0));
        }
    }
}
