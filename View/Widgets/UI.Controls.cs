using System.Numerics;
using Hexa.NET.ImGui;
using Client.View.Helpers;

namespace Client.View.Widgets;

public static partial class UI
{
    public static bool Toggle(string label, ref bool value)
    {
        ImGui.PushID(label);

        float height = 20f;
        float toggleWidth = 38f; // largura fixa para o switch para manter consistência
        float width = ImGui.GetContentRegionAvail().X;

        Vector2 start = ImGui.GetCursorScreenPos();
        Vector2 p = new Vector2(start.X + width - toggleWidth, start.Y + (height * 0.5f) - 9f);

        // área de clique invisível
        ImGui.InvisibleButton("##line", new Vector2(width, height));

        bool clicked = ImGui.IsItemClicked();
        bool hovered = ImGui.IsItemHovered();
        if (clicked) value = !value;

        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        // animações
        float target = value ? 1.0f : 0.0f;
        float t = Animation.Animate(label + "_toggle", target, 12.0f); // animação do switch
        float h = Animation.Animate(label + "_hover", hovered ? 1.0f : 0.0f, 10.0f); // animação de hover

        // ===== colors (usando seu Color.RGBA) =====
        uint bgOff = Color.RGBA(30, 30, 30, 255);
        uint bgOn = ImGui.ColorConvertFloat4ToU32(accentColor);
        uint knobCol = Color.RGBA(255, 255, 255, 255);
        uint textCol = ImGui.ColorConvertFloat4ToU32(Animation.Lerp(new Vector4(1, 1, 1, 0.5f), new Vector4(1, 1, 1, 1f), t));

        // ===== label =====
        drawList.AddText(new Vector2(start.X, start.Y + (height - ImGui.GetFontSize()) * 0.5f), textCol, label);

        // ===== background (track) =====
        // interpolar a cor do bg entre cinza escuro e o seu accent
        Vector4 mixedBg = Animation.Lerp(ImGui.ColorConvertU32ToFloat4(bgOff), ImGui.ColorConvertU32ToFloat4(bgOn), t);

        // efeito de brilho sutil no hover
        if (h > 0.01f)
        {
            mixedBg.W = Math.Clamp(mixedBg.W + (h * 0.2f), 0f, 1f);
        }

        drawList.AddRectFilled(p, p + new Vector2(toggleWidth, 18f), ImGui.ColorConvertFloat4ToU32(mixedBg), 10f);

        // ===== knob (círculo) =====
        float radius = (hovered || value) ? 7f : 6f; // knob cresce sutilmente no hover ou on
        radius = Animation.Lerp(6f, radius, h);

        // cálculo do movimento do knob
        float knobStart = p.X + 9f;
        float knobEnd = p.X + toggleWidth - 9f;
        float knobX = Animation.Lerp(knobStart, knobEnd, t);
        float knobY = p.Y + 9f;

        // sombra sutil sob o knob (opcional para profundidade)
        drawList.AddCircleFilled(new Vector2(knobX, knobY + 1f), radius, Color.RGBA(0, 0, 0, 50));

        // desenho do knob principal
        drawList.AddCircleFilled(new Vector2(knobX, knobY), radius, knobCol);

        ImGui.PopID();
        return value;
    }

    public static void Slider(string label, ref float value, float min, float max)
    {
        Vector4 accent = accentColor;

        ImGuiIOPtr io = ImGui.GetIO();
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        ImGui.PushID(label);

        float fullWidth = ImGui.GetContentRegionAvail().X;
        Vector2 start = ImGui.GetCursorScreenPos();

        // =========================
        // LABEL
        // =========================

        ImGui.TextColored(new Vector4(1f, 1f, 1f, 1f), label);

        float valueAnim = Animation.Animate(label + "_value", value, 12f);

        string text = valueAnim.ToString("0");
        Vector2 textSize = ImGui.CalcTextSize(text);

        drawList.AddText(
            new Vector2(start.X + fullWidth - textSize.X, start.Y),
            ImGui.ColorConvertFloat4ToU32(new Vector4(0.55f, 0.55f, 0.55f, 1f)),
            text
        );

        // =========================
        // SLIDER
        // =========================

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4);

        float sliderH = 6.0f;
        float knobR = 6.5f;

        ImGui.InvisibleButton(
            "##slider",
            new Vector2(fullWidth, sliderH + knobR * 2)
        );

        bool hovered = ImGui.IsItemHovered();
        bool active = ImGui.IsItemActive();

        Vector2 bbMin = ImGui.GetItemRectMin();
        Vector2 bbMax = ImGui.GetItemRectMax();

        float minX = bbMin.X;
        float maxX = bbMax.X;
        float trackY = bbMin.Y + knobR;

        float width = maxX - minX;

        if (max <= min)
            max = min + 1.0f;

        float target = Math.Clamp((value - min) / (max - min), 0.0f, 1.0f);

        // =========================
        // ANIMAÇÕES (SEU SISTEMA)
        // =========================

