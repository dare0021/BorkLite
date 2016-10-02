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
using Bork.Controls;

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

            var rng = new Random();
            for (int i=0; i<1000; i++)
            {
                var iter = new RichImage();
                iter.setSource(Bork.Properties.Resources.DummyImg1);
                iter.setPosition(rng.Next((int)(Width / -2), (int)(Width/2)),
                                 rng.Next((int)(Height / -2), (int)(Height/2)));
                iter.setSize(iter.Source.Width / 10, iter.Source.Height / 10);
                grid.Children.Add(iter);
            }

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
                foreach (var ctrl in grid.Children)
                {
                    if (!(ctrl is RichImage))
                        continue;
                    var ri = (RichImage)ctrl;
                    var rot = ri.getRotation();
                    ri.setRotation(rot + 5);

                    var aruScale = aruImage.getScale().X;
                    if (aruScale > 5 || aruScale < 1)
                        dScale *= -1;
                    var scale = ri.getScale();
                    scale += dScale;
                    ri.setScale(scale);
                }
            });
        }
    }
}
