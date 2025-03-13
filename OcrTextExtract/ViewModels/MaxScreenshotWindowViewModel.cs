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
        public event Action<object> OnCancelEvent = null;


        public void OnSave(Bitmap o) => this.OnSaveEvent?.Invoke(o);
        public void OnCancel(object o) => this.OnCancelEvent?.Invoke(o);
    }
}
