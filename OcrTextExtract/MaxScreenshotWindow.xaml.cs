using System.IO;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using OcrTextExtract.Converters;
using OcrTextExtract.Extionsions;
using OcrTextExtract.Helpers;
using OcrTextExtract.ViewModels;

namespace OcrTextExtract
{
    /// <summary>
    /// MaxScreenshotWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MaxScreenshotWindow : Window
    {
        private readonly MaxScreenshotWindowViewModel _viewModel;

        private Point point1 = new Point();
        private Point point2 = new Point();
        private StackPanel cutPanel = null;
        private Thickness cutPanelMargin = new Thickness(0, 0, 0, 0);

        private StackPanel topPanel = null;
        private StackPanel leftPanel = null;
        private StackPanel rightPanel = null;
        private StackPanel bottomPanel = null;

        private ToolControl tools = null;

        public MaxScreenshotWindow(ref MaxScreenshotWindowViewModel viewModel)
        {
            _viewModel = viewModel;

            InitializeComponent();

            this.Loaded += MaxScreenshotWindow_Loaded;

            // 执行顺序: down -> move -> up
            this.PreviewMouseLeftButtonDown += MaxScreenshotWindow_PreviewMouseLeftButtonDown;
            this.PreviewMouseMove += MaxScreenshotWindow_PreviewMouseMove;
            this.PreviewMouseLeftButtonUp += MaxScreenshotWindow_PreviewMouseLeftButtonUp;

            this.KeyDown += MaxScreenshotWindow_KeyDown;
            this.Closing += MaxScreenshotWindow_Closing;

        }

        /// <summary>
        /// 截图窗口初始化完毕
        /// </summary>
        private void MaxScreenshotWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置截图窗口背景颜色
            screenBox.Background = new SolidColorBrush(_viewModel.Styles.MaskBackgroundColor);
        }

        private bool IsMouseDown = false;
        private const double toolOrBtnHeight = 32;
        private CloseEnum myClose = CloseEnum.ExitApp; // 非操作关闭, 则表示关闭程序


