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
            DisplayThreadInfo("main");
        }

        //async-await
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += "before await\n";
            int x = await TestIntAsync();
            textBox.Text += $"End {x}s";
        }

        private async Task<int> TestIntAsync()
        {
            return await Task.Run( 
                () =>{
                    DisplayThreadInfo("T1");
                    Thread.Sleep(5000);
                    return 5;
                });
        }
        
        //classical, with blocking gui
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += "Start blocking for 5s\n";
            Thread.Sleep(5000);
            textBox.Text += "End\n";
        }

        //blocking gui too
        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += "Start blocking for 5s\n";
            int x = await TestAsyncInt2();
            textBox.Text += "End\n";
        }

        private async Task<int> TestAsyncInt2() //block gui
        {//without create task
            Thread.Sleep(5000);
            return 5;
        }

        //2s+2s+2s
        private async void button3_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            await SleepAsync("T1");
            await SleepAsync("T2");
            await SleepAsync("T3");

            textBox.Text += "end\n";
        }
        private async void button5_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            var s1 = SleepAsync("T1");
            var s2 = SleepAsync("T2");
            var s3 = SleepAsync("T3");

            string d1 = await s1;
            string d2 = await s2;
            string d3 = await s3;
            textBox.Text += $"end {d1}, {d2}, {d3}\n";
        }

        private async Task<string> SleepAsync(string name)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(2000);
                DisplayThreadInfo(name);
                return name;
            });
        }
        
        private async void button4_Click(object sender, RoutedEventArgs e)
        {
            await TestAsync();
        }
        private async Task TestAsync()
        {
            textBox.Text = "before TestIntAsync() \n";
            Task<int> result =  TestIntAsync();
            textBox.Text += "after TestIntAsync() \n";
            textBox.Text += "sleep 3s\n";
            Thread.Sleep(3000);

            textBox.Text += "before 'await result' \n";
            var a = await result;
            textBox.Text += "after 'await result' \n";
            textBox.Text += a + "\n";
            textBox.Text += "after print result \n";
        }

        private void DisplayThreadInfo(string name)
        {
            int thread = Thread.CurrentThread.ManagedThreadId;
            int? task = Task.CurrentId;
            textBox.Dispatcher.Invoke(() => { textBox.Text += $"{name}; Thread:{thread}; Task:{task}; \n"; });
            //textBox.Dispatcher.InvokeAsync(() => { textBox.Text += $"{name}; Thread:{thread}; Task:{task}; \n"; });
        }

        private void DisplayClear()
        {
            textBox.Dispatcher.Invoke(() => { textBox.Text = string.Empty; });
        }
    }
}
