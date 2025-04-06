using ScreenshotCapture.Helpers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScreenshotCapture
{
    public class MaskPath
    {
        private Point startPoint = new Point(0, 0);
        private Point endPoint = new Point(0, 0);


        public List<Path> Roots { get; } = new List<Path>(3);

        private Path LineRoot {  get; set; }
        private Path StartRoot {  get; set; }
        private Path EndRoot {  get; set; }

        private LineGeometry Line { get; set; }
        private EllipseGeometry EllipseStart { get; set; }
        private EllipseGeometry EllipseCenter { get; set; }
        private EllipseGeometry EllipseEnd { get; set; }


        public RaiseElement RaiseObject { get; set; }

        public MaskPath(Color bgColor, RaiseElement raiseObject)
        {
            // 赋值
            this.RaiseObject = raiseObject;

            // 初始化
            this.LineRoot = ElementHelpers.CreatePath(bgColor, raiseObject);
            this.StartRoot = ElementHelpers.CreatePath(bgColor, raiseObject);
            this.EndRoot = ElementHelpers.CreatePath(bgColor, raiseObject);


            // 线条 - 主体
            this.Line = new LineGeometry(startPoint, endPoint);
            this.EllipseCenter = new EllipseGeometry(new Point(Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y)), 5, 5);
            var group = new GeometryGroup();
            group.Children.Add(this.Line);
            group.Children.Add(this.EllipseCenter);
            this.LineRoot.Data = group;

            // 线条 - 开始原点
            this.EllipseStart = new EllipseGeometry(startPoint, 5, 5);
            this.StartRoot.Data = this.EllipseStart;

            // 线条 - 结束原点
            this.EllipseEnd = new EllipseGeometry(endPoint, 5, 5);
            this.EndRoot.Data = this.EllipseEnd;


            // 将 path 添加到集合中
            this.Roots.Add(this.LineRoot);
            this.Roots.Add(this.StartRoot);
            this.Roots.Add(this.EndRoot);

            this.LineRoot.PreviewMouseDown += Root_PreviewMouseDown;
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


        public event Action<object, MouseButtonEventArgs, RaiseElement> OnMouseDownEvent = null;

        private void Root_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMouseDownEvent?.Invoke(sender, e, this.RaiseObject);
        }
    }


    public enum RaiseElement
    {
        CutRange, // 截图矩形

        Top,      // 顶部线条
        Left,     // 左侧线条
        Right,    // 右侧线条
        Bottom,   // 底部线条
    }
}
