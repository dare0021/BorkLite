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
using Bork.Helpers;

namespace Bork
{
    public partial class MainWindow : Window
    {
        private bool mouseDown = false;
        GameDisplayObject beingDragged;

        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            Height = SystemParameters.FullPrimaryScreenHeight;
            Width = SystemParameters.FullPrimaryScreenWidth;



            aruImage.setSource(Bork.Properties.Resources.DummyImg1);

            var rng = new Random();
            Vec2[] locations = {new Vec2(300,300),
                                new Vec2(300,-300),
                                new Vec2(-300,-300),
                                new Vec2(-300,300) };
            for (int i=0; i<4; i++)
            {
                var iter = new RichImage();
                iter.setSource("videos/deathAnimationDummy/" + i + ".png");
                iter.setPosition(locations[i]);
                iter.setSize(32, 32);
                grid.Children.Add(iter);
            }
            var iter2 = new GameDisplayObject();
            iter2.setSource(Bork.Properties.Resources.DummyImg2);
            iter2.setPosition(rng.Next((int)(Width / -2), (int)(Width / 2)),
                             rng.Next((int)(Height / -2), (int)(Height / 2)));
            iter2.setSize(iter2.Source.Width / 10, iter2.Source.Height / 10);
            grid.Children.Add(iter2);
            iter2.Speed = 5;
            iter2.RotationSpeed = 5;

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
            Dispatcher.Invoke(() =>
            {
                if (Common.displayBoundingBox)
                {
                    canvasClearEveryUpdate.Children.Clear();
                }
                foreach (var ctrl in grid.Children)
                {
                    if (!(ctrl is GameDisplayObject))
                        continue;

                    if (Common.displayBoundingBox)
                    {
                        var box = ((GameDisplayObject)ctrl).getBoundingBoxGeometry(false);
                        var p = new Path();
                        p.Data = box;
                        p.Fill = Brushes.Transparent;
                        p.Stroke = Brushes.Red;
                        p.StrokeThickness = 1;
                        p.Name = "boundingBox" + ctrl.GetHashCode();
                        canvasClearEveryUpdate.Children.Add(p);
                    }
                    
                    ((GameDisplayObject)ctrl).Update(dt);
                }
            });
            //aruAnimation(dt);
        }

        public Vec2 screenToWindow(Vec2 v)
        {
            return screenToWindow(v.X, v.Y);
        }
        public Vec2 screenToWindow(double x, double y)
        {
            return new Vec2(x - Width / 2, y - Height / 2);
        }

        public RichImage displaySingleUseVideo(String path, Vec2 position, Vec2 size, double rotation)
        {
            //TODO: implement the gif-like thing in RichImage
            // MediaElement is a no go as it ignores the alpha channel
            return null;
        }

        double dScale = 0.1;
        private void aruAnimation(double dt)
        {
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
                    if ((aruScale > 5 && dScale > 0) ||
                        (aruScale < 1 && dScale < 0))
                        dScale *= -1;
                    var scale = ri.getScale();
                    scale += dScale;
                    ri.setScale(scale);
                }
            });
        }

        private void grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            var ptOnScreen = new Vec2(e.GetPosition(grid));
            var ptInGame = screenToWindow(ptOnScreen);
            if (beingDragged != null)
            {
                beingDragged.setPosition(ptInGame.X, ptInGame.Y);
                //Console.Out.WriteLine(beingDragged.boundingBoxContainsPoint(aruImage.getPosition()));
            }
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            var ptOnScreen = new Vec2(e.GetPosition(grid));
            var ptInGame = screenToWindow(ptOnScreen);
            //Console.WriteLine(ptOnScreen + " -> " + ptInGame);
            beingDragged = null;
            foreach (var ctrl in grid.Children)
            {
                if (!(ctrl is GameDisplayObject))
                    continue;
                var gdo = ctrl as GameDisplayObject;
                if (gdo.boundingBoxContainsPoint(ptInGame))
                {
                    beingDragged = (GameDisplayObject)ctrl;
                    break;
                }
            }
        }

        private void grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }
    }
}
