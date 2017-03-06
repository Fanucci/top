using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SampleQuik
{
    /// <summary>
    /// Логика взаимодействия для ConsoleLikeWindow.xaml
    /// </summary>
    public partial class ConsoleLikeWindow : Window
    {
        public ConsoleLikeWindow()
        {
            InitializeComponent();
            Instance = this;
        }
        public static ConsoleLikeWindow Instance { get; private set; }
        delegate void printCallback(string text);
        public void print(string text)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
   DispatcherPriority.Normal,
   (ThreadStart)delegate
   {
       string newText = text + "\r\n";
       TBox.Text = String.Concat(TBox.Text, newText);
   });
        }

        public void print(string text, object arg0)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
DispatcherPriority.Normal,
(ThreadStart)delegate
{
    string newText = string.Format(text, arg0) + "\r\n";
    TBox.Text = String.Concat(TBox.Text, newText);
});
        }

        public void print(string text, object arg0, object arg1)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
DispatcherPriority.Normal,
(ThreadStart)delegate
{
    string newText = string.Format(text, arg0, arg1) + "\r\n";
    TBox.Text = String.Concat(TBox.Text, newText);
});
        }

        public void print(string text, object arg0, object arg1, object arg2)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
DispatcherPriority.Normal,
(ThreadStart)delegate
{
    string newText = string.Format(text, arg0, arg1, arg2) + "\r\n";
    TBox.Text = String.Concat(TBox.Text, newText);
});
        }
        public void print(string text, object arg0, object arg1, object arg2, object arg3)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
DispatcherPriority.Normal,
(ThreadStart)delegate
{
    string newText = string.Format(text, arg0, arg1, arg2, arg3) + "\r\n";
    TBox.Text = String.Concat(TBox.Text, newText);
});
        }
        public void print(string text, object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
DispatcherPriority.Normal,
(ThreadStart)delegate
{
    string newText = string.Format(text, arg0, arg1, arg2, arg3, arg4) + "\r\n";
    TBox.Text = String.Concat(TBox.Text, newText);
});
        }
        public void print(string text, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
DispatcherPriority.Normal,
(ThreadStart)delegate
{
    string newText = string.Format(text, arg0, arg1, arg2, arg3, arg4, arg5) + "\r\n";
    TBox.Text = String.Concat(TBox.Text, newText);
});
        }
    }
}
