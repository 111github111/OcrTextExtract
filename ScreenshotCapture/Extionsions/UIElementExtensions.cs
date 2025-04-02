using System.Windows;

namespace ScreenshotCapture.Extionsions
{
    public static class UIElementExtensions
    {
        /// <summary>
        /// 显示元素
        /// </summary>
        public static void Show(this UIElement element) => element.Visibility = Visibility.Visible;

        /// <summary>
        /// 不现实元素，但保留空间
        /// </summary>
        public static void Hide(this UIElement element) => element.Visibility = Visibility.Hidden;

    }
}
