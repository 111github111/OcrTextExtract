using System.Drawing;
using System.Windows.Media;

namespace OcrTextExtract.ViewModels
{
    public class MaxScreenshotWindowViewModel
    {
        public Bitmap ScreenBitmap { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        public ImageSource Source { get; set; }
    }
}
