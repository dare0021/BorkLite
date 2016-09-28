using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestProject
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                DateTime timeKeeper = DateTime.Now;
                while (true)
                {
                    var now = DateTime.Now;
                    double dt = (double)((now - timeKeeper).Milliseconds) / 1000;
                    someTask(dt);
                    Thread.Sleep(1000/60);
                    timeKeeper = now;
                }
            });
        }

        private void someTask(double dt)
        {
            System.Console.Out.WriteLine(dt);
            this.Dispatcher.Invoke(() =>
            {
                var rot = aruImage.RenderTransform as RotateTransform;
                rot.Angle += dt * 5;
                aruLabel.Content = rot.Angle;
            });
        }
    }
}
