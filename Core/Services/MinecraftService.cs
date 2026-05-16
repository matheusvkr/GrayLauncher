using Client.Shared;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;

namespace Client.Core.Services;

public static class MinecraftService
{
    static MinecraftLauncher MLauncher = new MinecraftLauncher();
    static HashSet<string>? CachedVersions;

    public static async Task LaunchAsync()
    {
        if (Data.IsLaunching)
            return;

        Data.IsLaunching = true;

        try
        {
            Data.LaunchState = "Building process...";

            var gameInstance = await MLauncher.BuildProcessAsync(
                Data.Config.Version,
                GetLaunchOptions());

            Data.LaunchState = "Launching Minecraft...";

            // Server auto run
            if (Data.ServerConfig.AutoRun && Data.ServerState != "Running...")
            {
                Data.LaunchState = "Starting server...";
                ServerService.StartServer();
            }

            gameInstance.Start();

            Data.LaunchState = "Minecraft started.";
        }
        catch (DirectoryNotFoundException)
        {
            try
            {
                Data.LaunchState =
                    $"Installing Minecraft {Data.Config.Version}...";

                var gameInstance =
                    await MLauncher.InstallAndBuildProcessAsync(
                        Data.Config.Version,
                        GetLaunchOptions());

                Data.LaunchState = "Launching Minecraft...";

                gameInstance.Start();

                Data.LaunchState = "Minecraft started.";
            }
            catch (Exception ex)
            {
                Data.LaunchState = $"Install error: {ex.Message}";
            }
        }
        catch (Exception ex)
        {
            Data.LaunchState = $"Launch error: {ex.Message}";
        }

        Data.IsLaunching = false;
    }

    static MLaunchOption GetLaunchOptions()
    {
        return new MLaunchOption
        {
            Session = MSession.CreateOfflineSession(
                Data.Config.Nickname),

            MaximumRamMb = Data.Config.RamLimit
        };
    }

    public static async Task<HashSet<string>> GetVersionNamesAsync()
    {
        if (CachedVersions != null)
            return CachedVersions;

        var versions = await MLauncher.GetAllVersionsAsync();

        CachedVersions = versions
            .Select(v => v.Name)
            .ToHashSet();

        return CachedVersions;
    }

    static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };

        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}