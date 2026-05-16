using System.Numerics;
using Hexa.NET.ImGui;
using Client.View.Helpers;

namespace Client.View.Widgets;

public static partial class UI
{

    public static bool Button(string label, Vector2 sizeArg = default)
    {
        var style = ImGui.GetStyle();
        Vector2 labelSize = ImGui.CalcTextSize(label);

        // 🔧 Igual ao CalcItemSize do ImGui
        Vector2 size = new Vector2(
            sizeArg.X > 0 ? sizeArg.X : labelSize.X + style.FramePadding.X * 2f,
            sizeArg.Y > 0 ? sizeArg.Y : labelSize.Y + style.FramePadding.Y * 2f
        );

        Vector2 pos = ImGui.GetCursorScreenPos();

        ImGui.InvisibleButton(label, size);

        bool hovered = ImGui.IsItemHovered();
        bool held = ImGui.IsItemActive();
        bool pressed = ImGui.IsItemClicked();

        var draw = ImGui.GetWindowDrawList();

        Vector2 min = pos;
        Vector2 max = pos + size;

        uint colTop = (hovered && held)
            ? ImGui.GetColorU32(ImGuiCol.ButtonActive)
            : hovered
                ? ImGui.GetColorU32(ImGuiCol.ButtonHovered)
                : ImGui.GetColorU32(ImGuiCol.Button);

        uint colBottom = ImGui.GetColorU32(ImGuiCol.FrameBg);

        draw.AddRectFilledMultiColor(
            min, max,
            colTop, colTop,
            colBottom, colBottom
        );

        uint borderColor = (hovered && held)
            ? ImGui.GetColorU32(new Vector4(0.2f, 0.5f, 1f, 1f)) // Scheme
            : ImGui.GetColorU32(ImGuiCol.BorderShadow);

        draw.AddRect(min, max, borderColor);

        Vector2 textPos = new Vector2(
            min.X + (size.X - labelSize.X) * 0.5f,
            min.Y + (size.Y - labelSize.Y) * 0.5f
        );

        uint textColor =
            (hovered && held)
                ? ImGui.GetColorU32(new Vector4(0.2f, 0.5f, 1f, 1f))
                : hovered
                    ? ImGui.GetColorU32(ImGuiCol.Text)
                    : ImGui.GetColorU32(ImGuiCol.TextDisabled);

        draw.AddText(textPos, textColor, label);

        return pressed;
    }

    public static bool WebButton(string label, Vector2 size = default)
    {
        // default height if not provided
        if (size.Y <= 0.0f) size.Y = 35.0f;

        ImGui.PushID(label);

        // --- size logic ---
        float availableWidth = ImGui.GetContentRegionAvail().X;
        float buttonWidth = (size.X <= 0.0f) ? availableWidth : size.X;
        float fixedHeight = size.Y;

        // --- interaction ---
        Vector2 screenPos = ImGui.GetCursorScreenPos();

        // we use an invisible button to capture input without default styling
        ImGui.InvisibleButton("##btn", new Vector2(buttonWidth, fixedHeight));

        bool hovered = ImGui.IsItemHovered();
        bool clicked = ImGui.IsItemClicked();

        // --- animation system ---
        // using your custom animation class
        float anim = Animation.Animate(label + "_webbtn", hovered ? 1.0f : 0.0f, 12.0f);

        // --- colors ---
        Vector4 accent = accentColor; // replace with your accent color variable
        Vector4 baseCol = accent;
        Vector4 hoverCol = new Vector4(accent.X, accent.Y, accent.Z, 0.5f);

        // interpolate color based on animation alpha
        Vector4 finalCol = Animation.Lerp(baseCol, hoverCol, anim);

        // --- draw ---
        var draw = ImGui.GetWindowDrawList();
        Vector2 min = ImGui.GetItemRectMin();
        Vector2 max = ImGui.GetItemRectMax();

        // draw background
        draw.AddRectFilled(min, max, ImGui.ColorConvertFloat4ToU32(finalCol), 4.0f);

        // centered text
        Vector2 textSize = ImGui.CalcTextSize(label);
        Vector2 textPos = new Vector2(
            min.X + (buttonWidth - textSize.X) * 0.5f,
            min.Y + (fixedHeight - textSize.Y) * 0.5f
        );

        draw.AddText(textPos, ImGui.GetColorU32(ImGuiCol.Text), label);

        ImGui.PopID();

        return clicked;
    }

