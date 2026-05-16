using System;
using System.Numerics;
using Hexa.NET.ImGui;

namespace Client.View.Helpers;

public static class Color
{
    public static Vector4 accentColor = Color.RGBA(120f, 80f, 100f, 255f);

    public static Vector4 Red = new Vector4(1, 0, 0, 1);
    public static Vector4 Green = new Vector4(0, 1, 0, 1);
    public static Vector4 Blue = new Vector4(0, 0, 1, 1);

    public static Vector4 Disabled = new Vector4(1f, 1f, 1f, 0.5f);

    public static Vector4 ColorFromHue(float h)
    {
        h %= 1f;
        h *= 6f;

        int i = (int)h;
        float f = h - i;
        float q = 1f - f;

        return i switch
        {
            0 => new Vector4(1, f, 0, 1),
            1 => new Vector4(q, 1, 0, 1),
            2 => new Vector4(0, 1, f, 1),
            3 => new Vector4(0, q, 1, 1),
            4 => new Vector4(f, 0, 1, 1),
            _ => new Vector4(1, 0, q, 1)
        };
    }

    public static Vector4 RGBA(float r, float g, float b, float a)
    {
        return new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
    }
    public static uint RGBA(int r, int g, int b, int a)
    {
        return ImGui.ColorConvertFloat4ToU32(new Vector4(r / 255f, g / 255f, b / 255f, a / 255f));
    }

}
