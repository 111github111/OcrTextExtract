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
        public static Path CreatePath(Color bgColor, RaiseElement raiseObject)
        {
            var path = new Path();
            path.StrokeThickness = 3;
            path.Stroke = new SolidColorBrush(bgColor); // 线条颜色
            path.Fill = new SolidColorBrush(bgColor);   // 填充颜色
            path.Cursor = Cursors.Hand;                 // 悬停鼠标样式

            return path;
        }
    }
}
