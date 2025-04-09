using ScreenshotCapture.Enums;
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


        private RaiseElement[] weCorsor = { RaiseElement.Left, RaiseElement.Right };
        private RaiseElement[] nsCorsor = { RaiseElement.Top, RaiseElement.Bottom };
        private RaiseElement[] neswCorsor = { RaiseElement.TopRight, RaiseElement.RightTop, RaiseElement.LeftBottom, RaiseElement.BottomLeft };
        private RaiseElement[] nwseCorsor = { RaiseElement.LeftTop, RaiseElement.TopLeft, RaiseElement.RightBottom, RaiseElement.BottomRight };



        public MaskPath(Color bgColor, RaiseElement rs)
        {
            // 赋值
            this.RaiseObject = rs;

            // 初始化
            this.LineRoot = ElementHelpers.CreatePath(bgColor);
            this.StartRoot = ElementHelpers.CreatePath(bgColor);
            this.EndRoot = ElementHelpers.CreatePath(bgColor);
            this.SetCursor(MaskCursor.SetCursor);


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

            this.StartRoot.PreviewMouseDown += StartRoot_PreviewMouseDown;
            this.EndRoot.PreviewMouseDown += EndRoot_PreviewMouseDown;
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


        public void SetCursor(MaskCursor cursorValue)
        {
            this.SetCursor(this.LineRoot, this.RaiseObject, cursorValue);
            this.SetCursor(this.StartRoot, this.RaiseObject + 1, cursorValue);
            this.SetCursor(this.EndRoot, this.RaiseObject + 2, cursorValue);
        }


        /// <summary>
        /// 设置鼠标状态
        /// </summary>
        private void SetCursor(Path path, RaiseElement rs, MaskCursor maskCursor)
        {
            if (maskCursor == MaskCursor.SetCursor)
            {
                path.Cursor = Cursors.Hand;


                // 左右 ↔
                if (weCorsor.Contains(rs))
                    path.Cursor = Cursors.SizeWE;

                // 上下 ↕
                if (nsCorsor.Contains(rs))
                    path.Cursor = Cursors.SizeNS;

                // // 左下右上 ↗↙
                // if (neswCorsor.Contains(rs))
                //     path.Cursor = Cursors.SizeNESW;
                // 
                // // 左上右下 ↖↘
                // if (nwseCorsor.Contains(rs))
                //     path.Cursor = Cursors.SizeNWSE;
            }
            else
            {
                path.Cursor = Cursors.No;
            }
        }




        public event Action<object, MouseButtonEventArgs, RaiseElement> OnMouseDownEvent = null;

        private void Root_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMouseDownEvent?.Invoke(sender, e, this.RaiseObject);
        }

        private void StartRoot_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMouseDownEvent?.Invoke(sender, e, this.RaiseObject + 1);
        }

        private void EndRoot_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMouseDownEvent?.Invoke(sender, e, this.RaiseObject + 2);
        }

    }

    public enum MaskCursor
    {
        Default,
        SetCursor,
    }
}
