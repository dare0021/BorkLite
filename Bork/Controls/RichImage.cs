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

namespace Bork.Controls
{
    public class RichImage : Image
    {
        private ScaleTransform scaleTransform;
        private RotateTransform rotateTransform;
        private TranslateTransform translateTransform;
        private SkewTransform skewTransform;

        public RichImage()
        {
            RenderTransformOrigin = new Point(0.5, 0.5);

            scaleTransform = new ScaleTransform();
            scaleTransform.ScaleY = 1;
            scaleTransform.ScaleX = 1;

            rotateTransform = new RotateTransform();
            rotateTransform.Angle = 0;

            translateTransform = new TranslateTransform();
            translateTransform.X = 0;
            translateTransform.X = 0;

            skewTransform = new SkewTransform();
            skewTransform.AngleX = 0;
            skewTransform.AngleY = 0;

            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(scaleTransform);
            myTransformGroup.Children.Add(rotateTransform);
            myTransformGroup.Children.Add(translateTransform);
            myTransformGroup.Children.Add(skewTransform);

            RenderTransform = myTransformGroup;

            RadiusType = Common.RadiusMode.Min;
        }

        /// <summary>
        /// Load a resource WPF-BitmapImage (png, bmp, ...) from embedded resource defined as 'Resource' not as 'Embedded resource'.
        /// http://stackoverflow.com/a/9737958/2444520
        /// </summary>
        /// <param name="pathInApplication">Path without starting slash</param>
        public void setSource(string pathInApplication)
        {
            if (pathInApplication[0] == '/')
            {
                pathInApplication = pathInApplication.Substring(1);
            }
            Source = new BitmapImage(new Uri(@"pack://application:,,,/Bork;component/" + pathInApplication, UriKind.Absolute));
        }

        public double getRadius()
        {
            var x = Width * getScale().X / 2;
            var y = Height * getScale().Y / 2;

            if (radiusType == Common.RadiusMode.Avg)
                return (x + y) / 2;
            if (RadiusType == Common.RadiusMode.Max && x < y)
                return y;
            if (radiusType == Common.RadiusMode.Min && x > y)
                return y;
            return x;
        }

        public double getRotation()
        {
            return rotateTransform.Angle;
        }
        public void setRotation(double v)
        {
            rotateTransform.Angle = v;
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
            return new Vec2(scaleTransform.ScaleX, scaleTransform.ScaleY);
        }
        public void setScale(Vec2 v)
        {
            setScale(v.X, v.Y);
        }
        public void setScale(double x, double y)
        {
            scaleTransform.ScaleX = x;
            scaleTransform.ScaleY = y;
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
        /// Uses a circle using getRadius()
        /// </summary>
        /// <param name="v">Game coordinates</param>
        /// <returns></returns>
        public bool containsPoint(Vec2 v)
        {
            return (v - getPosition()).getLength() < getRadius();
        }

        protected Common.RadiusMode radiusType;
        public Common.RadiusMode RadiusType { get; set; }
    }
}
