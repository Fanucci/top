using Ecng.Collections;
using Ecng.Common;
using Ecng.Xaml;
using StockSharp.Algo;
using StockSharp.Algo.Candles;
using StockSharp.BusinessEntities;
using StockSharp.Quik;
using StockSharp.Xaml.Charting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SampleQuik
{
    /// <summary>
    /// Логика взаимодействия для ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        private readonly Dictionary<CandleSeries, ChartWindow> _chartWindows = new Dictionary<CandleSeries, ChartWindow>();
        private CandleManager _candleManager;
        private QuikTrader _trader =  MainWindow.Instance.Trader;
        public ChartWindow()
        {
            InitializeComponent();
          /*  Security.SecurityProvider = new FilterableSecurityProvider(_trader);
            _candleManager = new CandleManager(_trader);
            _candleManager.Processing += DrawCandle;*/
        }

    /*    private void DrawCandle(CandleSeries series, Candle candle)
        {
            var wnd = _chartWindows.TryGetValue(series);

            if (wnd != null)
                wnd.Chart.Draw((ChartCandleElement)wnd.Chart.Areas[0].Elements[0], candle);
        }
        private Security SelectedSecurity => Security.SelectedSecurity;
        private void OnSecuritySelected()
        {
            ShowChart.IsEnabled = SelectedSecurity != null; //button
        }

        private void ShowChartClick(object sender, RoutedEventArgs e)
        {
            var security = SelectedSecurity;

            TimeSpan _timeFrame = TimeSpan.FromMinutes(5);
            var series = new CandleSeries(typeof(TimeFrameCandle), security, _timeFrame);
            //  var series = new CandleSeries(CandlesSettings.Settings.CandleType, security, CandlesSettings.Settings.Arg);

            _chartWindows.SafeAdd(series, key =>
            {
                var wnd = new ChartWindow
                {
                    Title = "{0} {1} {2}".Put(security.Code, series.CandleType.Name, series.Arg)
                };

                wnd.MakeHideable();

                var area = new ChartArea();
                wnd.Chart.Areas.Add(area);

                var candlesElem = new ChartCandleElement();
                area.Elements.Add(candlesElem);

                return wnd;
            }).Show();

            _candleManager.Start(series);
        }*/
    }
}
