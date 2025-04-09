using ScreenshotCapture.Enums;
using ScreenshotCapture.Extionsions;
using ScreenshotCapture.Helpers;
using ScreenshotCapture.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
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
            this.SetRevokeState(false);
        }




        public DrawType DrawSelect { get; private set; } =  DrawType.None;

        private void BtnDrawRange_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.DrawSelect != DrawType.Range)
                SetDrawButtom(DrawType.Range);
            else
                SetDrawButtom(DrawType.None);

        }

        private void BtnDrawArrow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.DrawSelect != DrawType.Arrow)
                SetDrawButtom(DrawType.Arrow);
            else
                SetDrawButtom(DrawType.None);
        }



        public void SetRevokeState(bool isOK)
        {
            if (isOK)
            {
                this.BtnRevoke.Foreground = new SolidColorBrush(ColorHelpers.FromString("#222"));
                this.BtnRevoke.Cursor = null;
            }
            else
            {
                this.BtnRevoke.Foreground = new SolidColorBrush(ColorHelpers.FromString("#666"));
                this.BtnRevoke.Cursor = Cursors.No;
            }
        }




        private void SetDrawButtom(DrawType value)
        {
            this.DrawSelect = value;

            // 显示矩形勾选
            if (value == DrawType.Range)
            {
                this.DrawRangeOKIdentific.Show();
                this.DrawArrowOKIdentific.Hide();
            }

            // 显示箭头勾选
            if (value == DrawType.Arrow)
            {
                this.DrawRangeOKIdentific.Hide();
                this.DrawArrowOKIdentific.Show();
            }

            // 隐藏所有
            if (value == DrawType.None)
            {
                this.DrawRangeOKIdentific.Hide();
                this.DrawArrowOKIdentific.Hide();
            }
        }
    }
}
