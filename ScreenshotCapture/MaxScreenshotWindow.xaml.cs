using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using ScreenshotCapture.Helpers;
using ScreenshotCapture.ViewModels;
using ScreenshotCapture.Converters;
using ScreenshotCapture.Extionsions;
using ScreenshotCapture.Enums;
using System.Windows.Shapes;

namespace ScreenshotCapture
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

        private MaskControl maskControl = null;

        private ToolControl tools = null;
        private bool toolIsNeedHide = false; // tool 是否需要隐藏, 截图时


        private bool isMouseDown = false;
        private CloseEnum myClose = CloseEnum.ExitApp; // 非操作关闭, 则表示关闭程序


        private WorkStateEnum workState = WorkStateEnum.Capture; // 工作状态


        // 记录调整前的大小
        private Rect tempRect = new Rect();
        private Thickness tempCutPanelMargin = new Thickness(0, 0, 0, 0);
        private RaiseElement historyRaise = RaiseElement.CutRange;
        private RaiseElement raiseObject = RaiseElement.CutRange;


        private bool isExchange = false; // 用于解决快速交换时出现的bug
        private RaiseElement[] leftKeys = new[] { RaiseElement.Left, RaiseElement.LeftTop, RaiseElement.LeftBottom, RaiseElement.TopLeft, RaiseElement.BottomLeft };
        private RaiseElement[] rightKeys = new[] { RaiseElement.Right, RaiseElement.RightTop, RaiseElement.RightBottom, RaiseElement.TopRight, RaiseElement.BottomRight };
        private RaiseElement[] topKeys = new[] { RaiseElement.Top, RaiseElement.TopLeft, RaiseElement.TopRight, RaiseElement.LeftTop, RaiseElement.RightTop };
        private RaiseElement[] bottomKeys = new[] { RaiseElement.Bottom, RaiseElement.BottomLeft, RaiseElement.BottomRight, RaiseElement.LeftBottom, RaiseElement.RightBottom };


        private readonly List<Path> drawRamgeList = new List<Path>();
        private readonly List<Path> drawArrowList = new List<Path>();
        private Path tempPath = null;
        private Geometry tempGmtry = null;
        private bool isCreate = false;

        public MaxScreenshotWindow(MaxScreenshotWindowViewModel viewModel)
        {
            _viewModel = viewModel;

            InitializeComponent();

            this.Loaded += MaxScreenshotWindow_Loaded;

            // 执行顺序: down -> move -> up
            //   this.PreviewMouseLeftButtonDown
            //   this.PreviewMouseMove
            //   this.PreviewMouseLeftButtonUp
            this.MouseLeftButtonDown += MaxScreenshotWindow_PreviewMouseLeftButtonDown;
            this.MouseMove += MaxScreenshotWindow_PreviewMouseMove;
            this.MouseLeftButtonUp += MaxScreenshotWindow_PreviewMouseLeftButtonUp;

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
            Console.WriteLine("Window...Down");

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

            if (this.raiseObject <= (RaiseElement)100)
            {
                // 设置背景图片
                var brush = new ImageBrush(ImageConvert.BitmapToBitmapImage(_viewModel.ScreenBitmap));
                brush.Stretch = Stretch.None;
                this.Background = brush;


                // 剪切面板
                this.cutPanel = ElementHelpers.CreateMask(Colors.Transparent);
                this.cutPanelMargin = new Thickness(0, 0, 0, 0);
                // 遮罩面板
                this.maskControl = new MaskControl(_viewModel, this, this.cutPanel);
                this.maskControl.OnMouseDownEvent += OnMouseDownEvent;

                // 工具面板
                this.tools = CreateTool();

                // 清空上次操作遗留组件
                this.screenBox.Children.Clear();

                // 添加-剪切面板
                this.screenBox.Children.Add(this.cutPanel);
                // 添加-遮罩面板
                this.maskControl.AppendTo(this.screenBox);
                // 添加-工具面板
                this.screenBox.Children.Add(this.tools);

                this.screenBox.Background = new SolidColorBrush(Colors.Transparent);

                // 非工具栏操作触发标识, 重置为截图操作
                this.raiseObject = RaiseElement.CutRange;
            }

            OnMouseDownEvent(point, this.raiseObject);
        }



        /// <summary>
        /// 鼠标移动
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            Console.WriteLine("win -- " + Random.Shared.Next(10, 50));

            // 按下鼠标后, 移动操作才属于有效操作
            if (isMouseDown)
            {
                // 获取鼠标相对于窗口的位置
                var point = e.GetPosition(this);
                // 鼠标移动
                this.SetPanelRectangle(MouseState.Move, point, this.raiseObject);
                this.SetOverlayRectangle(MouseState.Move, this.raiseObject);
                this.SetToolsPostion(MouseState.Move, point, this.raiseObject);

            }
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isMouseDown)
            {

                // 获取鼠标相对于窗口的位置
                var point = e.GetPosition(this);

                // 鼠标弹起
                this.SetPanelRectangle(MouseState.Up, point, this.raiseObject);
                this.SetOverlayRectangle(MouseState.Up, this.raiseObject);
                this.SetToolsPostion(MouseState.Up, point, this.raiseObject);

                this.isMouseDown = false;
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


        private ToolControl CreateTool()
        {
            // 工具面板
            var myTool = new ToolControl(_viewModel);

            // 工具面板-添加按钮
            {
                // 保存
                myTool.BtnOK.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    // 1. 隐藏工具栏并延时 38 毫秒
                    myTool.Hide();
                    this.maskControl.HidePath();
                    Task.Delay(38).ContinueWith(s =>
                    {
                        // 2. 回到 ui 主线程, 继续执行
                        this.Dispatcher.Invoke(() =>
                        {
                            myClose = CloseEnum.CloseWindow;
                            var bitmap = CutImage();
                            this.Hide();
                            _viewModel.OnSave(bitmap);
                            this.Close();
                        });
                    });
                };

                // 取消
                myTool.BtnCancel.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    myClose = CloseEnum.CloseWindow;
                    this.Close();
                };

                // 下载 or 另存为
                myTool.BtnSaveAs.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    // 1. 隐藏工具栏并延时 38 毫秒
                    myTool.Hide();
                    this.maskControl.HidePath();
                    Task.Delay(38).ContinueWith(s =>
                    {
                        // 2. 回到 ui 主线程, 继续执行
                        this.Dispatcher.Invoke(() =>
                        {
                            var bitmap = CutImage();
                            this.Hide();

                            SaveFileDialog saveFile = new SaveFileDialog();
                            saveFile.Filter = "png files (*.png)|*.png|jpeg files (*.jpeg)|*.jpeg|bmp files (*.bmp)|*.bmp";
                            saveFile.DefaultExt = ".png";
                            saveFile.FileName = "cut.png";
                            if (saveFile.ShowDialog() == true)
                            {
                                // 格式处理
                                var extName = System.IO.Path.GetExtension(saveFile.FileName);
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
                            _viewModel.OnSaveAs();
                            this.Close();
                        });
                    });

                };

                // 绘制矩形
                myTool.BtnDrawRange.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    if (myTool.DrawRangeIsSelected)
                    {
                        // 记录历史状态
                        historyRaise = this.raiseObject;

                        // 设置当前操作状态
                        this.raiseObject = RaiseElement.DrawRange;
                    }
                    else
                    {
                        // 还原历史状态
                        this.raiseObject = historyRaise;
                    }
                };

                // 绘制箭头
                myTool.BtnDrawArrow.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    if (myTool.DrawArrowIsSelected)
                    {
                        // 记录历史状态
                        historyRaise = this.raiseObject;

                        // 设置当前操作状态
                        this.raiseObject = RaiseElement.DrawArrow;
                    }
                    else
                    {
                        // 还原历史状态
                        this.raiseObject = historyRaise;
                    }
                };
            }

            return myTool;
        }


        /// <summary>
        /// 剪切面板处理
        /// </summary>
        private void SetPanelRectangle(MouseState state, Point point, RaiseElement rs)
        {
            // Console.WriteLine($"{ state.ToString() }, x = {point.X}, y = {point.Y}");

            this.raiseObject = rs;
            if (rs == RaiseElement.CutRange)
            {
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
            else if (rs == RaiseElement.DrawRange)
            {
                var mgLeft = this.cutPanelMargin.Left;
                var mgTop = this.cutPanelMargin.Top;
                var absWidth = cutPanelMargin.Left + cutPanel.Width;
                var absHeight = cutPanelMargin.Top + cutPanel.Height;

                if (state == MouseState.Down)
                {
                    // 只有在截图区域创建的绘图对象才是有效对象
                    if (point.X > mgLeft && point.X < absWidth &&
                        point.Y > mgTop && point.Y < absHeight)
                    {
                        isCreate = true;

                        this.point1 = point;
                        tempPath = new Path();
                        tempPath.Stroke = new SolidColorBrush(Colors.Red);
                        tempPath.StrokeThickness = 2;

                        var rectangle = new RectangleGeometry();
                        rectangle.Rect = new Rect(point, point);
                        tempPath.Data = tempGmtry = rectangle;

                        this.screenBox.Children.Add(tempPath);
                    }
                    else
                    {
                        isCreate = false;
                    }
                }
                else
                {
                    if (isCreate)
                    {
                        // 将 x,y 锁定在截图区域内
                        var x = point.X <= mgLeft ? (mgLeft + 1) : (point.X >= absWidth ? (absWidth - 1) : point.X);
                        var y = point.Y <= mgTop ? (mgTop + 1) : (point.Y >= absHeight ? (absHeight - 1) : point.Y);

                        this.point2 = new Point(x, y);
                        var myGmtry = tempGmtry as RectangleGeometry;
                        myGmtry.Rect = new Rect(this.point1, this.point2);
                    }
                }
            }
            else if (rs == RaiseElement.DrawArrow)
            {
                var mgLeft = this.cutPanelMargin.Left;
                var mgTop = this.cutPanelMargin.Top;
                var absWidth = cutPanelMargin.Left + cutPanel.Width;
                var absHeight = cutPanelMargin.Top + cutPanel.Height;

                if (state == MouseState.Down)
                {
                    // 只有在截图区域创建的绘图对象才是有效对象
                    if (point.X > mgLeft && point.X < absWidth &&
                        point.Y > mgTop && point.Y < absHeight)
                    {
                        isCreate = true;

                        this.point1 = point;
                        tempPath = new Path();
                        tempPath.Stroke = new SolidColorBrush(Colors.Red);
                        tempPath.StrokeThickness = 2;

                        var line = new LineGeometry();
                        line.StartPoint = point;
                        line.EndPoint = point;
                        tempPath.Data = tempGmtry = line;

                        this.screenBox.Children.Add(tempPath);
                    }
                    else
                    {
                        isCreate = false;
                    }
                }
                else
                {
                    if (isCreate)
                    {
                        // 将 x,y 锁定在截图区域内
                        var x = point.X <= mgLeft ? (mgLeft + 1) : (point.X >= absWidth ? (absWidth - 1) : point.X);
                        var y = point.Y <= mgTop ? (mgTop + 1) : (point.Y >= absHeight ? (absHeight - 1) : point.Y);

                        this.point2 = new Point(x, y);
                        var myGmtry = tempGmtry as LineGeometry;
                        myGmtry.EndPoint = this.point2;
                    }
                }
            }
            else
            {

                if (state == MouseState.Down)
                {
                    // 记录调整前的位置与大小
                    tempRect = new Rect(0, 0, cutPanel.Width, cutPanel.Height);
                    tempCutPanelMargin = new Thickness(cutPanelMargin.Left, cutPanelMargin.Top, cutPanelMargin.Right, cutPanelMargin.Bottom);
                }

                // 左侧
                if (leftKeys.Contains(rs))
                {
                    // value > 0, 修改左侧
                    // value < 0, 修改右侧
                    var value = tempRect.Width - (point.X - tempCutPanelMargin.Left);
                    if (value > 0)
                    {
                        this.cutPanelMargin.Left = point.X;
                        this.cutPanel.Width = value;
                        this.cutPanel.Margin = this.cutPanelMargin;
                        isExchange = true;
                    }
                    else
                    {
                        // 重置 margin-left
                        if (isExchange)
                        {
                            this.cutPanelMargin.Left = tempCutPanelMargin.Left + tempRect.Width;
                            isExchange = false;
                        }

                        this.cutPanelMargin.Right = point.X;
                        this.cutPanel.Width = Math.Abs(value);
                        this.cutPanel.Margin = this.cutPanelMargin;
                    }
                }

                // 右侧
                if (rightKeys.Contains(rs))
                {
                    if (point.X > tempCutPanelMargin.Left)
                    {
                        var moveValue = point.X - (tempCutPanelMargin.Left + tempRect.Width);

                        this.cutPanelMargin.Left = tempCutPanelMargin.Left;
                        this.cutPanel.Width = tempRect.Width + moveValue;
                        this.cutPanel.Margin = this.cutPanelMargin;
                        isExchange = true;
                    }
                    else
                    {
                        // 重置 margin-right
                        if (isExchange)
                        {
                            this.cutPanelMargin.Right = tempCutPanelMargin.Left;
                            isExchange = false;
                        }

                        var moveValue = tempCutPanelMargin.Left - point.X;
                        this.cutPanelMargin.Left = tempCutPanelMargin.Left - moveValue;
                        this.cutPanel.Width = moveValue;
                        this.cutPanel.Margin = this.cutPanelMargin;
                    }
                }

                // 顶部
                if (topKeys.Contains(rs))
                {
                    var absHeight = tempCutPanelMargin.Top + tempRect.Height;

                    if (point.Y < absHeight)
                    {
                        var poorValue = absHeight - point.Y;
                        this.cutPanelMargin.Top = absHeight - poorValue;
                        this.cutPanel.Height = poorValue;
                        this.cutPanel.Margin = this.cutPanelMargin;
                        isExchange = true;
                    }
                    else
                    {
                        // 重置 margin-top
                        if (isExchange)
                        {
                            this.cutPanelMargin.Top = absHeight;
                            isExchange = false;
                        }

                        var poorValue = point.Y - absHeight;
                        this.cutPanelMargin.Bottom = absHeight + poorValue;
                        this.cutPanel.Height = poorValue;
                        this.cutPanel.Margin = this.cutPanelMargin;
                    }
                }

                // 底部
                if (bottomKeys.Contains(rs))
                {
                    var absHeight = tempCutPanelMargin.Top + tempRect.Height;
                    if (point.Y > tempCutPanelMargin.Top)
                    {
                        var poorValue = point.Y - absHeight;

                        this.cutPanelMargin.Top = tempCutPanelMargin.Top;
                        this.cutPanelMargin.Bottom = absHeight - poorValue;
                        this.cutPanel.Height = tempRect.Height + poorValue;
                        this.cutPanel.Margin = this.cutPanelMargin;
                        isExchange = true;
                    }
                    else
                    {
                        // 重置 margin-bottom
                        if (isExchange)
                        {
                            this.cutPanelMargin.Bottom = tempCutPanelMargin.Top;
                            isExchange = false;
                        }

                        var poorValue = tempCutPanelMargin.Top - point.Y;
                        this.cutPanelMargin.Top = tempCutPanelMargin.Top - poorValue;
                        this.cutPanel.Height = poorValue;
                        this.cutPanel.Margin = this.cutPanelMargin;
                    }
                }
            }
        }

        /// <summary>
        /// 遮罩处理
        /// </summary>
        private void SetOverlayRectangle(MouseState state, RaiseElement rs)
        {
            if (rs <= (RaiseElement)100)
                this.maskControl.SetLayout(cutPanel, cutPanelMargin, rs);
        }

        /// <summary>
        /// 工具面板位置处理
        /// </summary>
        private void SetToolsPostion(MouseState state, Point point, RaiseElement rs)
        {
            if (state == MouseState.Down && rs <= (RaiseElement)100)
            {
                this.tools.Hide();
            }

            if (state == MouseState.Up && rs <= (RaiseElement)100)
            {
                // 底部剩余不足, 则显示在顶部
                // 顶部剩余不足, 则显示在内部


                // 间隙宽度
                var gapPixel = 10;
                // 剩余高度
                var residueHeight = this.Height - (this.cutPanelMargin.Top + this.cutPanel.Height);
                toolIsNeedHide = false;

                if (residueHeight > 55)
                {
                    var mLeft = this.cutPanelMargin.Left + this.cutPanel.Width - this.tools.Width;
                    var mTop = this.cutPanelMargin.Top + this.cutPanel.Height + gapPixel;
                    this.tools.Margin = new Thickness(Math.Max(mLeft, 20), mTop, 0, 0);
                }
                else
                {
                    var mLeft = this.cutPanelMargin.Left + this.cutPanel.Width - this.tools.Width;
                    var mTop = this.cutPanelMargin.Top - (_viewModel.Styles.ToolPanelHeight + gapPixel);
                    if (mTop > 5)
                    {
                        this.tools.Margin = new Thickness(Math.Max(mLeft, 20), mTop, 0, 0);
                    }
                    else
                    {
                        toolIsNeedHide = true;
                        mTop += _viewModel.Styles.ToolPanelHeight + gapPixel * 2;
                        this.tools.Margin = new Thickness(Math.Max(mLeft, 20), mTop, 0, 0);
                    }
                }

                this.tools.Show();
            }
        }






        /// <summary>
        /// 鼠标按下
        /// </summary>
        private void OnMouseDownEvent(Point point, RaiseElement rs)
        {
            // 初始化或重置参数
            this.isMouseDown = true;


            // 计算剪切面板、遮罩与工具面板的位置
            this.SetPanelRectangle(MouseState.Down, point, rs);
            this.SetOverlayRectangle(MouseState.Down, rs);
            this.SetToolsPostion(MouseState.Down, point, rs);
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
