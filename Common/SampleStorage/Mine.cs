
namespace Synapse.Cooking.MarketDataRecorder
{

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Linq;
    using StockSharp.BusinessEntities;
    using StockSharp.Quik;
    using StockSharp.Algo.Storages;
    using StockSharp.Algo.Candles;
    using StockSharp.Algo.Storages.Csv;
    using StockSharp.Messages;

    class History
    {
        public void getfromstore()
        {
            var storage = new StorageRegistry() { DefaultDrive = new LocalMarketDataDrive { Path = @"C:\Personal Soft\StockSharp\Hydra\MyData\" } };
            var security = new Security
            {
                Id = "SIH7@FORTS",
                PriceStep = 1m,
                Decimals = 1,
                VolumeStep = 1
            };
            var candles = storage.GetCandleStorage(typeof(TimeFrameCandle), security, TimeSpan.FromMinutes(5), null, StorageFormats.Csv);
            var loadedCandles = candles.Load(new DateTime(2017, 1, 18, 0, 0, 0), DateTime.Today + TimeSpan.FromMinutes(1000));
            Console.WriteLine(loadedCandles.Count());
            foreach (var c in loadedCandles)
            {
                Console.WriteLine("Сделка {0} : {1}", c.ClosePrice, c.OpenPrice);
            }


                Console.ReadLine();
        }
        static void Main(string[] args)
        {
            History h = new History();
            h.getfromstore();
        }
    }
}