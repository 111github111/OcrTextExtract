using System;
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


        public static StackPanel CreateTools(Color bgColor)
        {
            var stackPanel = new StackPanel();
            stackPanel.Width = 0;
            stackPanel.Height = 0;
            stackPanel.Background = new SolidColorBrush(bgColor);
            stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.FlowDirection = FlowDirection.RightToLeft;
            return stackPanel;
        }



        public static StackPanel CreateButton(double width, double height, double marginRight, Color bgColor)
        {
            var stackPanel = new StackPanel();
            stackPanel.Width = width;
            stackPanel.Height = height;
            stackPanel.Background = new SolidColorBrush(bgColor);
            stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            stackPanel.Margin = new Thickness(0, 0, marginRight, 0);

            return stackPanel;
        }


        public static Label CreateLabel(string text, double width, double height, double marginRight, Color bgColor)
        {
            var label = new Label();
            label.Content = text;
            label.Width = width;
            label.Height = height;
            label.Background = new SolidColorBrush(bgColor);
            label.HorizontalAlignment = HorizontalAlignment.Right;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.Margin = new Thickness(0, 0, marginRight, 0);

            return label;
        }

    }
}
