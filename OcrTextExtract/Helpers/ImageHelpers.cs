using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OcrTextExtract.Helpers
{
    public class ImageHelpers
    {
        /// <summary>
        /// 截取一帧图片
        /// </summary>
        /// <param name="x">x坐标</param
        /// <param name="y">y坐标</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns>截屏后的位图对象，需要调用Dispose手动释放资源。</returns>
        public static Bitmap Snapshot(int x, int y, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return bitmap;
        }

        /// <summary>
        /// 屏幕截图
        /// </summary>
        public static Bitmap SnapshotScreen()
        {
            double currentGraphics = Graphics.FromHwnd(new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle).DpiX / 96;
            double screenWidth = SystemParameters.PrimaryScreenWidth * currentGraphics;
            double screenHeight = SystemParameters.PrimaryScreenHeight * currentGraphics;
            return Snapshot(0, 0, (int)screenWidth, (int)screenHeight);
        }
    }
}
