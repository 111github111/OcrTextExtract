using ScreenshotCapture.Extionsions;
using ScreenshotCapture.Helpers;
using ScreenshotCapture.ViewModels;
using System.Windows;
using System.Windows.Controls;

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
        private readonly Rect _windowRect;

        public MaskControl(MaxScreenshotWindowViewModel viewModel, Rect windowRect)
        {
            this._viewModel = viewModel;
            this._windowRect = windowRect;


            this.ResetPanel();
        }


        public void ResetPanel()
        {
            // 遮罩面板
            this.TopPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.LeftPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.RightPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.BottomPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);

            this.topPath = new MaskPath(_viewModel.Styles.LineColor);
            this.leftPath = new MaskPath(_viewModel.Styles.LineColor);
            this.rightPath = new MaskPath(_viewModel.Styles.LineColor);
            this.bottomPath = new MaskPath(_viewModel.Styles.LineColor);
        }

        public void AppendTo(Panel panel)
        {
            panel.Children.Add(this.TopPanel);
            panel.Children.Add(this.LeftPanel);
            panel.Children.Add(this.RightPanel);
            panel.Children.Add(this.BottomPanel);

            panel.Children.Add(topPath.Root);
            panel.Children.Add(leftPath.Root);
            panel.Children.Add(rightPath.Root);
            panel.Children.Add(bottomPath.Root);
        }


        public void SetLayout(StackPanel cutPanel, Thickness cutPanelMargin)
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
                this.topPath.Root.Show();
                this.leftPath.Root.Show();
                this.rightPath.Root.Show();
                this.bottomPath.Root.Show();
                isShow = true;
            }
        }

        public void HidePath()
        {
            if (isShow)
            {
                this.topPath.Root.Hide();
                this.leftPath.Root.Hide();
                this.rightPath.Root.Hide();
                this.bottomPath.Root.Hide();
                isShow = false;
            }
        }
    }
}
