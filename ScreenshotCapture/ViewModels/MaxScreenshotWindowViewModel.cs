using ScreenshotCapture.Helpers;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace ScreenshotCapture.ViewModels
{
    public class MaxScreenshotWindowViewModel
    {
        /// <inheritdoc />
        public MaxScreenshotWindowViewModel()
        {
            this.Styles = new ScreenshotStyles
            {
                MaskBackgroundColor = ColorHelpers.FromString("#33222222"),
                
                LineColor = ColorHelpers.FromString("#2080F0"),

                ToolBackgroundColor = Colors.AliceBlue,
                ToolPanelWidth = 200,
                ToolPanelHeight = 32,
                ToolPanelButtonHeight = 32,
            };
        }


        private MaxScreenshotWindow ScreenshotWindow { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        internal Bitmap ScreenBitmap { get; set; }
        

        #region 样式

        /// <summary>
        /// 样式设置
        /// </summary>
        internal ScreenshotStyles Styles { get; private set; }

        public void SetStyles(Action<ScreenshotStyles> value)
        {
            value?.Invoke(Styles);
        }
        #endregion


        #region 事件

        public event Action<Bitmap> OnSaveEvent = null;
        public event Action<CloseEnum> OnCancelEvent = null;
        public event Action OnSaveAsEvent = null;


        internal void OnSave(Bitmap o) => this.OnSaveEvent?.Invoke(o);

        internal void OnCancel(CloseEnum o) => this.OnCancelEvent?.Invoke(o);
        internal void OnSaveAs() => this.OnSaveAsEvent?.Invoke();

        #endregion

        /// <summary>
        /// 1. 截取全屏 <br />
        /// 2. 将截取的图像设置为图像处理窗口的背景图片, 然后进行区域截取处理
        /// </summary>
        public void ShowCapture()
        {
            // 获取屏幕信息
            this.ScreenBitmap = ImageHelpers.SnapshotScreen();

            // 创建截图窗口
            this.ScreenshotWindow = new MaxScreenshotWindow(this);
            this.ScreenshotWindow.Show();
        }
    }


    public enum CloseEnum
    {
        CloseWindow, // 表示 操作完毕后关闭 当前窗口
        ExitApp,     // 表示 从任务管理器，状态栏 等地方关闭的窗口，则退出程序
    }

    /// <summary>
    /// 工作状态枚举
    /// </summary>
    public enum WorkStateEnum
    {
        Capture, // 截图
        Move,    // 移动
        Draw,    // 涂鸦
    }

    /// <summary>
    /// 截图样式
    /// </summary>
    public class ScreenshotStyles
    {
        /// <summary>
        /// 遮罩背景颜色
        /// </summary>
        public System.Windows.Media.Color MaskBackgroundColor { get; set; }

        /// <summary>
        /// 工具面板背景颜色
        /// </summary>
        public System.Windows.Media.Color ToolBackgroundColor { get; set; }

        public int ToolPanelWidth { get; set; }
        public int ToolPanelHeight { get; set; }
        public int ToolPanelButtonHeight { get; set; }


        /// <summary>
        /// 线条颜色
        /// </summary>
        public System.Windows.Media.Color LineColor { get; set; }
    }
}
