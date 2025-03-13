using System;
using System.Drawing;
using Tesseract;

namespace OcrTextExtract.Helpers
{
    public class OCRUtils : IDisposable
    {
        private TesseractEngine _engineForchi = null;
        private TesseractEngine _engineForeng = null;


        public string test_string(Bitmap bitmap)
        {
            if (_engineForchi == null)
            {
                _engineForchi = new TesseractEngine("tessdata-main", "chi_sim", EngineMode.Default);
            }
            using (var page = _engineForchi.Process(bitmap))
            {
                // 获取识别结果
                return page.GetText();
            }
        }
        public string test_int(Bitmap bitmap)
        {
            if (_engineForeng == null)
            {
                _engineForeng = new TesseractEngine("tessdata-main", "eng", EngineMode.Default);
            }
            using (var page = _engineForeng.Process(bitmap))
            {
                return page.GetText();
            }
        }

        /// <summary>
        /// 图片区域裁剪
        /// </summary>
        /// <param name="sourcePath">源图路径</param>
        /// <param name="x">裁剪起始坐标x</param>
        /// <param name="y">裁剪起始坐标y</param>
        /// <param name="width">裁剪区域长度</param>
        /// <param name="height">裁剪区域高度</param>
        /// <returns></returns>
        public static Bitmap RegionCropping(string sourcePath, int x, int y, int width, int height)
        {
            Bitmap result = null;
            //从文件加载原图
            using (Image originImage = Image.FromFile(sourcePath))
            {
                //创建矩形对象表示原图上裁剪的矩形区域，这里相当于划定原图上坐标为(10, 10)处，50x50大小的矩形区域为裁剪区域
                Rectangle cropRegion = new Rectangle(x, y, width, height);

                //创建空白画布，大小为裁剪区域大小
                result = new Bitmap(cropRegion.Width, cropRegion.Height);

                //创建Graphics对象，并指定要在result（目标图片画布）上绘制图像
                Graphics graphics = Graphics.FromImage(result);

                //使用Graphics对象把原图指定区域图像裁剪下来并填充进刚刚创建的空白画布
                graphics.DrawImage(originImage, new Rectangle(0, 0, cropRegion.Width, cropRegion.Height), cropRegion, GraphicsUnit.Pixel);
            }
            return result;
        }


        public void Dispose()
        {
            if (_engineForchi != null) { _engineForchi.Dispose(); }
            if (_engineForeng != null) { _engineForeng.Dispose(); }
        }

    }
}
