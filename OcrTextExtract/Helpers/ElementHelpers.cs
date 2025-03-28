using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OcrTextExtract.Helpers
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
    }
}
