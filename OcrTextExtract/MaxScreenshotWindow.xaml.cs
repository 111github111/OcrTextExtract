using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OcrTextExtract.Converters;
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

        public MaxScreenshotWindow(ref MaxScreenshotWindowViewModel viewModel)
        {
            _viewModel = viewModel;

            InitializeComponent();

            // 执行顺序: down -> move -> up
            this.PreviewMouseLeftButtonDown += MaxScreenshotWindow_PreviewMouseLeftButtonDown;
            this.PreviewMouseMove += MaxScreenshotWindow_PreviewMouseMove;
            this.PreviewMouseLeftButtonUp += MaxScreenshotWindow_PreviewMouseLeftButtonUp;

        }

        private bool IsMouseDown = false;

        /// <summary>
        /// 鼠标按下
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取鼠标相对于窗口的位置
            // var point = this.point1 = e.GetPosition(this);


            // 设置背景图片
            var brush = new ImageBrush(ImageConvert.BitmapToBitmapImage(_viewModel.ScreenBitmap));
            brush.Stretch = Stretch.None;
            this.Background = brush;


            // 初始化或重置参数 
            this.IsMouseDown = true;
            this.cutPanel = ElementHelpers.CreateNewStackPanel(Colors.Transparent);
            this.cutPanelMargin = new Thickness(0, 0, 0, 0);

            var myColor = (Color)ColorConverter.ConvertFromString("#33222222");
            this.topPanel = ElementHelpers.CreateNewStackPanel(myColor);
            this.leftPanel = ElementHelpers.CreateNewStackPanel(myColor);
            this.rightPanel = ElementHelpers.CreateNewStackPanel(myColor);
            this.bottomPanel = ElementHelpers.CreateNewStackPanel(myColor);

            this.screenBox.Children.Clear();
            this.screenBox.Background = new SolidColorBrush(Colors.Transparent);

            this.screenBox.Children.Add(this.cutPanel);

            this.screenBox.Children.Add(this.topPanel);
            this.screenBox.Children.Add(this.leftPanel);
            this.screenBox.Children.Add(this.rightPanel);
            this.screenBox.Children.Add(this.bottomPanel);

            this.SetPanelRectangle(MouseState.Down, e.GetPosition(this));
            this.SetOverlayRectangle(MouseState.Down, e.GetPosition(this));
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // 按下鼠标后, 移动操作才属于有效操作
            if (IsMouseDown)
            {
                this.SetPanelRectangle(MouseState.Move, e.GetPosition(this));
                this.SetOverlayRectangle(MouseState.Move, e.GetPosition(this));
            }
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = false;
            this.SetPanelRectangle(MouseState.Up, e.GetPosition(this));
            this.SetOverlayRectangle(MouseState.Up, e.GetPosition(this));


            // if (!IsMouseUp)
            // {
            //     WrapPanel_Btns.Visibility = Visibility.Visible;
            // 
            //     //当所选的截图区域不大时，按钮区直接在其下方显示
            //     if (Rect_RealScreen.Y + Rect_RealScreen.Height + this.WrapPanel_Btns.ActualHeight < SystemParameters.PrimaryScreenHeight)
            //     {
            //         Canvas.SetRight(this.WrapPanel_Btns, Rectangle_Right.Width);
            //         Canvas.SetBottom(this.WrapPanel_Btns, Rectangle_Bottom.Height - this.WrapPanel_Btns.ActualHeight - 4);
            //     }
            //     else //当鼠标选择区域大到一定程度时，设置按钮选择区的位置到选择区域内左上角
            //     {
            //         Canvas.SetLeft(this.WrapPanel_Btns, Rect_RealScreen.X + 4);
            //         Canvas.SetTop(this.WrapPanel_Btns, Rect_RealScreen.Y + 4);
            //     }
            // 
            //     IsMouseUp = true;
            // }
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
