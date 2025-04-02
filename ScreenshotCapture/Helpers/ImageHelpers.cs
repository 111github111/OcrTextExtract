using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ScreenshotCapture.Helpers
{
    public static class ImageHelpers
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

        public static Bitmap Snapshot(System.Windows.Point p1, System.Windows.Point p2)
        {
            double currentGraphics = Graphics.FromHwnd(new WindowInteropHelper(Application.Current.MainWindow).Handle).DpiX / 96;
            int x1 = (int)(p1.X * currentGraphics);
            int y1 = (int)(p1.Y * currentGraphics);
            int x2 = (int)(p2.X * currentGraphics);
            int y2 = (int)(p2.Y * currentGraphics);
            return Snapshot(x1, y1, x2, y2);

            // return Snapshot((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y);
        }


        /// <summary>
        /// 屏幕截图
        /// </summary>
        public static Bitmap SnapshotScreen()
        {
            double currentGraphics = Graphics.FromHwnd(new WindowInteropHelper(Application.Current.MainWindow).Handle).DpiX / 96;
            double screenWidth = SystemParameters.PrimaryScreenWidth * currentGraphics;
            double screenHeight = SystemParameters.PrimaryScreenHeight * currentGraphics;
            return Snapshot(0, 0, (int)screenWidth, (int)screenHeight);
        }
    }
}
