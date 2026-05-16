using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Widgets;
using Client.View.Widgets;
using Client.Shared;
using Client.Core.Services;

namespace Client.View.Windows;

public class MainWindow : ImWindow
{
    //dotnet publish Client.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
    // gui vars
    static bool test = false;
    static int curTab = 1;
    static string usernameTemp = string.Empty;
    static string passTemp = string.Empty;
    static bool autoRunTemp = Data.ServerConfig.AutoRun;
    static string[] mversions = { "1.7.10", "1.8.9", "1.12.2", "1.16.5", "1.20.4" };
    static string[] ramlimits = { "1024", "2048", "3072", "4096", "6144" };
    static string nickInput = string.Empty;
    static int versionIndex = 1;
    static int limitIndex = 3;
    static string? foundServer = null;
    static bool isScanning = false;

    public MainWindow()
    {
        IsEmbedded = true;
    }

    public override string Name => "Main Window";

    public override void DrawContent()
    {
        UI.ApplyStyle();
        ImGui.BeginChild("Sidebar"u8, new Vector2(170, 0), ImGuiChildFlags.Borders);

        if (UI.TabButton("\uf045  Home", curTab == 1)) curTab = 1;
        if (UI.TabButton("\uf022  Server", curTab == 2)) curTab = 2;
        if (UI.TabButton("\uf033  Mods", curTab == 3)) curTab = 3;
        if (UI.TabButton("\uf013  Settings", curTab == 4)) curTab = 4;

        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginChild("Content"u8);

        switch (curTab)
        {
            case 1: HomePage(); break;
            case 2: ServerPage(); break;
            case 3: ModsPage(); break;
            case 4: ConfigPage(); break;
            default: ImGui.Text("[ERROR] Unknown error."u8); break;
        }

        ImGui.EndChild();
    }

    static void HomePage()
    {
        ImGui.BeginChild("Card1"u8, new Vector2(300, 400), ImGuiChildFlags.Borders);
        UI.TabHeader("Launcher");

        UI.Input("Nickname", ref nickInput);
        UI.Combo("Minecraft version", ref versionIndex, mversions);
        UI.Combo("Ram limit (mb)", ref limitIndex, ramlimits);

        // state label
        ImGui.Text($"State: "); ImGui.SameLine();
        ImGui.TextColored(Data.IsLaunching ? Helpers.Color.Green : Helpers.Color.Disabled, Data.LaunchState);

        if (Data.IsLaunching)
        {
            ImGui.BeginDisabled();
        }

        if (UI.WebButton("Launch"))
        {
            if (!string.IsNullOrEmpty(nickInput))
                Data.Config.Nickname = nickInput;

            Data.Config.Version = mversions[versionIndex];
            Data.Config.RamLimit = int.Parse(ramlimits[limitIndex]);

            Task.Run(async () =>
            {
                try
                {
                    await MinecraftService.LaunchAsync();
                }
                catch (Exception ex)
                {
                    Data.LaunchState = ex.ToString();
                }
            });
        }

        if (Data.IsLaunching)
        {
            ImGui.EndDisabled();
        }

        ImGui.EndChild();
    }

    static void ServerPage()
    {
        ImGui.BeginChild("Card2"u8, new Vector2(300, 400), ImGuiChildFlags.Borders);
        UI.TabHeader("Local server");

        ImGui.Text($"Address: {ServerService.GetServerAddress()}");

        ImGui.SameLine();
        if (UI.Button("Copy"))
            Hexa.NET.KittyUI.Windows.Clipboard.SetClipboardText(ServerService.GetServerAddress());

        UI.Toggle("Auto run", ref autoRunTemp);
        Data.ServerConfig.AutoRun = autoRunTemp;

        ImGui.Text("State: ");
        ImGui.SameLine();

        ImGui.TextColored(Data.IsServerOn
            ? Helpers.Color.Green
            : Helpers.Color.Disabled,
        Data.ServerState);

        if (UI.WebButton(Data.IsServerOn ? "Stop" : "Start"))
        {
            if (!Data.IsServerOn)
                ServerService.StartServer();
            else
                ServerService.StopServer();
        }

        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginChild("Card3"u8, new Vector2(300, 400), ImGuiChildFlags.Borders);
        UI.TabHeader("Server list (LAN)");

        if (UI.WebButton(isScanning ? "Scanning..." : "Scan"))
        {
            if (!isScanning)
            {
                isScanning = true;
                foundServer = null;

                Task.Run(async () =>
                {
                    foundServer = await ServerService.FindServerAsync();
                    isScanning = false;
                });
            }
        }

        UI.Line(Helpers.Color.accentColor);

        if (isScanning)
        {
            ImGui.TextDisabled("Scanning network...");
        }
        else if (foundServer != null)
        {
            ImGui.TextColored(
                Helpers.Color.Green,
                $"[ONLINE] {foundServer}");

            ImGui.SameLine();
            if (UI.Button("Copy"))
                Hexa.NET.KittyUI.Windows.Clipboard.SetClipboardText(foundServer);
        }
        else
        {
            ImGui.TextDisabled("No server found.");
        }

        ImGui.EndChild();
    }

    static void ModsPage()
    {
        ImGui.Text("Under development. Available soon!"u8);
    }

    static void ConfigPage()
    {
        UI.BeginGroupCenteredTranslucent("Auth", 300f, 300f);
        {
            UI.Input("Username", ref usernameTemp);
            UI.Input("Passwordword", ref passTemp, true);
            UI.Toggle("Remember me", ref test);
            UI.WebButton("Sign in");
        }
        UI.EndGroup();
    }
}
