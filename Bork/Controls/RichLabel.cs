using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Bork.Helpers;

namespace Bork.Controls
{
    class RichLabel : Label
    {
        private ScaleTransform scaleTransform;
        private RotateTransform rotateTransform;
        private TranslateTransform translateTransform;
        private SkewTransform skewTransform;

        public RichLabel()
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
        }

        public double getRotation()
        {
            return -rotateTransform.Angle;
        }
        public void setRotation(double v)
        {
            rotateTransform.Angle = -v;
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
    }
}
