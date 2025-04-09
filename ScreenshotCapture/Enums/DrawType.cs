using System;

namespace ScreenshotCapture.Enums
{
    public enum DrawType
    {
        None,

        Range = RaiseElement.DrawRange, // 绘制矩形
        Arrow = RaiseElement.DrawArrow, // 绘制箭头
    }
}
