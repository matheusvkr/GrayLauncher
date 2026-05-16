using System.Numerics;
using Hexa.NET.ImGui;
using Client.View.Helpers;

namespace Client.View.Widgets;

public static partial class UI
{
    public static int groupBoxIndex = 0;
    const float GROUPBOX_HEIGHT = 220;
    const float GROUPBOX_HEADER = 28f;

    public static void BeginGroup(string title)
    {
        var style = ImGui.GetStyle();

        float avail = ImGui.GetContentRegionAvail().X;
        float spacing = style.ItemSpacing.X;
        float width = (avail - spacing) * 0.5f;

        if (groupBoxIndex % 2 != 0)
            ImGui.SameLine();

        ImGui.PushStyleVarY(ImGuiStyleVar.ItemSpacing, 4f);
        ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.RGBA(18, 18, 18, 150));

        // text top
        ImGui.TextColored(Helpers.Color.Blue, title);

        ImGui.BeginChild("##groupbox_" + title,
            new Vector2(width, GROUPBOX_HEIGHT));

        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        Vector2 pos = ImGui.GetWindowPos();
        Vector2 size = ImGui.GetWindowSize();

        // HEADER
        draw.AddRectFilledMultiColor(
            pos,
            new Vector2(pos.X + size.X, pos.Y + GROUPBOX_HEADER),
            ImGui.ColorConvertFloat4ToU32(new Vector4(accentColor.X, accentColor.Y, accentColor.Z, 0.4f)),
            ImGui.ColorConvertFloat4ToU32(new Vector4(accentColor.X, accentColor.Y, accentColor.Z, 0.0f)),
            ImGui.ColorConvertFloat4ToU32(new Vector4(accentColor.X, accentColor.Y, accentColor.Z, 0.0f)),
            ImGui.ColorConvertFloat4ToU32(new Vector4(accentColor.X, accentColor.Y, accentColor.Z, 0.1f))
        );

        // TEXTO CENTRALIZADO
        Vector2 textSize = ImGui.CalcTextSize(title);
        Vector2 textPos = new Vector2(
            pos.X + (size.X - textSize.X) * 0.5f,
            pos.Y + (GROUPBOX_HEADER - textSize.Y) * 0.5f
        );

        draw.AddText(textPos, ImGui.GetColorU32(ImGuiCol.Text), title);

        // Padding interno (ESSENCIAL)
        float padding = style.WindowPadding.X;

        ImGui.SetCursorPos(new Vector2(padding, GROUPBOX_HEADER + style.ItemSpacing.Y));

        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
        // child interno com padding padrão
        ImGui.BeginChild("##content_" + title, new Vector2(-padding, -padding));

