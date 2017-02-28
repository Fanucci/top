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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ClickClick(object sender, RoutedEventArgs e)
        {
            EmitentEventa me = new EmitentEventa();
            me.SomeEvent += (object source,EventArgs arg)=>Console.WriteLine("X==15");
            me.MyMethod();
        }
    }

    class EmitentEventa
    {
        public event EventHandler<EventArgs>  SomeEvent;
        void OnSomeEvent()
        {
            if (SomeEvent != null) SomeEvent(this,EventArgs.Empty);
        }
        public void MyMethod()
        {
            int X;
            for (X = 10; X <= 20; X++)
                if (X == 15) OnSomeEvent();
        }

    }
}
