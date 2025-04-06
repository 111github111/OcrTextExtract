using System;

namespace ScreenshotCapture.Enums
{
    public enum RaiseElement
    {
        CutRange = 10, // 截图矩形

        Top = 20,      // 顶部边框
        TopLeft,
        TopRight,
        Left = 30,     // 左侧边框
        LeftTop,
        LeftBottom,
        Right = 40,    // 右侧边框
        RightTop,
        RightBottom,
        Bottom = 50,   // 底部边框
        BottomLeft,
        BottomRight,

        DrawRange = 110, // 绘制矩形
        DrawArrow = 120, // 绘制箭头
    }
}