        groupBoxIndex++;
    }

    public static void BeginGroupCentered(string title, float width = 300f, float height = 450f)
    {
        var style = ImGui.GetStyle();

        // 1. Cálculo de Centralização Real
        // Usamos ContentRegionAvail para pegar o espaço útil da janela atual
        float availWidth = ImGui.GetContentRegionAvail().X;
        float availHeight = ImGui.GetContentRegionAvail().Y;

        float offX = (availWidth - width) * 0.5f;
        float offY = (availHeight - height) * 0.5f;

        // Garante que não seja negativo se a janela for menor que o grupo
        if (offX < 0) offX = 0;
        if (offY < 0) offY = 0;

        // Define a posição do cursor para começar o desenho
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offX);
        // Remova a linha abaixo se quiser centralizar apenas horizontalmente
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + offY);

        ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.RGBA(18, 18, 18, 150));
        // 2. Início do Child Principal
        ImGui.BeginChild("##groupbox_" + title, new Vector2(width, height));

        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        Vector2 pos = ImGui.GetCursorScreenPos(); // CursorScreenPos é mais preciso para o DrawList
        Vector2 size = new Vector2(width, height);

        // 3. HEADER (Fundo)
        uint colTop = ImGui.ColorConvertFloat4ToU32(new Vector4(accentColor.X, accentColor.Y, accentColor.Z, 0.4f));
        uint colBot = ImGui.ColorConvertFloat4ToU32(new Vector4(accentColor.X, accentColor.Y, accentColor.Z, 0.05f));

        draw.AddRectFilledMultiColor(
            pos,
            new Vector2(pos.X + size.X, pos.Y + GROUPBOX_HEADER),
            colTop, colTop, colBot, colBot
        );

        // 4. TEXTO CENTRALIZADO NO HEADER
        Vector2 textSize = ImGui.CalcTextSize(title);
        Vector2 textPos = new Vector2(
            pos.X + (size.X - textSize.X) * 0.5f,
            pos.Y + (GROUPBOX_HEADER - textSize.Y) * 0.5f
        );
        draw.AddText(textPos, ImGui.GetColorU32(ImGuiCol.Text), title);

        // 5. CONTEÚDO INTERNO
        float padding = style.WindowPadding.X + 20;
        ImGui.SetCursorScreenPos(new Vector2(pos.X + padding, pos.Y + GROUPBOX_HEADER + style.ItemSpacing.Y));

        ImGui.PopStyleColor();
        // Child interno para o conteúdo (usa -1 para preencher até o fim do pai com padding)
        ImGui.BeginChild("##content_" + title, new Vector2(size.X - (padding * 2), size.Y - GROUPBOX_HEADER - (padding * 2)));
    }

    public static void BeginGroupCenteredTranslucent(string title, float width = 300f, float height = 450f)
    {
        var style = ImGui.GetStyle();

        // 1. Cálculo de Centralização Real
        // Usamos ContentRegionAvail para pegar o espaço útil da janela atual
        float availWidth = ImGui.GetContentRegionAvail().X;
        float availHeight = ImGui.GetContentRegionAvail().Y;

        float offX = (availWidth - width) * 0.5f;
        float offY = (availHeight - height) * 0.5f;

        // Garante que não seja negativo se a janela for menor que o grupo
        if (offX < 0) offX = 0;
        if (offY < 0) offY = 0;

        // Define a posição do cursor para começar o desenho
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offX);
        // Remova a linha abaixo se quiser centralizar apenas horizontalmente
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + offY);

        // style
        ImGui.PushStyleColor(ImGuiCol.ChildBg, Vector4.Zero);

        // 2. Início do Child Principal
        ImGui.BeginChild("##groupbox_" + title, new Vector2(width, height));

        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        Vector2 pos = ImGui.GetCursorScreenPos(); // CursorScreenPos é mais preciso para o DrawList
        Vector2 size = new Vector2(width, height);

        // 3. HEADER (Fundo)
        uint colTop = ImGui.ColorConvertFloat4ToU32(new Vector4(accentColor.X, accentColor.Y, accentColor.Z, 0f));
        uint colBot = ImGui.ColorConvertFloat4ToU32(new Vector4(accentColor.X, accentColor.Y, accentColor.Z, 0f));

        draw.AddRectFilledMultiColor(
            pos,
            new Vector2(pos.X + size.X, pos.Y + GROUPBOX_HEADER),
            colTop, colTop, colBot, colBot
        );

        // 4. TEXTO CENTRALIZADO NO HEADER
        Vector2 textSize = ImGui.CalcTextSize(title);
        Vector2 textPos = new Vector2(
            pos.X + (size.X - textSize.X) * 0.5f,
            pos.Y + (GROUPBOX_HEADER - textSize.Y) * 0.5f
        );
        draw.AddText(textPos, ImGui.GetColorU32(ImGuiCol.Text), title);

        // 5. CONTEÚDO INTERNO
        float padding = style.WindowPadding.X + 20;
        ImGui.SetCursorScreenPos(new Vector2(pos.X + padding, pos.Y + GROUPBOX_HEADER + style.ItemSpacing.Y));

        ImGui.PopStyleColor();

        // Child interno para o conteúdo (usa -1 para preencher até o fim do pai com padding)
        ImGui.BeginChild("##content_" + title, new Vector2(size.X - (padding * 2), size.Y - GROUPBOX_HEADER - (padding * 2)));
    }

    public static void EndGroup()
    {
        ImGui.EndChild(); // content
        ImGui.EndChild(); // groupbox
    }

}
