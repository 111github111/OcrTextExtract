using System;
using System.Windows.Media;

namespace ScreenshotCapture.Helpers
{
    public class ColorHelpers
    {
        public static Color FromString(string colorString)
        {
            return (Color)ColorConverter.ConvertFromString(colorString);
        }
    }
}
