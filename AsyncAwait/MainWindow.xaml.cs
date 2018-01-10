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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncAwait
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            int thread = Thread.CurrentThread.ManagedThreadId;
            int? task = Task.CurrentId;
            textBox.Text = $"Start;Th{thread};Ta{task} \n";
        }

        //async-await
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            int x = await TestIntAsync();
            MessageBox.Show(x.ToString());
        }

        private async Task<int> TestIntAsync()
        {
            return await Task.Run( 
                () =>{
                    Thread.Sleep(5000);
                    return 5;
                });
        }
        
        //classical, with blocking gui
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(5000);
            int x = 5;
            MessageBox.Show(x.ToString());
        }

        //blocking gui too
        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            int x = await TestAsyncInt2();
            MessageBox.Show(x.ToString());
        }

        private async Task<int> TestAsyncInt2() //block gui
        {
            Thread.Sleep(5000);
            return 5;
        }

        //2s+2s+2s
        private async void button3_Click(object sender, RoutedEventArgs e)
        {
            int thread = Thread.CurrentThread.ManagedThreadId;
            int? task = Task.CurrentId;
            textBox.Text = $"T0;Th{thread};Ta{task} \n";
            await Task.Run(() =>
            { 
                Thread.Sleep(2000);
                thread = Thread.CurrentThread.ManagedThreadId;
                task = Task.CurrentId;
                textBox.Dispatcher.InvokeAsync(() => { textBox.Text += $"T1;Th{thread};Ta{task} \n"; });
            });
            
            await Task.Run(() =>
            {
                Thread.Sleep(2000);
                thread = Thread.CurrentThread.ManagedThreadId;
                task = Task.CurrentId;
                textBox.Dispatcher.InvokeAsync(() => { textBox.Text += $"T2;Th{thread};Ta{task} \n"; });
            });
            
            await Task.Run(() =>
            {
                Thread.Sleep(2000);
                thread = Thread.CurrentThread.ManagedThreadId;
                task = Task.CurrentId;
                textBox.Dispatcher.InvokeAsync(() => { textBox.Text += $"T3;Th{thread};Ta{task} \n"; });
            });
            
        }

        //
        private async void button4_Click(object sender, RoutedEventArgs e)
        {
            await testAsync();
        }
        private async Task testAsync()
        {
            textBox.Text = "start\n";
            int res = await TestIntAsync(); //waiting for end
            textBox.Text += "before await\n";
            textBox.Text += res.ToString();
        }
    }
}
