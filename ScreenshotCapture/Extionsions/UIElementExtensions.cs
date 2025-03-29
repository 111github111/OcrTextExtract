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



        /// <summary>
        /// ms > 0, 隐藏并延时, 反之无操作
        /// </summary>
        public static Task HideDelayMs(this UIElement element, int ms)
        {
            if (ms > 0)
            {
                element.Visibility = Visibility.Hidden;
                return Task.Delay(ms);
            }
            return Task.CompletedTask;
        }
    }
}
