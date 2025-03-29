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
        }
    }
}
