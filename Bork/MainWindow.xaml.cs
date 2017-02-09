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
using System.Diagnostics;
using Bork.Controls;
using Bork.Helpers;
using Bork.Modules;

namespace Bork
{
    public partial class MainWindow : Window
    {
        private const int longtermInterval = 10000;
        /// <summary>
        /// 20Hz
        /// </summary>
        private const int shorttermInterval = 1000/20;        

        /// <summary>
        /// Key to Timestamp (int, ms since 1970/1/1)
        /// </summary>
        private Dictionary<Key, int> pressedKeys = new Dictionary<Key, int>();

        private GameDisplayObject beingDragged;
        private Dictionary<ulong, int> singleUseSpriteLifetimes = new Dictionary<ulong, int>();
        private List<UIElement> objectsMarkedForDeletion = new List<UIElement>();
        private List<SoundSystem> soundsMarkedForDeletion = new List<SoundSystem>();
        private List<UIElement> markedForAddition = new List<UIElement>();

        private bool mouseDown = false;

        private Mutex updateMutex = new Mutex();

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
            var iter2 = new GameDisplayObject(Bork.Properties.Resources.DummyImg2, Modules.CollisionDetection.CollisionTypes.Ship);
            iter2.setPosition(rng.Next((int)(Width / -2), (int)(Width / 2)),
                             rng.Next((int)(Height / -2), (int)(Height / 2)));
            iter2.setSize(iter2.Source.Width / 10, iter2.Source.Height / 10);
            grid.Children.Add(iter2);
            iter2.Speed = 5;
            iter2.MaxRotationSpeed = new Degree(5);
            iter2.RotationMode = Common.RotationMode.Tracking;

            var animtest = new GameDisplayObject("videos/deathAnimationDummy", Modules.CollisionDetection.CollisionTypes.Ship, true, 4, 0.5, 1);
            grid.Children.Add(animtest);
            
            iter2.TrackingTarget = animtest;
            iter2.Name = "firing_test";
            iter2.Allegiance = iter2.Name;

            var singleusetest = new GameDisplayObject("videos/deathAnimationDummy", Modules.CollisionDetection.CollisionTypes.None, true, 1, 3, 1);
            singleusetest.setPosition(200, 0);
            registerAsSingleUseVideo(singleusetest, 1);

            var singleusetest2 = new GameDisplayObject("videos/deathAnimationDummy", Modules.CollisionDetection.CollisionTypes.None, true, 4, 0.2, 1);
            singleusetest2.setPosition(-200, 0);
            registerAsSingleUseVideo(singleusetest2, 3);
            
            this.KeyDown += new KeyEventHandler(keyDown);
            this.KeyUp += new KeyEventHandler(keyUp);

            var longtermTimer = new System.Timers.Timer(longtermInterval);
            longtermTimer.Elapsed += OnLongtermTimer;
            longtermTimer.Start();

            var shorttermTimer = new System.Timers.Timer(shorttermInterval);
            shorttermTimer.Elapsed += OnShorttermTimer;
            shorttermTimer.Start();

            SoundSystem.init(canvasSounds.Children);

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


            Debug.Assert(UnitTest.run());

            var bgm = new SoundSystem("sounds/hemanLong.mp3", true, true);

            var testJsonImport = FileHandling.JsonToGdo("data/testShip.json");
            grid.Children.Add(testJsonImport);
        }

        private void OnLongtermTimer(object sender, ElapsedEventArgs e)
        {
            updateMutex.WaitOne();
            Dispatcher.Invoke(() =>
            {
                processAutocullObjects();
                processAutocullSounds();
            });
            updateMutex.ReleaseMutex();
        }

        private void OnShorttermTimer(object sender, ElapsedEventArgs e)
        {
            updateMutex.WaitOne();
            Dispatcher.Invoke(() =>
            {
                processCollisionDetection();
            });
            updateMutex.ReleaseMutex();
        }

