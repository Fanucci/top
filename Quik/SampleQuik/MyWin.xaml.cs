using StockSharp.Algo.Candles;
using StockSharp.BusinessEntities;
using System;
using System.Collections;
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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class MyWin : Window
    {
        TestSpreader TS;
        public MyWin()
        {
            InitializeComponent();
        }
        private void ClickClick(object sender, RoutedEventArgs e)
        {
            /*       RichSecurity RSec = new RichSecurity() { Id = "SIH7@FORTS" };
                   RSec.loadCandles();
       foreach (Candle c in RSec.candles) Console.WriteLine("Security:{0}  Time:{2}  ClosePrice:{1}",c.Security,c.ClosePrice, c.CloseTime);*/

            /*var sh = new StorageHistory() { security = new Security { Id = "SBER@TQBR" }, timespan = TimeSpan.FromMinutes(5) };
            sh.loadTrades();
            var trades=OrderAnalazyer.consolidateOrders(sh.trades);
            OrderAnalazyer.printHighestVolume(trades);*/

            TS = new TestSpreader();
            TS.start();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            TS.stop();
        }
    }
}
