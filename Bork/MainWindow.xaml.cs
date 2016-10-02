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

namespace Bork
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            //var lblpos = aruLabel.TransformToAncestor((Visual)aruLabel.Parent).Transform(new Point(0, 0));
            aruImage.setSource(Bork.Properties.Resources.DummyImg1);

            Task.Run(() =>
            {
                DateTime timeKeeper = DateTime.Now;
                while (true)
                {
                    var now = DateTime.Now;
                    double dt = (double)((now - timeKeeper).Milliseconds) / 1000;
                    Update(dt);
                    Thread.Sleep(1000/60);
                    timeKeeper = now;
                }
            });
        }

        protected void Update(double dt)
        {
            aruAnimation(dt);
        }

        double dScale = 0.1;
        private void aruAnimation(double dt)
        {
            System.Console.Out.WriteLine(dt);
            Dispatcher.Invoke(() =>
            {
                var rot = aruImage.getRotation();
                aruImage.setRotation(rot + 5);
                
                var scale = aruImage.getScale();
                scale += dScale;
                if (scale.X > 15 || scale.X < 1)
                    dScale *= -1;
                aruImage.setScale(scale);
            });
        }
    }
}