        private void MaxScreenshotWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.OnCancel(myClose);
        }

        /// <summary>
        /// esc 键关闭截图
        /// </summary>
        private void MaxScreenshotWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                myClose = CloseEnum.CloseWindow;
                this.Close();
            }
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取鼠标相对于窗口的位置
            var point = e.GetPosition(this);


            // 如果鼠标点是落在工具栏, 则不做截图处理
            if (this.tools != null)
            {
                var tsPoint1 = new Point(this.tools.Margin.Left, this.tools.Margin.Top);
                var tsPoint2 = new Point(tsPoint1.X + this.tools.Width, tsPoint1.Y + this.tools.Height);

                // 落点 x, y 若是都在工具栏上的话
                if (point.X >= tsPoint1.X && point.X <= tsPoint2.X &&
                    point.Y >= tsPoint1.Y && point.Y <= tsPoint2.Y)
                {
                    return;
                }
            }



            // 设置背景图片
            var brush = new ImageBrush(ImageConvert.BitmapToBitmapImage(_viewModel.ScreenBitmap));
            brush.Stretch = Stretch.None;
            this.Background = brush;


            // 初始化或重置参数
            this.IsMouseDown = true;

            // 剪切面板
            this.cutPanel = ElementHelpers.CreateMask(Colors.Transparent);
            this.cutPanelMargin = new Thickness(0, 0, 0, 0);
            // 遮罩面板
            this.topPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.leftPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.rightPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            this.bottomPanel = ElementHelpers.CreateMask(_viewModel.Styles.MaskBackgroundColor);
            // 工具面板
            this.tools = new ToolControl(_viewModel);
            // 工具面板-添加按钮
            {
                // 保存
                this.tools.BtnOK.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {

                    myClose = CloseEnum.CloseWindow;
                    this.Close();
                    _viewModel.OnSave(CutImage());
                };

                // 取消
                this.tools.BtnCancel.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    myClose = CloseEnum.CloseWindow;
                    this.Close();
                };

                // 下载 or 另存为
                this.tools.BtnSaveAs.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    var bitmap = CutImage();

                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "png files (*.png)|*.png|jpeg files (*.jpeg)|*.jpeg|bmp files (*.bmp)|*.bmp";
                    saveFile.DefaultExt = ".png";
                    saveFile.FileName = "cut.png";
                    if (saveFile.ShowDialog() == true)
                    {
                        // 格式处理
                        var extName = Path.GetExtension(saveFile.FileName);
                        var format = extName switch
                        {
                            ".jpeg" or ".jpg" => ImageFormat.Jpeg,
                            ".png" => ImageFormat.Png,
                            ".bmp" => ImageFormat.Bmp,
                            _ => ImageFormat.Jpeg
                        };

                        bitmap.Save(saveFile.FileName, format);
                    }

                    myClose = CloseEnum.CloseWindow;
                    this.Close();
                };
            }


            // 清空上次操作遗留组件
            this.screenBox.Children.Clear();
            this.screenBox.Background = new SolidColorBrush(Colors.Transparent);

            // 添加-剪切面板
            this.screenBox.Children.Add(this.cutPanel);
            // 添加-遮罩面板
            this.screenBox.Children.Add(this.topPanel);
            this.screenBox.Children.Add(this.leftPanel);
            this.screenBox.Children.Add(this.rightPanel);
            this.screenBox.Children.Add(this.bottomPanel);
            // 添加-工具面板
            this.screenBox.Children.Add(this.tools);


            // 计算剪切面板、遮罩与工具面板的位置
            this.SetPanelRectangle(MouseState.Down, point);
            this.SetOverlayRectangle(MouseState.Down, point);
            this.SetToolsPostion(MouseState.Down, point);
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // 按下鼠标后, 移动操作才属于有效操作
            if (IsMouseDown)
            {
                // 获取鼠标相对于窗口的位置
                var point = e.GetPosition(this);

                this.SetPanelRectangle(MouseState.Move, point);
                this.SetOverlayRectangle(MouseState.Move, point);
                this.SetToolsPostion(MouseState.Move, point);
            }
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseDown)
            {
                // 获取鼠标相对于窗口的位置
                var point = e.GetPosition(this);

                this.SetPanelRectangle(MouseState.Up, point);
                this.SetOverlayRectangle(MouseState.Up, point);
                this.SetToolsPostion(MouseState.Up, point);

                this.IsMouseDown = false;
            }
        }



        private System.Drawing.Bitmap CutImage()
        {
            // 偏移像素
            int offset = -6;

            var cutPoint1 = new Point(this.cutPanelMargin.Left + offset, this.cutPanelMargin.Top + offset);
            var cutPoint2 = new Point(this.cutPanel.Width, this.cutPanel.Height);
            return ImageHelpers.Snapshot(cutPoint1, cutPoint2);
        }


        /// <summary>
        /// 剪切面板处理
        /// </summary>
        private void SetPanelRectangle(MouseState state, Point point)
        {
            // Console.WriteLine($"{ state.ToString() }, x = {point.X}, y = {point.Y}");

            if (state == MouseState.Down)
            {
                this.point1 = point;
                this.cutPanel.Margin = this.cutPanelMargin = new Thickness(point.X, point.Y, 0, 0);
            }
            else
            {
                this.point2 = point;

                var newWidth = this.point2.X - this.point1.X;
                var newHeight = this.point2.Y - this.point1.Y;
                if (newWidth >= 0)
                {
                    cutPanel.Width = newWidth;
                }
                else
                {
                    this.cutPanel.Width = Math.Abs(newWidth);
                    this.cutPanelMargin.Left = this.point1.X - Math.Abs(newWidth);
                    this.cutPanel.Margin = this.cutPanelMargin;
                }


                if (newHeight >= 0)
                {
                    cutPanel.Height = newHeight;
                }
                else
                {
                    this.cutPanel.Height = Math.Abs(newHeight);
                    this.cutPanelMargin.Top = this.point1.Y - Math.Abs(newHeight);
                    this.cutPanel.Margin = this.cutPanelMargin;
                }
            }
        }

        /// <summary>
        /// 遮罩处理
        /// </summary>
        private void SetOverlayRectangle(MouseState state, Point point)
        {
            this.topPanel.Width = this.Width;
            this.topPanel.Height = this.cutPanelMargin.Top;

            this.leftPanel.Width = this.cutPanelMargin.Left;
            this.leftPanel.Height = this.cutPanel.Height;
            this.leftPanel.Margin = new Thickness(0, this.cutPanelMargin.Top, 0, 0);
            
            this.rightPanel.Width = this.Width - point.X + 20; // +20 避免像素计算时出现问题
            this.rightPanel.Height = this.cutPanel.Height;
            this.rightPanel.Margin = new Thickness(this.cutPanelMargin.Left + this.cutPanel.Width, this.cutPanelMargin.Top, 0, 0);

            this.bottomPanel.Width = this.Width;
            this.bottomPanel.Height = this.Height - point.Y + 20;  // +20 避免像素计算时出现问题
            this.bottomPanel.Margin = new Thickness(0, this.cutPanelMargin.Top +  this.cutPanel.Height, 0, 0);

        }

        /// <summary>
        /// 工具面板位置处理
        /// </summary>
        private void SetToolsPostion(MouseState state, Point point)
        {
            if (state == MouseState.Down)
            {
                this.tools.Hide();
                this.tools.Height = 0;
            }

            if (state== MouseState.Up)
            {
                var mLeft = this.cutPanelMargin.Left + this.cutPanel.Width - this.tools.Width;
                var mTop = this.cutPanelMargin.Top + this.cutPanel.Height + 10;
                this.tools.Margin = new Thickness(Math.Max(mLeft, 20), mTop, 0, 0);

                this.tools.Show();
            }
        }

        /// <summary>
        /// 鼠标状态
        /// </summary>
        private enum MouseState
        {
            Down, // 按下
            Move, // 移动
            Up    // 弹起
        }
    }
}
