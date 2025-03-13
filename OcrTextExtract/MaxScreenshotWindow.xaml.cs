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

namespace OcrTextExtract
{
    /// <summary>
    /// MaxScreenshotWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MaxScreenshotWindow : Window
    {
        public MaxScreenshotWindow()
        {
            InitializeComponent();

            this.MouseLeftButtonUp += MaxScreenshotWindow_MouseLeftButtonUp;
        }

        private bool IsMouseUp = false;

        private void MaxScreenshotWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
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
    }
}
