using System;
using System.Numerics;
using Hexa.NET.ImGui;

namespace Client.View.Helpers;

public static class Animation
{
    public static readonly Dictionary<string, float> Animations = new();
    public static float Animate(string id, float target, float speed = 8f)
    {
        if (!Animations.TryGetValue(id, out float value))
            value = target;

        value = Lerp(value, target, ImGui.GetIO().DeltaTime * speed);
        Animations[id] = value;

        return value;
    }
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * Math.Clamp(t, 0f, 1f);
    }
    public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new Vector4(
            a.X + (b.X - a.X) * t,
            a.Y + (b.Y - a.Y) * t,
            a.Z + (b.Z - a.Z) * t,
            a.W + (b.W - a.W) * t
        );
    }

}
