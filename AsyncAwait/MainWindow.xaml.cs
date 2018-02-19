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
            var seconds = 2;
            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += "Start await 1:\n";
            var x = await SleepTaskRunAsync(seconds, "T1");
            textBox.Text += "Start await 2\n";
            var y = await SleepTaskRun2Async(seconds, "T2");
            textBox.Text += $"Ended all {x+y}s";
        }

        //task.Start()
        private async Task<int> SleepTaskRunAsync(int seconds, string name)
        {
            Func<int, string, int> sleep = (sec,nam) => {
                for (var i = 0; i < sec; i++)
                {
                    DisplayThreadInfo($"{nam} {i}s");
                    Thread.Sleep(1000 * (i + 1));
                }
                return seconds;
            };

            var task = new Task<int>(()=>sleep(seconds, name));
            task.Start();

            return await task;
        }

        //As SleepTaskRunAsync, but Run(lambda)
        private async Task<int> SleepTaskRun2Async(int seconds, string name)
        {
            return await Task.Run(() => {
                for (var i = 0; i < seconds; i++)
                {
                    DisplayThreadInfo($"{name} {i}s");
                    Thread.Sleep(1000 * (i + 1));
                }
                return seconds;
            });
        }

        //Synchronously, blocking gui, without Task
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var sleepSec = 5;
            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += "Start\n";
            textBox.Text += $"Blocking for {SyncSleep(sleepSec)}ms\n";
            textBox.Text += "End\n";
        }

        private int SyncSleep(int sec)
        {
            Func<int, int> sleep = seconds =>
            {
                var msec = seconds * 1000;
                DisplayThreadInfo("In sleep method");
                Thread.Sleep(msec);
                return msec;
            };
            return sleep(sec);
        }

        //asynchronous without async/await
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var taskSeconds = 5;
            var mainSeconds = 4;

            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += $"Start task. Wait for {taskSeconds}s\n";

            Task<int> task = SleepTaskStart(taskSeconds);
            textBox.Text += "Main start sleep 4s\n";
            Thread.Sleep(mainSeconds * 1000);

            if (task.Status != TaskStatus.Running && task.Status != TaskStatus.RanToCompletion)
                textBox.Text += "Zadania nie uruchomiono\n";

            //task.Result block gui (this thread), when this method waiting for finish task
            textBox.Text += $"Task result: {task.Result}\n"; 
            textBox.Text += "End\n";
        }

        private Task<int> SleepTaskStart(int seconds)
        {
            Func<object,int> sleep = sec =>
            {
                Thread.Sleep((int)sec * 1000);
                return (int)sec;
            };

            var task = new Task<int>(sleep, seconds);
            task.Start();
            return task;
        }

        //waiting for the end, each method
        private async void button3_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            var d1 = await SleepTaskRun2Async(2, "T1");
            var d2 = await SleepTaskRun2Async(3, "T2");
            var d3 = await SleepTaskRun2Async(2, "T3");

            textBox.Text += $"end {d1 + d2 + d3}s\n";
        }

        //run all methods and wait for result
        private async void button4_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            var s1 = SleepTaskRun2Async(3, "T1");
            var s2 = SleepTaskRun2Async(4, "T2");
            var s3 = SleepTaskRun2Async(3, "T3");

            textBox.Text += "Main sleep for 1s\n";
            Thread.Sleep(1000);

            var d1 = await s1;
            var d2 = await s2; //only one sec
            var d3 = await s3; //will be done with d1
            textBox.Text += $"end {d1}s {d2}s {d3}s\n";
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += "Before method with exception\n";
            try
            {
                SleepAndThrowException(2); //application will be closed by error
            }
            catch (Exception)
            {
                textBox.Text += "Caught exception!\n";
            }
        }

        private async void SleepAndThrowException(int seconds) //Task instead will not destroy app, but not display info about error
        {
            await Task.Run(() =>
            {
                Thread.Sleep(seconds * 1000);
                throw new Exception();
            });
        }

        private async void button6_Click(object sender, RoutedEventArgs e)
        {
            DisplayClear();
            DisplayThreadInfo("main");
            textBox.Text += "Before method with exception\n";
            try
            {
                await SleepAndThrowException2(2); 
            }
            catch (Exception)
            {
                textBox.Text += "Caught exception!\n"; //display this because Task return error by await
            }
        }

        private async Task SleepAndThrowException2(int seconds)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(seconds * 1000);
                throw new Exception();
            });
        }

        private void DisplayThreadInfo(string name)
        {
            int thread = Thread.CurrentThread.ManagedThreadId;
            int? task = Task.CurrentId;
            textBox.Dispatcher.Invoke(() => { textBox.Text += $"{name}; Thread:{thread}; Task:{task}; \n"; });
        }

        private void DisplayClear()
        {
            textBox.Dispatcher.Invoke(() => { textBox.Text = string.Empty; });
        }

    }
}
