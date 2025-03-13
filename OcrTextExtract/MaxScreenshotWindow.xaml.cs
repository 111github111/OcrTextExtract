using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OcrTextExtract.Helpers;
using OcrTextExtract.ViewModels;

namespace OcrTextExtract
{
    /// <summary>
    /// MaxScreenshotWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MaxScreenshotWindow : Window
    {
        private readonly MaxScreenshotWindowViewModel _screenshotViewModel;

        private Point point1 = new Point();
        private Point point2 = new Point();
        private StackPanel cutPanel = null;
        private Thickness cutPanelMargin = new Thickness(0, 0, 0, 0);

        // <StackPanel Width="100" Height="100"  Background="AliceBlue" HorizontalAlignment="Left" VerticalAlignment="Top" Margin="0,0,0,0"></StackPanel>

        public MaxScreenshotWindow(ref MaxScreenshotWindowViewModel viewModel)
        {
            _screenshotViewModel = viewModel;

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

            // 初始化或重置参数 
            this.IsMouseDown = true;
            this.cutPanel = ElementHelpers.CreateNewStackPanel();
            this.cutPanelMargin = new Thickness(0, 0, 0, 0);
            this.screenBox.Children.Clear();
            this.screenBox.Children.Add(cutPanel);

            this.SetPanelRectangle(MouseState.Down, e.GetPosition(this));
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
            }
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        private void MaxScreenshotWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = false;
            this.SetPanelRectangle(MouseState.Up, e.GetPosition(this));


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


        private void SetPanelRectangle(MouseState state, Point point)
        {
            // Console.WriteLine($"{ state.ToString() }, x = {point.X}, y = {point.Y}");

            if (state == MouseState.Down)
            {
                this.point1 = point;
                this.cutPanel.Margin = this.cutPanelMargin = new Thickness(this.point1.X, this.point1.Y, 0, 0);
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