        protected void Update(double dt)
        {
            updateMutex.WaitOne();
            Dispatcher.Invoke(() =>
            {
                if (Common.displayBoundingBox)
                {
                    canvasClearEveryUpdate.Children.Clear();
                }
                foreach (var obj in objectsMarkedForDeletion)
                {
                    grid.Children.Remove(obj);
                }
                objectsMarkedForDeletion.Clear();
                foreach (var obj in soundsMarkedForDeletion)
                {
                    SoundSystem.getAddTo().Remove(obj);
                }
                soundsMarkedForDeletion.Clear();
                foreach (var ctrl in grid.Children)
                {
                    var gdo = ctrl as GameDisplayObject;
                    var ri = ctrl as RichImage;
                    if (gdo != null && gdo.CollisionType != CollisionDetection.CollisionTypes.None)
                    {
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
                    else if(ri != null)
                    {
                        ri.Update(dt);
                        checkIfSingleUseFinished(ri);
                    }
                }
                foreach (var obj in markedForAddition)
                {
                    grid.Children.Add(obj);
                }
                markedForAddition.Clear();
            });
            updateMutex.ReleaseMutex();
        }

        private void checkIfSingleUseFinished(RichImage image)
        {
            if (!singleUseSpriteLifetimes.ContainsKey(image.id) 
                || singleUseSpriteLifetimes[image.id] > image.getLoopNo())
                return;
            var gdo = (GameDisplayObject)image;
            if (gdo != null)
            {
                gdo.kill(-1 * gdo.HP);
            }
            objectsMarkedForDeletion.Add(image);
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
            markedForAddition.Add(image);
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
                var gdo = ctrl as GameDisplayObject;
                if (gdo != null && gdo.boundingBoxContainsPoint(ptInGame))
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

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (!pressedKeys.Keys.Contains(e.Key))
            {
                // function called repeatedly as long as the key is pressed
                pressedKeys[e.Key] = e.Timestamp;
            }
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            if (!pressedKeys.Keys.Contains(e.Key)) 
                // I'm sure this is going to happen whether it makes sense or not.
                return;
            int pressed = pressedKeys[e.Key];
            pressedKeys.Remove(e.Key);
            if (e.Timestamp - pressed < Common.keyTypeThresh)
                keyTyped(sender, e, pressed);
        }

        /// <summary>
        /// Not a built-in function
        /// </summary>
        private void keyTyped(object sender, KeyEventArgs e, int pressed)
        {
            var parent = (GameDisplayObject)getChildByName("firing_test");
            GameDisplayObject child = null;
            switch (e.Key)
            {
                case Key.Space:
                    child = new GameDisplayObject(Bork.Properties.Resources.DummyImg1, CollisionDetection.CollisionTypes.Projectile);
                    parent.spawnChild(child, grid.Children, new Vec2(42, 42), 110);
                    break;
                case Key.LeftCtrl:
                    Vec2 rayDest = null;
                    float rayLen = 500;
                    var collisionObj = CollisionDetection.rayTrace(parent, rayLen, out rayDest);
                    if (collisionObj == null)
                    {
                        rayDest = new Vec2(0, 0);
                        rayDest.X = rayLen * Common.Sin(parent.getRotation());
                        rayDest.Y = rayLen * Common.Cos(parent.getRotation());
                        rayDest += parent.getPosition();
                    }
                    else
                    {
                        rayLen = (float)(parent.getPosition() - rayDest).getLength();

                        var singleuseExplosion = RichImageProfiles.explosion();
                        singleuseExplosion.setPosition(rayDest);
                        registerAsSingleUseVideo(singleuseExplosion);
                        var explosionSFx = new SoundSystem("sounds/hemanShort.mp3");
                    }
                    child = RichImageProfiles.redLaser();
                    child = parent.spawnChild(child, grid.Children, new Vec2(10, rayLen), 0, false, false);
                    child.setPosition((parent.getPosition() + rayDest)/2);
                    child.setScale(1, rayLen);
                    registerAsSingleUseVideo(child);
                    break;
            }
        }

        private UIElement getChildByName(string name)
        {
            foreach (var c in grid.Children)
            {
                var child = c as FrameworkElement;
                if (child == null)
                    continue;
                if (child.Name == name)
                    return child;
            }
            return null;
        }

        /// <summary>
        /// Marks for deletion any RichImage object that is
        /// more than 0.6 screeens away from the center.
        /// Ignores that extremely large sprites can be removed
        /// while still visible.
        /// </summary>
        private void processAutocullObjects()
        {
            var autocullThresh = 0.6;
            foreach (var child in grid.Children)
            {
                var ri = child as RichImage;
                if (ri == null 
                    || ri.AutoCull == false)
                    continue;

                var heightThresh = SystemParameters.FullPrimaryScreenHeight * autocullThresh;
                var widthThresh = SystemParameters.FullPrimaryScreenWidth * autocullThresh;
                var pos = ri.getPosition();
                var x = Math.Abs(pos.X);
                var y = Math.Abs(pos.Y);
                if (x > widthThresh || y > heightThresh)
                {
                    objectsMarkedForDeletion.Add(ri);
                }
            }
        }

        private void processAutocullSounds()
        {
            if (SoundSystem.getAddTo() == null)
                return;
            foreach (var child in SoundSystem.getAddTo())
            {
                var ss = child as SoundSystem;
                if (ss == null
                    || ss.AutoCull == false)
                    continue;
                if (ss.getState() == Common.AudioState.Done)
                {
                    soundsMarkedForDeletion.Add(ss);
                }
            }
        }

        private void processCollisionDetection()
        {
            Modules.CollisionDetection.Update();
            foreach (var child in grid.Children)
            {
                var gdo = child as GameDisplayObject;
                if (gdo == null || 
                    gdo.CollisionType != Modules.CollisionDetection.CollisionTypes.Projectile)
                    continue;

                if (gdo.isKilled())
                {
                    var singleuseExplosion = RichImageProfiles.explosion();
                    singleuseExplosion.setPosition(gdo.getPosition());
                    registerAsSingleUseVideo(singleuseExplosion);
                    objectsMarkedForDeletion.Add(gdo);
                    var explosionSFx = new SoundSystem("sounds/hemanShort.mp3");
                }
            }
        }
    }
}
