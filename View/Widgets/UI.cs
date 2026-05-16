using System.Numerics;
using Hexa.NET.ImGui;
using Client.View.Helpers;

namespace Client.View.Widgets;

public static partial class UI
{
    private static Vector4 accentColor = Color.RGBA(120f, 80f, 100f, 255f);

    public static void ApplyStyle()
    {
        var style = ImGui.GetStyle();

        // =====================================================
        // Core colors (dark theme with blue accent)
        // =====================================================

        Vector4 backgroundColor = Color.RGBA(23f, 23f, 23f, 255f);
        Vector4 childColor = Vector4.Zero;
        Vector4 textColor = Color.RGBA(242f, 245f, 250f, 255f);
        Vector4 borderColor = Color.RGBA(30f, 30f, 30f, 255f);

        // =====================================================
        // Colors configuration
        // =====================================================

        var colors = style.Colors;

        colors[(int)ImGuiCol.ChildBg] = childColor;
        colors[(int)ImGuiCol.WindowBg] = backgroundColor;
        colors[(int)ImGuiCol.Border] = borderColor;
        colors[(int)ImGuiCol.Text] = textColor;

        colors[(int)ImGuiCol.TitleBg] = accentColor;
        colors[(int)ImGuiCol.TitleBgActive] = accentColor;

        colors[(int)ImGuiCol.CheckMark] = accentColor;

        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.15f, 0.15f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.20f, 0.20f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.25f, 0.25f, 0.30f, 1.00f);

        colors[(int)ImGuiCol.Button] = new Vector4(0.15f, 0.15f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.20f, 0.20f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.25f, 0.25f, 0.30f, 1.00f);

        colors[(int)ImGuiCol.Separator] = new Vector4(1f, 1f, 1f, 1f);

        colors[(int)ImGuiCol.Header] = accentColor;
        colors[(int)ImGuiCol.HeaderHovered] = accentColor;
        colors[(int)ImGuiCol.HeaderActive] = accentColor;

        colors[(int)ImGuiCol.BorderShadow] = Color.RGBA(0f, 0f, 0f, 0f);

        // =====================================================
        // Roundings
        // =====================================================

        style.WindowRounding = 5f;
        style.ChildRounding = 5f;
        style.FrameRounding = 4f;
        style.GrabRounding = 2f;
        style.PopupRounding = 2f;
        style.ScrollbarRounding = 4f;

        style.ScrollbarSize = 9f;
        style.FramePadding = new Vector2(6, 3);
        style.ItemSpacing = new Vector2(4, 4);

        // =====================================================
        // Spacing and padding
        // =====================================================

        style.WindowPadding = new Vector2(12, 12);
        style.FramePadding = new Vector2(8, 4);
        style.ItemSpacing = new Vector2(8, 10);
        style.ItemInnerSpacing = new Vector2(4, 4);
        style.TouchExtraPadding = new Vector2(0, 0);
        style.IndentSpacing = 21f;

        style.ScrollbarSize = 6f;
        style.GrabMinSize = 10f;

        // =====================================================
        // Borders
        // =====================================================

        style.WindowBorderSize = 0f;
        style.ChildBorderSize = 1f;
        style.PopupBorderSize = 1f;
        style.FrameBorderSize = 0f;
        style.TabBorderSize = 0f;

        // =====================================================
        // Misc
        // =====================================================

        style.AntiAliasedLines = true;
        style.AntiAliasedFill = true;
    }

    public static void TopBorder(Vector4 color)
    {
        var draw = ImGui.GetForegroundDrawList(); // Foreground olmalı
        var pos = ImGui.GetWindowPos();
        var size = ImGui.GetWindowSize();

        draw.AddLine(
        new Vector2(pos.X, pos.Y),              // sol üst köşe
        new Vector2(pos.X + size.X, pos.Y),      // sağ üst köşe
        ImGui.GetColorU32(color), // TURUNCU
        1f // kalınlık
        );
    }
    public static void Line(Vector4 color, float thickness = 1.0f)
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        Vector2 pos = ImGui.GetCursorScreenPos();
        float width = ImGui.GetContentRegionAvail().X;

        drawList.AddLine(
            pos,
            new Vector2(pos.X + width, pos.Y),
            ImGui.ColorConvertFloat4ToU32(color),
            thickness
        );

        ImGui.Dummy(new Vector2(0, thickness + 2.0f));
    }
    public static void RgbLine()
    {
        var drawList = ImGui.GetWindowDrawList();

        // Obtém a posição da janela na tela
        Vector2 windowPos = ImGui.GetWindowPos();

        // Largura da janela
        float width = ImGui.GetWindowSize().X; // 6 para compensar a borda

        // Define a altura da linha (em pixels)
        float lineHeight = 1f;

        // Define as coordenadas (relativas à tela, não à janela)
        Vector2 min = new Vector2(windowPos.X, windowPos.Y);
        Vector2 max = new Vector2(windowPos.X + width, windowPos.Y + lineHeight);

        float t = (float)ImGui.GetTime() * 0.7f;
        Vector4 rgb = Color.ColorFromHue(t % 1f);

        // Cor da linha (azul com transparência)
        Vector4 col = new Vector4(0f, 0.5f, 1.0f, 0.8f);

        // Line
        Vector4 cL = Color.ColorFromHue((t % 1f));
        Vector4 cR = Color.ColorFromHue((t + 0.3f) % 1f);

        drawList.AddRectFilledMultiColor(
            new Vector2(min.X, min.Y),
            new Vector2(max.X, min.Y + 1f),
            ImGui.ColorConvertFloat4ToU32(cL),
            ImGui.ColorConvertFloat4ToU32(cR),
            ImGui.ColorConvertFloat4ToU32(cR),
            ImGui.ColorConvertFloat4ToU32(cL)
        );

        ImGui.Dummy(Vector2.Zero);
    }

}
