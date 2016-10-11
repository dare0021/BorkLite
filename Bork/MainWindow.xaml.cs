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
        private GameDisplayObject beingDragged;
        private Dictionary<ulong, int> singleUseSpriteLifetimes = new Dictionary<ulong, int>();
        private List<RichImage> markedForDeletion = new List<RichImage>();

        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            Height = SystemParameters.FullPrimaryScreenHeight;
            Width = SystemParameters.FullPrimaryScreenWidth;

            var rng = new Random();
            Vec2[] locations = {new Vec2(300,300),
                                new Vec2(300,-300),
                                new Vec2(-300,-300),
                                new Vec2(-300,300) };
            for (int i=0; i<4; i++)
            {
                var iter = new RichImage("videos/deathAnimationDummy/" + i + ".png");
                iter.setPosition(locations[i]);
                iter.setSize(32, 32);
                grid.Children.Add(iter);
            }
            var iter2 = new GameDisplayObject(Bork.Properties.Resources.DummyImg2);
            iter2.setPosition(rng.Next((int)(Width / -2), (int)(Width / 2)),
                             rng.Next((int)(Height / -2), (int)(Height / 2)));
            iter2.setSize(iter2.Source.Width / 10, iter2.Source.Height / 10);
            grid.Children.Add(iter2);
            iter2.Speed = 5;
            iter2.MaxRotationSpeed = 5;
            iter2.RotationMode = Common.RotationMode.Tracking;

            var animtest = new GameDisplayObject("videos/deathAnimationDummy", true, 4, 0.5, 1);
            grid.Children.Add(animtest);

            iter2.TrackingTarget = animtest;

            var singleusetest = new GameDisplayObject("videos/deathAnimationDummy", true, 1, 3, 1);
            singleusetest.setPosition(200, 0);
            registerAsSingleUseVideo(singleusetest, 1);
            grid.Children.Add(singleusetest);

            var singleusetest2 = new GameDisplayObject("videos/deathAnimationDummy", true, 4, 0.2, 1);
            singleusetest2.setPosition(-200, 0);
            registerAsSingleUseVideo(singleusetest2, 3);
            grid.Children.Add(singleusetest2);

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
                foreach (var image in markedForDeletion)
                {
                    grid.Children.Remove(image);
                }
                markedForDeletion.Clear();
                foreach (var ctrl in grid.Children)
                {
                    if (ctrl is GameDisplayObject)
                    {
                        var gdo = (GameDisplayObject)ctrl;
                        if (Common.displayBoundingBox)
                        {
                            var box = gdo.getBoundingBoxGeometry(false);
                            var p = new Path();
                            p.Data = box;
                            p.Fill = Brushes.Transparent;
                            p.Stroke = Brushes.Red;
                            p.StrokeThickness = 1;
                            p.Name = "boundingBox" + ctrl.GetHashCode();
                            canvasClearEveryUpdate.Children.Add(p);
                        }
                        gdo.Update(dt);
                        checkIfSingleUseFinished(gdo);
                    }
                    else if(ctrl is RichImage)
                    {
                        var ri = (RichImage)ctrl;
                        ri.Update(dt);
                        checkIfSingleUseFinished(ri);
                    }
                }
            });
        }

        private void checkIfSingleUseFinished(RichImage image)
        {
            if (!singleUseSpriteLifetimes.ContainsKey(image.id) 
                || singleUseSpriteLifetimes[image.id] > image.getLoopNo())
                return;
            if (image is GameDisplayObject)
            {
                var gdo = (GameDisplayObject)image;
                gdo.kill(-1 * gdo.HP);
            }
            markedForDeletion.Add(image);
        }

        public Vec2 screenToWindow(Vec2 v)
        {
            return screenToWindow(v.X, v.Y);
        }
        public Vec2 screenToWindow(double x, double y)
        {
            return new Vec2(x - Width / 2, y - Height / 2);
        }

        /// <summary>
        /// Remove image image after loops loops
        /// </summary>
        /// <param name="image"></param>
        /// <param name="loops"></param>
        /// <returns></returns>
        public void registerAsSingleUseVideo(RichImage image, int loops = 1)
        {
            singleUseSpriteLifetimes[image.id] = loops;
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
