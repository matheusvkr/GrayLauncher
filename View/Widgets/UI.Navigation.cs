using System.Numerics;
using Hexa.NET.ImGui;
using Client.View.Helpers;
using Hexa.NET.KittyUI.Input;

namespace Client.View.Widgets;

public static partial class UI
{
    public static bool TabButton(string label, bool isSelected, float borderRadius = 6f, float fSidebarWidth = 170f)
    {
        const float FixedHeight = 35f;

        var io = ImGui.GetIO();
        float dt = io.DeltaTime;
        float speed = 10f;

        float buttonWidth = fSidebarWidth - 20f;
        float buttonHeight = FixedHeight;

        var drawList = ImGui.GetWindowDrawList();
        Vector2 pos = ImGui.GetCursorScreenPos();
        Vector2 size = new(buttonWidth, buttonHeight);
        Vector2 endPos = pos + size;

        // -------------------------
        // INTERACTION
        // -------------------------
        ImGui.InvisibleButton(label, size);

        bool hovered = ImGui.IsItemHovered();
        bool clicked = ImGui.IsItemClicked();

        // -------------------------
        // ANIMATION (your framework)
        // -------------------------
        string id = $"tab_{ImGui.GetID(label)}";

        float target = isSelected ? 1f : (hovered ? 0.4f : 0f);
        float anim = Animation.Animate(id, target, speed);

        // -------------------------
        // BACKGROUND (clean modern)
        // -------------------------
        Vector4 idleBg = Color.RGBA(17f, 17f, 17f, 10f);

        Vector4 activeBg = new(
            accentColor.X,
            accentColor.Y,
            accentColor.Z,
            0.10f
        );

        Vector4 bg = Animation.Lerp(idleBg, activeBg, anim);

        drawList.AddRectFilled(
            pos,
            endPos,
            ImGui.ColorConvertFloat4ToU32(bg),
            borderRadius
        );

        // -------------------------
        // GRADIENT INDICATOR 🔥
        // -------------------------
        float barWidth = 4f * anim;

        if (barWidth > 0.01f)
        {
            Vector4 c1 = accentColor;
            Vector4 c2 = new Vector4(c1.X * 0.6f, c1.Y * 0.6f, c1.Z * 0.6f, 1f);

            drawList.AddRectFilledMultiColor(
                new Vector2(pos.X + 2f, pos.Y + 4f),
                new Vector2(pos.X + 2f + barWidth, endPos.Y - 4f),
                ImGui.ColorConvertFloat4ToU32(c1),
                ImGui.ColorConvertFloat4ToU32(c1),
                ImGui.ColorConvertFloat4ToU32(c2),
                ImGui.ColorConvertFloat4ToU32(c2)
            );
        }

        // -------------------------
        // TEXT (animated color 🔥)
        // -------------------------
        Vector4 idleText = new(1f, 1f, 1f, 0.5f);
        Vector4 activeText = new(1f, 1f, 1f, 1f);

        Vector4 textCol = Animation.Lerp(idleText, activeText, anim);

        Vector2 textSize = ImGui.CalcTextSize(label);

        Vector2 textPos = new(
            pos.X + (buttonWidth - textSize.X) * 0.2f, // change to 0.5 for centralize
            pos.Y + (buttonHeight - textSize.Y) * 0.5f
        );

        drawList.AddText(
            textPos,
            ImGui.ColorConvertFloat4ToU32(textCol),
            label
        );

        return clicked;
    }

    public static void TabLabel(string label)
    {
        Vector4 textColor = Color.RGBA(65f, 68f, 75f, 255f);
        ImGui.TextColored(textColor, label);
    }

    public static void TabHeader(string label)
    {
        // ===============================
        // Cores
        // ===============================

        Vector4 whiteColor = new Vector4(1f, 1f, 1f, 1f);

        // ===============================
        // Espaçamento e tamanhos
        // ===============================
        const float headerHeight = 40.0f;
        const float borderThickness = 1.0f;
        const float verticalPadding = 6.0f;

        // ===============================
        // Tamanho do texto
        // ===============================
        Vector2 labelSize = ImGui.CalcTextSize(label);

        // ===============================
        // Largura disponível
        // ===============================
        float availableWidth = ImGui.GetContentRegionAvail().X;

        // ===============================
        // Draw
        // ===============================
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        Vector2 min = ImGui.GetCursorScreenPos();
        Vector2 max = new Vector2(min.X + availableWidth, min.Y + headerHeight);

        // Fundo
        drawList.AddRectFilled(
            min,
            max,
            Helpers.Color.RGBA(25, 25, 25, 255),
            (float)ImGuiStyleVar.ChildRounding
        );

        // Borda inferior (accent)
        drawList.AddRectFilled(
            new Vector2(min.X, max.Y - borderThickness),
            max,
            ImGui.ColorConvertFloat4ToU32(accentColor),
            0.0f
        );

        // Texto centralizado
        Vector2 textPos = new Vector2(
            min.X + (availableWidth - labelSize.X) * 0.5f,
            min.Y + (headerHeight - labelSize.Y) * 0.5f
        );

        drawList.AddText(
            textPos,
            ImGui.ColorConvertFloat4ToU32(whiteColor),
            label
        );

        // ===============================
        // Avançar cursor
        // ===============================
        ImGui.SetCursorScreenPos(new Vector2(min.X, max.Y + verticalPadding));
        ImGui.Dummy(Vector2.Zero);
    }

    public static void GroupHeader(string label)
    {
        ImGui.TextColored(accentColor, label);
        Line(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
        ImGui.Spacing();
    }

}
