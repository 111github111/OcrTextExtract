using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Microsoft.Win32;
using OcrTextExtract.Converters;
using OcrTextExtract.Helpers;
using OcrTextExtract.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            this.textExtract(new Bitmap(dialog.FileName));
        }
    }

    /// <summary>
    /// 截图事件-触发源
    /// </summary>
    private void btnCutExtract_Click(object sender, RoutedEventArgs e)
    {
        // 1. 当前窗口隐藏 50ms
        // 2. 截取全屏
        // 3. 将截取的图像设置为图像处理窗口的背景图片, 然后进行区域截取处理

        this.Hide();
        Thread.Sleep(50);


        var bitmap = ImageHelpers.SnapshotScreen();
        MaxScreenshotWindowViewModel viewModel = new MaxScreenshotWindowViewModel(bitmap);
        viewModel.OnSaveEvent += ViewModel_OnSaveEvent;
        viewModel.OnCancelEvent += ViewModel_OnCancelEvent;

        var screenShot = new MaxScreenshotWindow(ref viewModel);
        screenShot.Show();

    }

    /// <summary>
    /// 保存
    /// </summary>
    private void ViewModel_OnSaveEvent(Bitmap bitmap)
    {
        this.Show();
        this.textExtract(bitmap);
    }

    /// <summary>
    /// 取消
    /// </summary>
    private void ViewModel_OnCancelEvent(object obj)
    {
        this.Show();
    }



    /// <summary>
    /// 文字提取
    /// </summary>
    private void textExtract(Bitmap bitmap)
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
