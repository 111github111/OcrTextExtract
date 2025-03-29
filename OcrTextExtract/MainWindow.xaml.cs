using Microsoft.Win32;
using OcrTextExtract.Helpers;
using ScreenshotCapture.Converters;
using ScreenshotCapture.ViewModels;
using System.Windows;

namespace OcrTextExtract;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : System.Windows.Window
{
    private const string winTitle = "图像识别";
    private OCRUtils ocr = null;

    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += MainWindow_Loaded;
        this.Closing += MainWindow_Closing;
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        this.ocr = new OCRUtils();
    }


    /// <summary>
    /// 加载图片识别
    /// </summary>
    private void btnLoadingExtract_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        if (dialog.ShowDialog() == true)
        {
            this.textExtract(new System.Drawing.Bitmap(dialog.FileName));
        }
    }

    /// <summary>
    /// 截图事件-触发源
    /// </summary>
    private void btnCutExtract_Click(object sender, RoutedEventArgs e)
    {
        // 隐藏50ms毫秒, 避免当前窗口被拉入到中截图
        this.Hide();
        Thread.Sleep(50);

        MaxScreenshotWindowViewModel viewModel = new MaxScreenshotWindowViewModel();
        viewModel.OnSaveEvent += ViewModel_OnSaveEvent;
        viewModel.OnCancelEvent += ViewModel_OnCancelEvent;
        viewModel.SetStyles(s =>
        {
            // s.ToolBackgroundColor = ColorHelpers.FromString("#F2F2F2");
        });

        viewModel.ShowCapture();
    }

    /// <summary>
    /// 保存
    /// </summary>
    private void ViewModel_OnSaveEvent(System.Drawing.Bitmap bitmap)
    {
        this.Show();
        this.textExtract(bitmap);
    }

    /// <summary>
    /// 取消
    /// </summary>
    private void ViewModel_OnCancelEvent(CloseEnum state)
    {
        if (state == CloseEnum.CloseWindow)
        {
            this.Show();
        }
        else
        {
            this.Close();
        }
    }



    /// <summary>
    /// 文字提取
    /// </summary>
    private void textExtract(System.Drawing.Bitmap bitmap)
    {
        try
        {
            this.txImage.Source = ImageConvert.BitmapToBitmapImage(bitmap);
            this.txBox1.Text = ocr.test_string(bitmap);

            this.SetTitleSuccess();
        }
        catch (Exception ex)
        {
            this.SetTitleFail(ex);
            this.txBox1.Clear();
        }
    }

    private void SetTitleSuccess()
    {
        this.Title = winTitle + " - 识别成功";
    }
    private void SetTitleFail(Exception ex)
    {
        this.Title = winTitle + " - 识别失败, ex = " + ex.Message;
    }
}
