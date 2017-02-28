using StockSharp.Algo.Candles;
using StockSharp.BusinessEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleQuik
{
    class RichSecurity:Security
    {
        public IEnumerable<Candle> candles { get; set; }
        StorageHistory sh;
        public void loadCandles()
        {
            sh = new StorageHistory()
            {
                security = this,
                timespan = TimeSpan.FromMinutes(5)
            };
            sh.loadCandles();
            candles = sh.candles;
        }
    }
    class RichTrade : Trade
    {
        public decimal EndPrice { get; set; }
        public RichTrade(Trade tr)
        {
            Currency = tr.Currency;
            ExtensionInfo = tr.ExtensionInfo;
            Id = tr.Id;
            IsSystem = tr.IsSystem;
            IsUpTick = tr.IsUpTick;
            LocalTime = tr.LocalTime;
            OpenInterest=tr.OpenInterest;
            OrderDirection = tr.OrderDirection;
            Price = tr.Price;
            Security = tr.Security;
            Status = tr.Status;
            StringId = tr.StringId;
            Time = tr.Time;
            Volume = tr.Volume;
        }
    }
}
