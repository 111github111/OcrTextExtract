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
        }

        public Bitmap ScreenBitmap { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        public ImageSource Source { get; set; }



        public event Action<Bitmap> OnSaveEvent = null;
        public event Action<CloseEnum> OnCancelEvent = null;


        public void OnSave(Bitmap o) => this.OnSaveEvent?.Invoke(o);
        public void OnCancel(CloseEnum o) => this.OnCancelEvent?.Invoke(o);
    }

    public enum CloseEnum
    {
        CloseWindow, // 表示 操作完毕后关闭 当前窗口
        ExitApp,     // 表示 从任务管理器，状态栏 等地方关闭的窗口，则退出程序
    }
}
