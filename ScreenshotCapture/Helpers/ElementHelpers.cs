using ScreenshotCapture.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScreenshotCapture.Helpers
{
    public class ElementHelpers
    {
        /// <summary>
        /// 创建遮罩面板
        /// </summary>
        public static StackPanel CreateMask(Color bgColor)
        {
            var stackPanel = new StackPanel();
            stackPanel.Width = 0;
            stackPanel.Height = 0;
            stackPanel.Background = new SolidColorBrush(bgColor);
            stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;

            return stackPanel;
        }

        /// <summary>
        /// 创建path路径
        /// </summary>
        public static Path CreatePath(Color bgColor, RaiseElement rs)
        {
            var path = new Path();
            path.StrokeThickness = 3;
            path.Stroke = new SolidColorBrush(bgColor); // 线条颜色
            path.Fill = new SolidColorBrush(bgColor);   // 填充颜色
            path.Cursor = Cursors.Hand;                 // 悬停鼠标样式


            var weKeys = new[] { RaiseElement.Left, RaiseElement.Right }; 
            var nsKeys = new[] { RaiseElement.Top, RaiseElement.Bottom };
            var neswKeys = new[] { RaiseElement.TopRight, RaiseElement.RightTop, RaiseElement.LeftBottom, RaiseElement.BottomLeft };
            var nwseKeys = new[] { RaiseElement.LeftTop, RaiseElement.TopLeft, RaiseElement.RightBottom, RaiseElement.BottomRight };

            // 左右 ↔
            if (weKeys.Contains(rs))
                path.Cursor = Cursors.SizeWE;

            // 上下 ↕
            if (nsKeys.Contains(rs))
                path.Cursor = Cursors.SizeNS;

            // // 左下右上 ↗↙
            // if (neswKeys.Contains(rs))
            //     path.Cursor = Cursors.SizeNESW;
            // 
            // // 左上右下 ↖↘
            // if (nwseKeys.Contains(rs))
            //     path.Cursor = Cursors.SizeNWSE;

            return path;
        }
    }
}
