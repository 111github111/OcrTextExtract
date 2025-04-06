using ScreenshotCapture.Enums;
using ScreenshotCapture.Extionsions;
using ScreenshotCapture.Helpers;
using ScreenshotCapture.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScreenshotCapture
{
    public class MaskControl
    {
        // 遮罩层对象
        private StackPanel TopPanel { get; set; }
        private StackPanel LeftPanel { get; set; }
        private StackPanel RightPanel { get; set; }
        private StackPanel BottomPanel { get; set; }

        // 边框线
        private MaskPath topPath = null;
        private MaskPath leftPath = null;
        private MaskPath rightPath = null;
        private MaskPath bottomPath = null;

        private bool isShow = true;

        private readonly MaxScreenshotWindowViewModel _viewModel;
        private readonly Window _window;
        private readonly StackPanel _cutPanel;
        private readonly Rect _windowRect;

        public MaskControl(MaxScreenshotWindowViewModel viewModel, Window window, StackPanel cutPanel)
        {
            this._viewModel = viewModel;
            this._window = window;
            this._cutPanel = cutPanel;
            this._windowRect = new Rect(0, 0, window.Width, window.Height);

            this.ResetPanel();
        }


        public void ResetPanel()
        {
            // 遮罩面板
            this.TopPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.LeftPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.RightPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.BottomPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);

            this.topPath = new MaskPath(_viewModel.Styles.LineColor, RaiseElement.Top);
            this.leftPath = new MaskPath(_viewModel.Styles.LineColor, RaiseElement.Left);
            this.rightPath = new MaskPath(_viewModel.Styles.LineColor, RaiseElement.Right);
            this.bottomPath = new MaskPath(_viewModel.Styles.LineColor, RaiseElement.Bottom);


            // Down -> Move -> Up
            this.topPath.OnMouseDownEvent += InnerMouseDownEvent;
            this.leftPath.OnMouseDownEvent += InnerMouseDownEvent;
            this.rightPath.OnMouseDownEvent += InnerMouseDownEvent;
            this.bottomPath.OnMouseDownEvent += InnerMouseDownEvent;
        }



        public void AppendTo(Panel panel)
        {
            panel.Children.Add(this.TopPanel);
            panel.Children.Add(this.LeftPanel);
            panel.Children.Add(this.RightPanel);
            panel.Children.Add(this.BottomPanel);

            topPath.Roots.ForEach(item => panel.Children.Add(item));
            leftPath.Roots.ForEach(item => panel.Children.Add(item));
            rightPath.Roots.ForEach(item => panel.Children.Add(item));
            bottomPath.Roots.ForEach(item => panel.Children.Add(item));
        }


        public void SetLayout(StackPanel cutPanel, Thickness cutPanelMargin, RaiseElement raiseObject)
        {

            var xMax = cutPanelMargin.Left + cutPanel.Width;
            var yMax = cutPanelMargin.Top + cutPanel.Height;


            this.TopPanel.Width = _windowRect.Width;
            this.TopPanel.Height = cutPanelMargin.Top;

            this.LeftPanel.Width = cutPanelMargin.Left;
            this.LeftPanel.Height = cutPanel.Height;
            this.LeftPanel.Margin = new Thickness(0, cutPanelMargin.Top, 0, 0);

            this.RightPanel.Width = _windowRect.Width - xMax; // +20 避免像素计算时出现问题
            this.RightPanel.Height = cutPanel.Height;
            this.RightPanel.Margin = new Thickness(xMax, cutPanelMargin.Top, 0, 0);

            this.BottomPanel.Width = _windowRect.Width;
            this.BottomPanel.Height = _windowRect.Height - yMax;  // +20 避免像素计算时出现问题
            this.BottomPanel.Margin = new Thickness(0, yMax, 0, 0);


            this.topPath.SetValue(new Point(cutPanelMargin.Left, cutPanelMargin.Top), new Point(xMax, cutPanelMargin.Top));
            this.leftPath.SetValue(new Point(cutPanelMargin.Left, cutPanelMargin.Top), new Point(cutPanelMargin.Left, yMax));
            this.rightPath.SetValue(new Point(xMax, cutPanelMargin.Top), new Point(xMax, yMax));
            this.bottomPath.SetValue(new Point(cutPanelMargin.Left, yMax), new Point(xMax, yMax));
            this.ShowPath();
        }


        public void ShowPath()
        {
            if (!isShow)
            {
                this.topPath.Roots.ForEach(item => item.Show());
                this.leftPath.Roots.ForEach(item => item.Show());
                this.rightPath.Roots.ForEach(item => item.Show());
                this.bottomPath.Roots.ForEach(item => item.Show());
                isShow = true;
            }
        }

        public void HidePath()
        {
            if (isShow)
            {
                this.topPath.Roots.ForEach(item => item.Hide());
                this.leftPath.Roots.ForEach(item => item.Hide());
                this.rightPath.Roots.ForEach(item => item.Hide());
                this.bottomPath.Roots.ForEach(item => item.Hide());
                isShow = false;
            }
        }


        public event Action<Point, RaiseElement> OnMouseDownEvent = null;

        private void InnerMouseDownEvent(object sender, MouseButtonEventArgs e, RaiseElement raiseObject)
        {
            // 线条事件, 阻止事件冒泡
            e.Handled = true;


            // 获取鼠标相对于窗口的位置
            var point = e.GetPosition(this._window);
            OnMouseDownEvent?.Invoke(point, raiseObject);
        }
    }
}
