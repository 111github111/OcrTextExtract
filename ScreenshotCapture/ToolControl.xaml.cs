using ScreenshotCapture.Extionsions;
using ScreenshotCapture.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScreenshotCapture
{
    /// <summary>
    /// ToolControl.xaml 的交互逻辑
    /// </summary>
    public partial class ToolControl : UserControl
    {
        private readonly MaxScreenshotWindowViewModel _viewModel;

        public ToolControl(MaxScreenshotWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
            InitializeComponent();

            this.Loaded += ToolControl_Loaded;
        }

        private void ToolControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Background = new SolidColorBrush(_viewModel.Styles.ToolBackgroundColor);

            this.toolBox.Width = this.Width = _viewModel.Styles.ToolPanelWidth;
            this.toolBox.Height = this.Height = _viewModel.Styles.ToolPanelHeight;

            this.BtnOK.Height = _viewModel.Styles.ToolPanelButtonHeight;
            this.BtnCancel.Height = _viewModel.Styles.ToolPanelButtonHeight;

            // 默认隐藏
            this.DrawRangeOKIdentific.Hide();
            this.DrawArrowOKIdentific.Hide();
        }




        public bool DrawRangeIsSelected { get; private set; } = false;
        public bool DrawArrowIsSelected { get; private set; } = false;

        private void BtnDrawRange_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DrawRangeIsSelected)
            {
                DrawRangeIsSelected = false;
                this.DrawRangeOKIdentific.Hide();
            }
            else
            {
                DrawRangeIsSelected = true;
                this.DrawRangeOKIdentific.Show();
            }
        }

        private void BtnDrawArrow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DrawArrowIsSelected)
            {
                DrawArrowIsSelected = false;
                this.DrawArrowOKIdentific.Hide();
            }
            else
            {
                DrawArrowIsSelected = true;
                this.DrawArrowOKIdentific.Show();
            }
        }
    }
}