    public static bool SimpleButton(string label, Vector4? hoverColor = null, bool hand = false)
    {
        ImGui.PushID(label);

        // Cor padrão (accent ou branco levemente azulado)
        Vector4 baseColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
        Vector4 hoverCol = hoverColor ?? accentColor;

        // Tamanho do texto
        Vector2 textSize = ImGui.CalcTextSize(label);
        Vector2 pos = ImGui.GetCursorScreenPos();

        // Área clicável
        ImGui.InvisibleButton("##btn", textSize);

        bool hovered = ImGui.IsItemHovered();
        bool pressed = ImGui.IsItemClicked();

        // 🔥 Fade suave
        float t = Animation.Animate(label + "_hover", hovered ? 1f : 0f, 10f);

        Vector4 finalColor = Animation.Lerp(baseColor, hoverCol, t);

        ImDrawListPtr drawList = ImGui.GetForegroundDrawList();

        drawList.AddText(
            pos,
            ImGui.ColorConvertFloat4ToU32(finalColor),
            label
        );

        // Cursor de mão (opcional mas fica nice)
        if (hovered && hand)
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

        ImGui.PopID();

        return pressed;
    }

    public static void CloseButton()
    {
        Vector2 size = ImGui.GetWindowSize();
        ImGui.SetCursorPos(new Vector2(size.X - 22, 5));
        if (SimpleButton("✖", Color.RGBA(100f, 10f, 10f, 255f)))
            Environment.Exit(0);
    }

