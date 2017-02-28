﻿#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: SampleTransaq.SampleTransaqPublic
File: ChartWindow.xaml.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace SampleTransaq
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Media;

	using StockSharp.Algo.Candles;
	using StockSharp.Transaq;
	using StockSharp.Xaml.Charting;

	partial class ChartWindow
	{
		private readonly TransaqTrader _trader;
		private readonly CandleSeries _candleSeries;
		private readonly ChartCandleElement _candleElem;

		public ChartWindow(CandleSeries candleSeries)
		{
			InitializeComponent();

			if (candleSeries == null)
				throw new ArgumentNullException(nameof(candleSeries));

			_candleSeries = candleSeries;
			_trader = MainWindow.Instance.Trader;

			Chart.ChartTheme = "ExpressionDark";

			var area = new ChartArea();
			Chart.Areas.Add(area);

			_candleElem = new ChartCandleElement
			{
				AntiAliasing = false, 
				UpFillColor = Colors.White,
				UpBorderColor = Colors.Black,
				DownFillColor = Colors.Black,
				DownBorderColor = Colors.Black,
			};

			area.Elements.Add(_candleElem);

			_trader.NewCandles += ProcessNewCandles;
			_trader.SubscribeCandles(_candleSeries, DateTime.Today - TimeSpan.FromTicks(((TimeSpan)candleSeries.Arg).Ticks * 10000), DateTimeOffset.MaxValue);
		}

		private void ProcessNewCandles(CandleSeries series, IEnumerable<Candle> candles)
		{
			if (series != _candleSeries)
				return;

			foreach (var timeFrameCandle in candles)
			{
				Chart.Draw(_candleElem, timeFrameCandle);
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			_trader.NewCandles -= ProcessNewCandles;
			base.OnClosing(e);
		}
	}
}