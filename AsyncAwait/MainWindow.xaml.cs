using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
            int x = await Sleep3sAsync();
            textBox.Text += $"End {x}s";
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
            int x = await SleepWithoutCreateTask();
            textBox.Text += "End\n";
        }
        
        //2s+2s+2s
        private async void button3_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            await Sleep2sAsync("T1");
            await Sleep2sAsync("T2");
            await Sleep2sAsync("T3");

            textBox.Text += "end\n";
        }
        private async void button4_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            var s1 = Sleep2sAsync("T1");
            var s2 = Sleep2sAsync("T2");
            var s3 = Sleep2sAsync("T3");

            string d1 = await s1;
            string d2 = await s2;
            string d3 = await s3;
            textBox.Text += $"end {d1}, {d2}, {d3}\n";
        }

        private async void button5_Click(object sender, RoutedEventArgs e)
        {
            await ParallelSleepAsync();
        }

        private async Task<int> Sleep3sAsync()
        {
            return await Task.Run(
                () => {
                    for (int i = 0; i < 5; i++)
                    {
                        DisplayThreadInfo($"T1 {i}s");
                        Thread.Sleep(1000 * (i + 1));
                    }
                    return 5;
                });
        }

        private async Task ParallelSleepAsync()
        {
            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += "before TestIntAsync() \n";
            Task<int> result = Sleep3sAsync();
            textBox.Text += "after TestIntAsync() \n";
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                textBox.Text += "sleep 1s\n";
            }

            textBox.Text += "before 'await result' \n";
            var a = await result;
            textBox.Text += "after 'await result' \n";
            textBox.Text += a + "\n";
            textBox.Text += "after print result \n";
        }
        private async Task<int> SleepWithoutCreateTask() //block gui
        {
            Thread.Sleep(5000);
            return 5;
        }

        private void DisplayThreadInfo(string name)
        {
            int thread = Thread.CurrentThread.ManagedThreadId;
            int? task = Task.CurrentId;
            textBox.Dispatcher.Invoke(() => { textBox.Text += $"{name}; Thread:{thread}; Task:{task}; \n"; });
            //textBox.Dispatcher.InvokeAsync(() => { textBox.Text += $"{name}; Thread:{thread}; Task:{task}; \n"; });
        }
        private async Task<string> Sleep2sAsync(string name)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(2000);
                DisplayThreadInfo(name);
                return name;
            });
        }

        private void DisplayClear()
        {
            textBox.Dispatcher.Invoke(() => { textBox.Text = string.Empty; });
        }
    }
}