    public static void ColorButton(string id, ref Vector4 color)
    {
        ImGui.PushID(id);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1, 1));

        float height = ImGui.GetFrameHeight();
        float width = ImGui.GetContentRegionAvail().X;
        float buttonSize = height;

        Vector2 start = ImGui.GetCursorScreenPos();

        // Botão invisível cobrindo a linha inteira
        ImGui.InvisibleButton("##line", new Vector2(width, height));
        bool clickedLine = ImGui.IsItemClicked();

        // Texto à esquerda
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        uint textColor = ImGui.GetColorU32(ImGuiCol.Text);
        drawList.AddText(start, textColor, id);

        // Botão de cor à direita
        Vector2 btnPos = new Vector2(start.X + width - buttonSize, start.Y);
        ImGui.SetCursorScreenPos(btnPos);

        bool clickedButton = ImGui.ColorButton(
            "##btn",
            color,
            ImGuiColorEditFlags.AlphaBar,
            new Vector2(buttonSize, buttonSize)
        );

        string popupId = "ColorPopup" + id;

        if (clickedLine || clickedButton)
        {
            ImGui.OpenPopup(popupId);
        }

        // 🔥 Só controla largura (altura automática evita scrollbar)
        ImGui.SetNextWindowSize(new Vector2(220, 0), ImGuiCond.Appearing);

        if (ImGui.BeginPopup(popupId))
        {

            string pickerId = "##picker" + id;

            ImGui.ColorPicker4(
                pickerId,
                ref color,
                ImGuiColorEditFlags.NoInputs |
                ImGuiColorEditFlags.AlphaBar |
                ImGuiColorEditFlags.DisplayRgb |
                ImGuiColorEditFlags.NoSidePreview |
                ImGuiColorEditFlags.NoSmallPreview |
                ImGuiColorEditFlags.NoLabel |
                ImGuiColorEditFlags.NoTooltip
            );

            ImGui.EndPopup();
        }

        ImGui.PopStyleVar();
        ImGui.PopID();
        ImGui.Spacing();
    }

    public static bool CardButton(string id, ImTextureRef image, string text = "", Vector4? accent = null, Vector2 size = default)
    {
        // 1. Configuração de Tamanho Dinâmico (Quadrado por padrão)
        float defaultSide = 120f;
        if (size == default) size = new Vector2(defaultSide, defaultSide);

        // Força o quadrado se um dos eixos for zero, ou usa o informado
        float cardWidth = size.X;
        float cardHeight = size.Y;

        Vector4 accentColor = accent ?? new Vector4(0.26f, 0.59f, 0.98f, 1.0f);
        var draw = ImGui.GetWindowDrawList();
        var pos = ImGui.GetCursorScreenPos();

        ImGui.InvisibleButton(id, size);

        bool hovered = ImGui.IsItemHovered();
        bool active = ImGui.IsItemActive();
        bool clicked = ImGui.IsItemClicked();

        // 🎬 Animações
        float hoverT = Animation.Animate(id + "_hover", hovered ? 1f : 0f, 12f);
        float pressT = Animation.Animate(id + "_press", active ? 1f : 0f, 20f);

        float scale = 1.0f + (hoverT * 0.04f) - (pressT * 0.05f);
        Vector2 center = pos + size * 0.5f;
        Vector2 scaledSize = size * scale;
        Vector2 pMin = center - scaledSize * 0.5f;
        Vector2 pMax = center + scaledSize * 0.5f;
        float rounding = 12f;

        // Cores Animadas
        uint colBg = ImGui.GetColorU32(Animation.Lerp(new Vector4(0.10f, 0.10f, 0.12f, 1f), new Vector4(0.14f, 0.14f, 0.17f, 1f), hoverT));
        uint colBorder = ImGui.GetColorU32(Animation.Lerp(new Vector4(1, 1, 1, 0.05f), accentColor, hoverT));

        // =========================
        // 🟪 Background & Sombras
        // =========================
        if (hoverT > 0.01f)
        {
            draw.AddRect(pMin + new Vector2(0, 4 * hoverT), pMax + new Vector2(0, 4 * hoverT),
                ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 0.2f * hoverT)), rounding, ImDrawFlags.None, 4.0f);
        }
        draw.AddRectFilled(pMin, pMax, colBg, rounding);
        draw.AddRect(pMin, pMax, colBorder, rounding, ImDrawFlags.None, 1.5f);

        // =========================
        // 📐 Lógica de Layout Dinâmico
        // =========================
        bool hasText = !string.IsNullOrEmpty(text);

        // Se não tem texto, o ícone ocupa 65% do card. Se tem, ocupa 40%.
        float iconMultiplier = hasText ? 0.40f : 0.65f;
        float iconDimension = Math.Min(scaledSize.X, scaledSize.Y) * iconMultiplier;
        Vector2 iconSize = new Vector2(iconDimension, iconDimension);

        if (hasText)
        {
            // Layout com Texto: Ícone em cima, Texto embaixo
            Vector2 iconPos = new Vector2(center.X - iconSize.X * 0.5f, pMin.Y + (scaledSize.Y * 0.22f));
            draw.AddImage(image, iconPos, iconPos + iconSize, Vector2.Zero, Vector2.One, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 0.8f + hoverT * 0.2f)));

            var textSize = ImGui.CalcTextSize(text);
            Vector2 textPos = new Vector2(center.X - textSize.X * 0.5f, pMax.Y - (scaledSize.Y * 0.25f));

            uint colText = ImGui.GetColorU32(Animation.Lerp(new Vector4(0.7f, 0.7f, 0.7f, 1f), new Vector4(1, 1, 1, 1f), hoverT));
            draw.AddText(textPos, colText, text);
        }
        else
        {
            // Layout sem Texto: Ícone centralizado e maior
            Vector2 iconPos = center - iconSize * 0.5f;

            // No hover, o ícone sem texto pode ganhar um leve "glow" colorido
            Vector4 tint = Animation.Lerp(new Vector4(1, 1, 1, 0.85f), accentColor with { W = 1.0f }, hoverT * 0.4f);
            draw.AddImage(image, iconPos, iconPos + iconSize, Vector2.Zero, Vector2.One, ImGui.ColorConvertFloat4ToU32(tint));
        }

        // Feedback de clique (overlay flash)
        if (pressT > 0.01f)
            draw.AddRectFilled(pMin, pMax, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, pressT * 0.06f)), rounding);

        return clicked;
    }

}