        float anim = Animation.Animate(label + "_anim", target, 14f);
        float hoverAnim = Animation.Animate(label + "_hover", hovered ? 1f : 0f, 10f);

        float knobX = minX + knobR + anim * (width - knobR * 2.0f);

        // =========================
        // TRACK
        // =========================

        drawList.AddRectFilled(
            new Vector2(minX, trackY - sliderH * 0.5f),
            new Vector2(maxX, trackY + sliderH * 0.5f),
            ImGui.ColorConvertFloat4ToU32(new Vector4(45f / 255f, 45f / 255f, 45f / 255f, 0.86f)),
            3.0f
        );

        Vector4 fill = Animation.Lerp(accent, new Vector4(1, 1, 1, 1), hoverAnim * 0.2f);

        drawList.AddRectFilled(
            new Vector2(minX, trackY - sliderH * 0.5f),
            new Vector2(knobX, trackY + sliderH * 0.5f),
            ImGui.ColorConvertFloat4ToU32(fill),
            3.0f
        );

        // =========================
        // KNOB
        // =========================

        float knobScale = 1.0f + (hoverAnim * 0.25f);

        drawList.AddCircleFilled(
            new Vector2(knobX, trackY),
            knobR * knobScale,
            ImGui.ColorConvertFloat4ToU32(accent),
            24
        );

        // glow
        drawList.AddCircleFilled(
            new Vector2(knobX, trackY),
            knobR * knobScale * 1.8f,
            ImGui.ColorConvertFloat4ToU32(new Vector4(accent.X, accent.Y, accent.Z, 0.15f)),
            24
        );

        // =========================
        // DRAG
        // =========================

        if (active && io.MouseDown[0])
        {
            float mx = Math.Clamp(io.MousePos.X, minX, maxX);
            float nt = (mx - minX) / width;
            value = min + (max - min) * nt;
        }

        ImGui.PopID();
    }

    public static bool Combo(string label, ref int currentItem, IReadOnlyList<string> items)
    {
        bool changed = false;

        ImGui.PushStyleVarY(ImGuiStyleVar.ItemSpacing, 4f);

        ImGui.BeginGroup();

        ImGui.TextColored(new Vector4(1f, 1f, 1f, 0.5f), label);

        ImGui.SameLine();

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 2f);

        ImGui.SetNextItemWidth(-1);

        if (ImGui.BeginCombo("##" + label, items[currentItem]))
        {
            for (int i = 0; i < items.Count; i++)
            {
                bool selected = currentItem == i;

                if (ImGui.Selectable(items[i], selected))
                {
                    currentItem = i;
                    changed = true;
                }

                if (selected)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }

        ImGui.PopStyleVar();
        ImGui.EndGroup();


        return changed;
    }

    public static void Input(string label, ref string buf, bool password = false)
    {
        ImGui.PushID(label);

        // cores bases
        uint textCol = Color.RGBA(230, 230, 230, 255);
        uint placeholderCol = Color.RGBA(100, 100, 100, 255);
        uint bgCol = Color.RGBA(25, 25, 25, 255);

        // estilo do campo
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 10));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, bgCol);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, Color.RGBA(35, 35, 35, 255));
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, bgCol);
        ImGui.PushStyleColor(ImGuiCol.Text, textCol);

        ImGui.SetNextItemWidth(-1f);
        ImGuiInputTextFlags flags = password ? ImGuiInputTextFlags.Password : ImGuiInputTextFlags.None;

        // o id do input precisa ser diferente da label visível para não dar conflito
        ImGui.InputText("##" + label, ref buf, 256, flags);

        // captura os estados para o desenho
        bool active = ImGui.IsItemActive();
        var min = ImGui.GetItemRectMin();
        var max = ImGui.GetItemRectMax();
        var draw = ImGui.GetWindowDrawList();

        // === Lógica do Placeholder ===
        // só desenha se o buffer estiver vazio e o campo NÃO estiver com foco 
        // (ou se quiser que suma só ao digitar, tire o !active)
        if (string.IsNullOrEmpty(buf))
        {
            Vector2 placeholderPos = new Vector2(min.X + 10f, min.Y + 10f);
            draw.AddText(placeholderPos, placeholderCol, label);
        }

        // === Underline Animado (Igual ao anterior, pois é muito bom) ===
        float uAnim = Animation.Animate(label + "_u", active ? 1.0f : 0.0f, 14.0f);

        // linha de fundo fixa
        draw.AddLine(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y), Color.RGBA(255, 255, 255, 15), 1f);

        // linha ativa expandindo
        if (uAnim > 0.01f)
        {
            float center = (min.X + max.X) * 0.5f;
            float halfWidth = ((max.X - min.X) * 0.5f) * uAnim;
            draw.AddLine(
                new Vector2(center - halfWidth, max.Y),
                new Vector2(center + halfWidth, max.Y),
                ImGui.ColorConvertFloat4ToU32(accentColor),
                1.5f
            );
        }

        ImGui.PopStyleColor(4);
        ImGui.PopStyleVar(2);
        ImGui.PopID();
    }

}
