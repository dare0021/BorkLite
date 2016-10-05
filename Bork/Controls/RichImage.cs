﻿using System;
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
        private ScaleTransform scaleTransform;
        private RotateTransform rotateTransform;
        private TranslateTransform translateTransform;
        private SkewTransform skewTransform;

        public RichImage()
        {
            RenderTransformOrigin = new Point(0.5, 0.5);

            scaleTransform = new ScaleTransform();
            scaleTransform.ScaleY = -1;
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

        /// <summary>
        /// For use in circular stuff e.g. buffs
        /// </summary>
        /// <returns></returns>
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
        /// <returns>Two opposite vertices of the box in world coordinates</returns>
        public Pair<Vec2> getBoundingBox()
        {
            var x0 = - getSize().X / 2;
            var x1 = getSize().X / 2;
            var y0 = - getSize().Y / 2;
            var y1 = getSize().Y / 2;
            var vect0 = new Vec2(x0, y0);
            var vect2 = new Vec2(x1, y1);
            var rotRad = Common.getRadians(getRotation());
            vect0 = Common.rotateVector(vect0, rotRad);
            vect2 = Common.rotateVector(vect2, rotRad);
            return new Pair<Vec2>(vect0 + getPosition(), vect2 + getPosition());
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
            var box = getBoundingBox();
            
            var pt0 = new Vec2(box.X.X, box.X.Y);
            var pt1 = new Vec2(box.X.X, box.Y.Y);
            var pt2 = new Vec2(box.Y.X, box.Y.Y);
            var pt3 = new Vec2(box.Y.X, box.X.Y);

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection(4);
            myPathSegmentCollection.Add(Common.generateLineSegment(pt0, pt1));
            myPathSegmentCollection.Add(Common.generateLineSegment(pt1, pt2));
            myPathSegmentCollection.Add(Common.generateLineSegment(pt2, pt3));
            myPathSegmentCollection.Add(Common.generateLineSegment(pt3, pt0));

            PathFigure myPathFigure = new PathFigure(new Point(pt0.X,pt0.Y), myPathSegmentCollection, true);

            PathFigureCollection myPathFigureCollection = new PathFigureCollection(1);
            myPathFigureCollection.Add(myPathFigure);

            PathGeometry myPathGeometry = new PathGeometry(myPathFigureCollection);

            return myPathGeometry.FillContains(new Point(v.X, v.Y));
        }

        protected Common.RadiusMode radiusType;
        public Common.RadiusMode RadiusType { get; set; }
    }
}
