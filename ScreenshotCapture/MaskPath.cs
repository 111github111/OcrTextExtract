using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScreenshotCapture
{
    public class MaskPath
    {
        private Point startPoint = new Point(0, 0);
        private Point endPoint = new Point(0, 0);


        public Path Root {  get; private set; }
        public LineGeometry Line { get; private set; }
        public EllipseGeometry EllipseStart { get; private set; }
        public EllipseGeometry EllipseCenter { get; private set; }
        public EllipseGeometry EllipseEnd { get; private set; }

        public MaskPath(Color bgColor)
        {
            this.Root = new Path();
            this.Root.StrokeThickness = 3;
            this.Root.Stroke = new SolidColorBrush(bgColor);

            this.Line = new LineGeometry(startPoint, endPoint);
            this.EllipseStart = new EllipseGeometry(startPoint, 5, 5);
            this.EllipseCenter = new EllipseGeometry(new Point(Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y)), 5, 5);
            this.EllipseEnd = new EllipseGeometry(endPoint, 5, 5);

            var group = new GeometryGroup();
            group.Children.Add(this.Line);
            group.Children.Add(this.EllipseStart);
            group.Children.Add(this.EllipseCenter);
            group.Children.Add(this.EllipseEnd);

            this.Root.Data = group;
        }


        public void SetValue(Point startPoint, Point endPoint)
        {
            this.Line.StartPoint = startPoint;
            this.Line.EndPoint = endPoint;

            this.EllipseStart.Center = startPoint;
            this.EllipseEnd.Center = endPoint;

            if (startPoint.X == endPoint.X)
            {
                var myY = Math.Abs((startPoint.Y + endPoint.Y) / 2);
                this.EllipseCenter.Center = new Point(startPoint.X, myY);
            }
            else
            {
                var myX = Math.Abs((startPoint.X + endPoint.X) / 2);
                this.EllipseCenter.Center = new Point(myX, startPoint.Y);
            }
        }
    }
}
