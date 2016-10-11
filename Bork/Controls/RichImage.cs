using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Bork.Helpers;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bork.Controls
{
    public class RichImage : Image
    {
        public readonly ulong id;
        private ImageResourceMap imageResourceMap = new ImageResourceMap();
        private Dictionary<string, AnimationProfile> animationProfiles = new Dictionary<string, AnimationProfile>();
        private string currentImage = null, currentAnimation = null;

        private ScaleTransform scaleTransform;
        private RotateTransform rotateTransform;
        private TranslateTransform translateTransform;
        private SkewTransform skewTransform;
        
        public RichImage(string path, bool animated = false, int frameCount = 0, double duration = 0, int from = 0)
        {
            RenderTransformOrigin = new Point(0.5, 0.5);
            scaleTransform = new ScaleTransform(1, -1);
            rotateTransform = new RotateTransform(0);
            translateTransform = new TranslateTransform(0, 0);
            skewTransform = new SkewTransform(0, 0);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(scaleTransform);
            myTransformGroup.Children.Add(rotateTransform);
            myTransformGroup.Children.Add(translateTransform);
            myTransformGroup.Children.Add(skewTransform);

            RenderTransform = myTransformGroup;

            id = Common.getNewUID();

            RadiusType = Common.RadiusMode.Min;
            IsSingleUse = false;

            if (!animated)
            {
                LoadResourceStatic("idle", path);
            }
            else
            {
                LoadResourceVideo("idle", path, frameCount, duration, from);
            }
            setAnimation("idle");
            setSize(Source.Width, Source.Height);
        }
        
        public AnimationProfile LoadResourceStatic(string name, string path)
        {
            imageResourceMap.LoadResource(name, path);
            animationProfiles[name] = new AnimationProfile(name);
            return animationProfiles[name];
        }
        /// <summary>
        /// Loads a video with the given frame rate using images of name namePrefix#, where # is ints from from to from + frameCount
        /// e.g. namePrefix1, namePrefix2, ... if from is 1
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentPath"></param>
        /// <param name="frameCount"></param>
        /// <param name="from">defaults to 0</param>
        public AnimationProfile LoadResourceVideo(string name, string parentPath, int frameCount, double duration, int from = 0)
        {
            imageResourceMap.ImportVideo(name, parentPath, frameCount, from);
            animationProfiles[name] = new AnimationProfile(name, duration, frameCount, from);
            return animationProfiles[name];
        }
        public void setAnimation(string name)
        {
            currentAnimation = name;
            var animation = animationProfiles[name];
            animation.resetLoopNo();
            setSource(animation.getCurrentItem());
        }
        public void setSource(string name)
        {
            Source = imageResourceMap.GetResource(name);
            currentImage = name;
        }

        public void Update(double dt)
        {
            if (currentAnimation != null)
            {
                var animationProfile = animationProfiles[currentAnimation];
                if (animationProfile.IsAnimated)
                {
                    animationProfile.Update(dt);
                    setSource(animationProfile.getCurrentItem());
                }
            }
        }

        /// <summary>
        /// For use in circular stuff e.g. buffs
        /// </summary>
        /// <returns></returns>
        public double getRadius()
        {
            var x = Width * getScale().X / 2;
            var y = Height * getScale().Y / 2;

            if (RadiusType == Common.RadiusMode.Avg)
                return (x + y) / 2;
            if (RadiusType == Common.RadiusMode.Max && x < y)
                return y;
            if (RadiusType == Common.RadiusMode.Min && x > y)
                return y;
            return x;
        }

        public double getRotation()
        {
            return -rotateTransform.Angle;
        }
        public void setRotation(double v)
        {
            rotateTransform.Angle = -v;
        }
        public Vec2 getSize()
        {
            return new Vec2(Width, Height);
        }
        public void setSize(Vec2 v)
        {
            setSize(v.X, v.Y);
        }
        public void setSize(double x, double y)
        {
            Width = x;
            Height = y;
        }
        public Vec2 getScale()
        {
            return new Vec2(scaleTransform.ScaleX, -scaleTransform.ScaleY);
        }
        public void setScale(Vec2 v)
        {
            setScale(v.X, v.Y);
        }
        public void setScale(double x, double y)
        {
            scaleTransform.ScaleX = x;
            scaleTransform.ScaleY = -y;
        }
        public Vec2 getPosition()
        {
            return new Vec2(translateTransform.X, translateTransform.Y);
        }
        public void setPosition(Vec2 v)
        {
            setPosition(v.X, v.Y);
        }
        public void setPosition(double x, double y)
        {
            translateTransform.X = x;
            translateTransform.Y = y;
        }

        /// <summary>
        /// Returns the rotated bounding box's two opposite points
        /// </summary>
        /// <param name="getInWorld">If false will use screen coordinates</param>
        /// <returns>world coordinates</returns>
        public Quad getBoundingBox(bool getInWorld = true)
        {
            var x0 = - getSize().X / 2;
            var x1 = getSize().X / 2;
            var y0 = - getSize().Y / 2;
            var y1 = getSize().Y / 2;
            var vect0 = new Vec2(x0, y0);
            var vect1 = new Vec2(x0, y1);
            var vect2 = new Vec2(x1, y1);
            var vect3 = new Vec2(x1, y0);
            var rotRad = Common.getRadians(getRotation());
            if (getInWorld)
                rotRad *= -1;
            vect0 = Common.rotateVector(vect0, rotRad);
            vect1 = Common.rotateVector(vect1, rotRad);
            vect2 = Common.rotateVector(vect2, rotRad);
            vect3 = Common.rotateVector(vect3, rotRad);
            if (!getInWorld)
            {
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
                RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
                var fudgeFactor = new Vec2(SystemParameters.FullPrimaryScreenWidth, SystemParameters.FullPrimaryScreenHeight) / 2;
                vect0.X += fudgeFactor.X;
                vect1.X += fudgeFactor.X;
                vect2.X += fudgeFactor.X;
                vect3.X += fudgeFactor.X;
                vect0.Y = fudgeFactor.Y - vect0.Y;
                vect1.Y = fudgeFactor.Y - vect1.Y;
                vect2.Y = fudgeFactor.Y - vect2.Y;
                vect3.Y = fudgeFactor.Y - vect3.Y;
            }
            vect0 += getPosition();
            vect1 += getPosition();
            vect2 += getPosition();
            vect3 += getPosition();
            return new Quad(vect0, vect1, vect2, vect3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v">world coordinates</param>
        /// <returns></returns>
        public bool circleContainsPoint(Vec2 v)
        {
            return (v - getPosition()).getLength() < getRadius();
        }

        /// <summary>
        /// </summary>
        /// <param name="v">world coordinates</param>
        /// <returns></returns>
        public bool boundingBoxContainsPoint(Vec2 v)
        {
            return getBoundingBoxGeometry().FillContains(new Point(v.X, v.Y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getInWorld">If false will use screen coordinates</param>
        /// <returns></returns>
        public PathGeometry getBoundingBoxGeometry(bool getInWorld = true)
        {
            var box = getBoundingBox(getInWorld);

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection(3);
            myPathSegmentCollection.Add(Common.generateLineSegment(box.v0, box.v1));
            myPathSegmentCollection.Add(Common.generateLineSegment(box.v1, box.v2));
            myPathSegmentCollection.Add(Common.generateLineSegment(box.v2, box.v3));

            PathFigure myPathFigure = new PathFigure(new Point(box.v0.X, box.v0.Y), myPathSegmentCollection, true);

            PathFigureCollection myPathFigureCollection = new PathFigureCollection(1);
            myPathFigureCollection.Add(myPathFigure);

            return new PathGeometry(myPathFigureCollection);
        }

        public int getLoopNo()
        {
            return animationProfiles[currentAnimation].getLoopNo();
        }
        
        public Common.RadiusMode RadiusType { get; set; }
        public bool IsSingleUse { get; set; }
    }
}
