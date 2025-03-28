using OcrTextExtract.Helpers;
using System.Drawing;
using System.Windows.Media;

namespace OcrTextExtract.ViewModels
{
    public class MaxScreenshotWindowViewModel
    {
        /// <inheritdoc />
        public MaxScreenshotWindowViewModel(Bitmap bitmap)
        {
            this.ScreenBitmap = bitmap;
            this.Styles = new ScreenshotStyles
            {
                MaskBackgroundColor = ColorHelpers.FromString("#33222222"),

                ToolBackgroundColor = Colors.AliceBlue,
                ToolPanelWidth = 200,
                ToolPanelHeight = 32,
                ToolPanelButtonHeight = 32
            };
        }

        /// <summary>
        /// 图片信息
        /// </summary>
        public Bitmap ScreenBitmap { get; set; }


        #region 样式

        /// <summary>
        /// 样式设置
        /// </summary>
        public ScreenshotStyles Styles { get; private set; }

        public void SetStyles(Action<ScreenshotStyles> value)
        {
            value?.Invoke(Styles);
        }
        #endregion


        #region 事件

        public event Action<Bitmap> OnSaveEvent = null;
        public event Action<CloseEnum> OnCancelEvent = null;


        public void OnSave(Bitmap o) => this.OnSaveEvent?.Invoke(o);

        public void OnCancel(CloseEnum o) => this.OnCancelEvent?.Invoke(o); 

        #endregion
    }

    public enum CloseEnum
    {
        CloseWindow, // 表示 操作完毕后关闭 当前窗口
        ExitApp,     // 表示 从任务管理器，状态栏 等地方关闭的窗口，则退出程序
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
    }
}
