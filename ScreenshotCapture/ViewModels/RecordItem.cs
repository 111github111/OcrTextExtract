using System;
using System.Windows.Shapes;

namespace ScreenshotCapture.ViewModels
{
    /// <summary>
    /// 记录项
    /// </summary>
    public class RecordItem
    {
        public RecordItem(Path path)
        {
            this.Path = path;
        }

        public RecordItem(Path path, Polygon polyon) : this(path)
        {
            this.Polyon = polyon;
        }


        public Path Path { get; set; }
        public Polygon Polyon { get; set; }



        public Shape[] GetShales()
        {
            if (Polyon != null)
                return new Shape[] { Path, Polyon };

            return new Shape[] { Path };
        }
    }
}
