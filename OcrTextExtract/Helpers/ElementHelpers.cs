using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OcrTextExtract.Helpers
{
    public class ElementHelpers
    {
        public static StackPanel CreateNewStackPanel(Color bgColor)
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
