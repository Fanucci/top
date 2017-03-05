namespace SampleQuik
{
    using Ecng.Collections;
    using StockSharp.Algo;
    using StockSharp.Algo.Candles;
    using StockSharp.Algo.Derivatives;
    using StockSharp.BusinessEntities;
    using StockSharp.Messages;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    class Signaler //current time. provides Forecasts
    {
        DateTime periodInFuture { get; set; }  //для какого периода делать прогноз. Переимоновать лучше)
        decimal minProbability { get; set; }
        Forecast val { get; set; }
        public void start() { }
        Security security;
        StorageHistory sh;
        public void addIndicator()
        {
            Indicator ind = new MovingAverage(sh);
            ind.doMath();
            Console.WriteLine(ind.value);
        }
        public void stop() { }
        public Signaler()
        {
            security = new Security { Id = "SIH7@FORTS" };
            sh = new StorageHistory() { security = this.security, timespan = TimeSpan.FromMinutes(5) };
            sh.loadCandles();
            //foreach (Candle c in sh.candles) Console.WriteLine("Security:{0}  Time:{2}  ClosePrice:{1}", c.Security, c.ClosePrice, c.CloseTime);
        }
    }

    class sssssss
    {
        event EventHandler<Quote> volumeSpotted;
        //RegisterMarketDepth //Начать получать котировки(стакан) по инструменту.Значение котировок можно получить через событие MarketDepthsChanged. 
        public sssssss(Security s)
        {
            MainWindow.Instance.Trader.RegisterMarketDepth(s);
            MainWindow.Instance.Trader.MarketDepthChanged += Trader_MarketDepthChanged;
            //volumeSpotted +=


        }
        void OnVolumeSpotted(Quote q)
        {
            if (volumeSpotted != null) volumeSpotted(this, q);
        }

        private void Trader_MarketDepthChanged(MarketDepth md)
        {
            int BigNumber = 1000;
            var asks = md.Asks;
            decimal total = 0;
            for (int i = 0; i < asks.Length; i++)
            {
                var ask = asks[i];
                var vol = ask.Volume;
                var price = ask.Price;
                if (vol > BigNumber)
                {
                    Console.WriteLine("{0} pcs. spotted at {1}. In total above-{2} pcs.", vol, price, total);

                }
                total += vol;
                OnVolumeSpotted(ask);
            }
        }

    }

    class StockFuturesMarketMaking
    {
        Security baseSec;
        Security futSec;
        //  Portfolio stockPortfolio = portfolios.FirstOrDefault(p => p.Name == "131776");
        //  Portfolio futuresPortfolio = portfolios.FirstOrDefault(p => p.Name == "4102KEU");
        Portfolio stockPortfolio = new Portfolio() { Name = "131776" };
        Portfolio futuresPortfolio = new Portfolio() { Name = "4102KEU" };
        decimal previousBestAsk;
        decimal previousBestBid;
        Order buyOrder;
        Order sellOrder;
        decimal buyVolumeLeft = 10;
        decimal sellVolumeLeft = 10;
        bool buyIsFilled = false;
        bool sellIsFilled = false;
        bool futuresIsSold = false;
        bool futuresIsBought = false;

        void start()
        {
            MainWindow.Instance.Trader.MarketDepthChanged += Trader_MarketDepthChanged;
            MainWindow.Instance.Trader.OrderChanged += Trader_OrderChanged;
        }

        private void Trader_OrderChanged(Order order)
        {
            var matched = order.GetMatchedVolume(MainWindow.Instance.Trader, true);
            if ((order.Direction == Sides.Buy) && (matched == buyVolumeLeft))
            {
                buyIsFilled = true;
                var ord = new Order
                {
                    Portfolio = futuresPortfolio,
                    Price = futSec.ShrinkPrice(futSec.BestAsk.Price),
                    Security = futSec,
                    Volume = 1,
                    Direction = Sides.Sell,
                };
                MainWindow.Instance.Trader.RegisterOrder(ord);
            }
            else { buyVolumeLeft -= matched; sellVolumeLeft += matched; }

            if ((order.Direction == Sides.Sell) && (matched == sellVolumeLeft))
            {
                sellIsFilled = false;
                var ord = new Order
                {
                    Portfolio = futuresPortfolio,
                    Price = futSec.ShrinkPrice(futSec.BestBid.Price),
                    Security = futSec,
                    Volume = 1,
                    Direction = Sides.Buy,
                };
                MainWindow.Instance.Trader.RegisterOrder(ord);
            }
            else { sellVolumeLeft -= matched; buyVolumeLeft += matched; }
        }

        void stop()
        {

        }

        void registerSec(Security s)
        {
            MainWindow.Instance.Trader.RegisterMarketDepth(s);

        }
        private void Trader_MarketDepthChanged(MarketDepth md)
        {
            decimal minBaseSpread = 6;
            decimal minFuturesSpread = 2;
            //check position---
            if (md.Security == baseSec)  //security is that we need
            {
                //exclude my orders--
                if ((baseSec.BestPair.SpreadPrice >= minBaseSpread) && (futSec.BestPair.SpreadPrice >= minFuturesSpread)) //both prices are in nice range
                {
                    if ((md.BestBid.Price != previousBestBid) && (buyVolumeLeft != 0))  //best bid is changed && order is not filled
                    {
                        MainWindow.Instance.Trader.CancelOrders(null, null, null, null, baseSec);
                        buyOrder = new Order
                        {
                            Portfolio = stockPortfolio,
                            Price = baseSec.ShrinkPrice(md.BestBid.Price),
                            Security = baseSec,
                            Volume = buyVolumeLeft,
                            Direction = Sides.Buy,
                        };
                        MainWindow.Instance.Trader.RegisterOrder(buyOrder);
                        Console.WriteLine("Заявка {0} зарегистрирована", buyOrder.Id);
                    }
                    if ((md.BestAsk.Price != previousBestAsk) && (sellVolumeLeft != 0)) //best ask is changed && order is not filled
                    {
                        MainWindow.Instance.Trader.CancelOrders(null, null, null, null, baseSec);
                        sellOrder = new Order
                        {
                            Portfolio = stockPortfolio,
                            Price = baseSec.ShrinkPrice(md.BestAsk.Price),
                            Security = baseSec,
                            Volume = sellVolumeLeft,
                            Direction = Sides.Sell,
                        };
                        MainWindow.Instance.Trader.RegisterOrder(sellOrder);
                        Console.WriteLine("Заявка {0} зарегистрирована", sellOrder.Id);
                    }
                }
                else { MainWindow.Instance.Trader.CancelOrders(null, null, null, null, baseSec); }
            }

        }

    }

    class OrderAnalazyer
    {
        public static List<RichTrade> consolidateOrders(IEnumerable<Trade> trades)
        {
            Trade previousTrade = new Trade();
            decimal volume = 0;
            decimal initPrice = 0;
            List<RichTrade> volumes = new List<RichTrade>();
            foreach (Trade trade in trades)
            {
                if ((trade.Time == previousTrade.Time) && (trade.OrderDirection == previousTrade.OrderDirection)) volume += trade.Volume;
                else
                {
                    RichTrade rt = new RichTrade(previousTrade);

                    rt.Volume = volume;
                    rt.EndPrice = rt.Price;
                    rt.Price = initPrice;
                    volumes.Add(rt);
                    volume = trade.Volume;
                    initPrice = trade.Price;
                }
                previousTrade = trade;
            }
            return volumes;
        }
        public static void printHighestVolume(List<RichTrade> volumes)
        {
            Console.WriteLine("There are {0} entries. Printing highest volume:", volumes.Count);
            List<RichTrade> SortedList = volumes.OrderByDescending(trade => trade.Volume).Take(50).ToList();
            foreach (RichTrade dec in SortedList) Console.WriteLine("{0} of {4}. Direction- {1}. Prices: {2}-{3}. Time:{5}",
                dec.Volume, dec.OrderDirection, dec.Price, dec.EndPrice, dec.Security, dec.Time);

        }
        public static void volumePeriodDependence(TimeSpan period, int numOfQuants)
        {

        }
    }

    class Spreader
    {
        Security sec;
        decimal minSpread;
        decimal maxPosition;
        decimal currentPos;

        Order sellOrder;
        Order buyOrder;
        void start()
        {
            MainWindow.Instance.Trader.MarketDepthChanged += Trader_MarketDepthChanged;
            MainWindow.Instance.Trader.OrderChanged += Trader_OrderChanged;
        }

        void stop()
        {
            MainWindow.Instance.Trader.MarketDepthChanged -= Trader_MarketDepthChanged;
            MainWindow.Instance.Trader.OrderChanged -= Trader_OrderChanged;
            MainWindow.Instance.Trader.CancelOrder(sellOrder);
            MainWindow.Instance.Trader.CancelOrder(buyOrder);
        }

        private void Trader_OrderChanged(Order order)
        {
            var matched = order.GetMatchedVolume(MainWindow.Instance.Trader, true);
            currentPos = order.Direction == Sides.Buy ? currentPos + matched : currentPos - matched;
            var oldOrder = order.Direction == Sides.Buy ? sellOrder : buyOrder;
            var newOrder = oldOrder.ReRegisterClone();
            newOrder.Volume = maxPosition + Math.Abs(currentPos);
            MainWindow.Instance.Trader.ReRegisterOrder(oldOrder, newOrder);
            if (order.Direction == Sides.Buy) sellOrder = newOrder; else buyOrder = newOrder;
        }

        private void Trader_MarketDepthChanged(MarketDepth obj)
        {
        }

    }

    class OptionStrikeHedger
    {
        decimal strike;
        Security sec;
        Trade prevTrade;
        decimal volume = 1;
        bool hedgeIfHigher = true;
        bool hedged;
        Portfolio portf = new Portfolio() { Name = "131776" };
        void start()
        {
            MainWindow.Instance.Trader.NewTrade += Trader_NewTrade;
        }


        void stop()
        {

        }

        private void Trader_NewTrade(Trade trade)
        {
            if (trade.Security == sec)
            {
                if (!hedged && hedgeIfHigher && (prevTrade.Price < trade.Price) && (trade.Price >= strike))
                {
                    var ord = new Order
                    {
                        Portfolio = portf,
                        Price = sec.MaxPrice.GetValueOrDefault(),
                        Security = sec,
                        Volume = 1,
                        Direction = Sides.Buy,
                    };
                    MainWindow.Instance.Trader.RegisterOrder(ord);

                    System.Threading.Thread.Sleep(200);

                    if (ord.IsMatched())
                    {
                        Console.WriteLine("Куплено {0} по цене {1}. Заявка номер {2}", ord.Security, ord.Price, ord.Id);
                        hedged = true;
                    }
                }
                if (!hedged && hedgeIfHigher && (prevTrade.Price < trade.Price) && (trade.Price >= strike)) ;
            }
        }
    }

    class CustomSpreadHolder //очеьн аккуратно пока
    {
        static public Portfolio futuresPortfolio = new Portfolio() { Name = "4102KEU" };
        Security futures = new Security() { Id = "RIH7@FORTS" };
        decimal curDelta = 0;
        public decimal maxDelta = 1;
        decimal minSpread = 30;
        decimal futuresBought = 0;
        decimal percentDeltaToHedge = 0.9M;
        List<SpreadHolder> options = new List<SpreadHolder>()
        {
            new SpreadHolder { sec = new Security { Id = "RI117500BC7A@FORTS" } },
            new SpreadHolder { sec = new Security { Id = "RI115000BC7A@FORTS" } },
            new SpreadHolder { sec = new Security { Id = "RI112500BC7A@FORTS" } },
        };
        void start()
        {
            placeOrders();
            MainWindow.Instance.Trader.MarketDepthChanged += Trader_MarketDepthChanged;
            MainWindow.Instance.Trader.OrderChanged += Trader_OrderChanged;
        }

        private void placeOrders()
        {
            foreach (SpreadHolder sh in options)
            {
                sh.quote();
            }
        }
        void stop()
        {
            MainWindow.Instance.Trader.MarketDepthChanged -= Trader_MarketDepthChanged;
            MainWindow.Instance.Trader.OrderChanged -= Trader_OrderChanged;
            MainWindow.Instance.Trader.CancelOrders();
        }

        private void Trader_OrderChanged(Order order)
        {
            bool contains = options.Any(p => p.sec.Id == order.Security.Id);
            if (contains)
            {
                var matched = order.GetMatchedVolume(MainWindow.Instance.Trader, true);
                curDelta += order.Security.Delta.GetValueOrDefault() * matched;
                requote();
            }
        }

        void requote()
        {
            foreach (SpreadHolder sh in options)
            {
                var volume = curDelta - futuresBought / sh.sec.Delta.GetValueOrDefault();
                if (curDelta - futuresBought < 0.9M)
                {
                    sh.changeVolume(sh.buyOrder, volume);
                }

            }

        }
        private void Trader_MarketDepthChanged(MarketDepth obj)
        {
        }
    }

    class SpreadHolder : CustomSpreadHolder
    {
        public Security sec { get; set; }
        public Order buyOrder;
        Order sellOrder;
        public SpreadHolder()
        {
            buyOrder = new Order { Portfolio = futuresPortfolio, Security = sec, Direction = Sides.Buy };
            sellOrder = new Order { Portfolio = futuresPortfolio, Security = sec, Direction = Sides.Sell };
        }
        public void quote()
        {
            buyOrder.Price = sec.ShrinkPrice(sec.BestAsk.Price);
            buyOrder.Volume = (maxDelta / sec.Delta.GetValueOrDefault());
            MainWindow.Instance.Trader.RegisterOrder(buyOrder);
        }

        public void cancelQuotation(Order order)
        {
            MainWindow.Instance.Trader.CancelOrder(order);
        }
        public void changePrice(Order order, decimal price)
        {
            var newOrder = order.ReRegisterClone();
            newOrder.Price = price;
            MainWindow.Instance.Trader.ReRegisterOrder(order, newOrder);
            if (order.Direction == Sides.Buy) buyOrder = newOrder; else sellOrder = newOrder;
        }
        public void changeVolume(Order order, decimal volume)
        {
            var newOrder = order.ReRegisterClone();
            newOrder.Volume = volume;
            MainWindow.Instance.Trader.ReRegisterOrder(order, newOrder);
            if (order.Direction == Sides.Buy) buyOrder = newOrder; else sellOrder = newOrder;
        }
    }

    class TestSpreader
    {
        Timer timer;
        Portfolio futuresPortfolio;// = new Portfolio() { Name = "4102KEU" };
        Security futures;// = new Security() { Id = "RIH7@FORTS" };
        Security option;// = new Security() { Id = "RI117500BC7@FORTS" }; //RI117500BC7A
        BlackScholes BS;
        decimal futuresSold = 0;
        decimal optionsBought = 0;
        Order buyOrder;
        Order sellOrder;
        decimal buyPriceDiff = 0;
        decimal buyPreviousMatched;
        decimal sellPreviousMatched;


        public TestSpreader()
        {
            timer = new Timer() { Interval = 1000 };
            timer.Elapsed += Timer_Elapsed;
            option = MainWindow.Instance.Trader.Securities.FirstOrDefault(o => o.Id == "RI112500BC7@FORTS");
            futures = MainWindow.Instance.Trader.Securities.FirstOrDefault(o => o.Id == "RIH7@FORTS");
            futuresPortfolio = MainWindow.Instance.Trader.Portfolios.FirstOrDefault(o => o.Name == "SPBFUTJR3L9");
            MainWindow.Instance.Trader.RegisterSecurity(option);
            MainWindow.Instance.Trader.RegisterSecurity(futures);
            BS = new BlackScholes(option, MainWindow.Instance.Trader, MainWindow.Instance.Trader);
            buyOrder = new Order { Portfolio = futuresPortfolio, Security = option, Direction = Sides.Buy };
            sellOrder = new Order { Portfolio = futuresPortfolio, Security = option, Direction = Sides.Sell };
            System.Threading.Thread.Sleep(1000);
        }

        public void start()
        {
            MainWindow.Instance.Trader.MarketDepthChanged += Trader_MarketDepthChanged;
            MainWindow.Instance.Trader.OrderChanged += Trader_OrderChanged;
            timer.Start();
        }

        public void stop()
        {
            MainWindow.Instance.Trader.MarketDepthChanged -= Trader_MarketDepthChanged;
            MainWindow.Instance.Trader.OrderChanged -= Trader_OrderChanged;
            MainWindow.Instance.Trader.CancelOrders();
            timer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            requoteBids();
            requoteAsks();
            hedge();
        }

        private void Trader_MarketDepthChanged(MarketDepth obj)
        {
            requoteBids();
            requoteAsks();
        }

        decimal checkDelta()
        {
            var delta = BS.Delta(DateTimeOffset.Now).GetValueOrDefault();
            return delta;
        }

        bool activeOrPending(Order ord)
        {
            if (ord.State == OrderStates.Active || ord.State == OrderStates.Pending) return true;
            return false;
        }

        private void Trader_OrderChanged(Order obj)
        {
            if (obj.Security.Id == option.Id && (obj.State == OrderStates.Active || obj.State == OrderStates.Done))
            {
                var matched = obj.GetMatchedVolume(MainWindow.Instance.Trader, true);
                if (obj.Direction == Sides.Buy)
                {
                    var newMatches = matched - buyPreviousMatched;
                    buyPriceDiff = (buyPriceDiff * optionsBought + (obj.Price - option.TheorPrice.GetValueOrDefault()) * newMatches) / (optionsBought + newMatches);
                    optionsBought += newMatches;
                    buyPreviousMatched = matched;
                }
                else
                {
                    var newMatches = matched - sellPreviousMatched;
                    optionsBought -= newMatches;
                    sellPreviousMatched = matched;
                }
                Console.WriteLine("OptionsBought: {0}. buyPriceDiff: {1}", optionsBought, buyPriceDiff);
                hedge();
            }
        }

        void hedge()
        {
            var order = new Order()
            {
                Portfolio = futuresPortfolio,
                Security = futures,
                Volume = 1
            };
            if ((checkDelta() * optionsBought > 0.55M) && futuresSold == 0)
            {
                order.Direction = Sides.Sell;
                order.Price = futures.MinPrice.GetValueOrDefault();
                MainWindow.Instance.Trader.RegisterOrder(order);
                futuresSold = 1;
            }
            if ((checkDelta() * optionsBought < 0.45M) && futuresSold == 1)
            {
                order.Direction = Sides.Buy;
                order.Price = futures.MaxPrice.GetValueOrDefault();
                MainWindow.Instance.Trader.RegisterOrder(order);
                futuresSold = 0;
            }
        }

        void requoteBids()
        {
            var delta = checkDelta();
            var volume = Decimal.Floor((1.1M - delta * optionsBought) / delta);
            if (option.BestAsk.Price > option.TheorPrice.GetValueOrDefault()) placeOrder(ref buyOrder, 0);
            else placeOrder(ref buyOrder, volume);
        }

        void requoteAsks()
        {
            var delta = checkDelta();
            //var volume = Decimal.Floor((delta * optionsBought - 0.55M) / delta);
            placeOrder(ref sellOrder, optionsBought);
        }

        void placeOrder(ref Order ord, decimal volume)
        {
            var price = ord.Direction == Sides.Buy ? getBidPrice() : getAskPrice();
            if (activeOrPending(ord) && volume == ord.Balance && price == ord.Price) return;
            if (activeOrPending(ord))
            {
                MainWindow.Instance.Trader.CancelOrder(ord);
                buyPreviousMatched = 0;
                sellPreviousMatched = 0;
            }
            if (volume <= 0) return;
            ord.Price = price;
            ord.Volume = volume;
            var newOrder = ord.ReRegisterClone();
            MainWindow.Instance.Trader.RegisterOrder(newOrder);
            ord = newOrder;
            Console.WriteLine("{0} order: Price - {1}, Volume - {2}, Delta - {3}", ord.Direction, ord.Price, ord.Volume, Decimal.Round(checkDelta(), 4));
        }

        /*     bool isMyOrderInQuote(Quote q)
             {
                 if (q.OrderDirection==Sides.Sell) return (sellOrder.State == OrderStates.Active) && (sellOrder.Price == q.Price) && (sellOrder.Volume > 0);
                 if (q.OrderDirection == Sides.Buy) return (buyOrder.State == OrderStates.Active) && (buyOrder.Price == q.Price) && (buyOrder.Volume > 0);
                 return false;
             }*/
        decimal getMiddle()
        {
            decimal bestAsk;
            decimal bestBid;
            var marketdepth = MainWindow.Instance.Trader.GetMarketDepth(option);

            if (marketdepth.Bids.First().Volume < 2) bestBid = marketdepth.Bids.FirstOrDefault().Price;
            else bestBid = marketdepth.Bids.Skip(1).FirstOrDefault().Price;
            if (marketdepth.Asks.First().Volume < 2) bestAsk = marketdepth.Asks.FirstOrDefault().Price;
            else bestAsk = marketdepth.Asks.Skip(1).FirstOrDefault().Price;

            if (bestAsk == 0 || bestBid == 0) return option.TheorPrice.GetValueOrDefault();
            return (bestAsk + bestBid) / 2;
        }

        decimal getBidPrice()
        {
            //check if quote is mine!
            var urgent = Math.Abs(checkDelta() * optionsBought - futuresSold) > 0.1M ? true : false;
            var middle = option.ShrinkPrice(getMiddle(), ShrinkRules.Less);
            decimal price = urgent ? middle - 10 : middle - 20;
            return price;
        }
        decimal getAskPrice()
        {
            var urgent = Math.Abs(checkDelta() * optionsBought - futuresSold) > 0.1M ? true : false;
            var middle = option.ShrinkPrice(getMiddle(), ShrinkRules.More);
            decimal price = urgent ? middle + 10 : middle + 20;
            var minimalPrice = option.TheorPrice.GetValueOrDefault() + buyPriceDiff + 20;
            if (price < minimalPrice) price = option.ShrinkPrice(minimalPrice, ShrinkRules.More);  //cant be not profitable
            return price;
        }
    }


    abstract class Indicator //history+current time
    {
        string id { get; set; }
        string name { get; set; }
        decimal weight { get; set; }
        public decimal value;
        StorageHistory sh;
        protected IEnumerable<Candle> candles;
        private Object extremumPoints;
        public Indicator(StorageHistory s)
        {
            sh = s;
            candles = s.candles;
        }
        public abstract void doMath();
    }

    class MovingAverage : Indicator
    {
        int timePeriod { get; set; }  //за какой период раасчитыввать(22 свечи)
        decimal previousValue;
        public MovingAverage(StorageHistory s) : base(s) { }
        public override void doMath()
        {
            previousValue = 0;
            timePeriod = 22;
            foreach (Candle c in candles)
            {
                decimal multiplier = 2 / (timePeriod + 1);
                value = (c.ClosePrice - previousValue) * multiplier + previousValue;
                previousValue = value;
            }
        }
    }

    class Forecast
    {
        DateTime point { get; set; }
        decimal value { get; set; }
        int probability { get; set; }  //лучше сделать графиком например ln x, а не int
    }

}
