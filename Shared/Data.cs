using System;
using Client.Core.Models;
using Client.Core.Services;

namespace Client.Shared;

public static class Data
{
    public static User User { get; set; } = new();
    public static Config Config { get; set; } = new();
    public static ServerConfig ServerConfig { get; set; } = new();

    public static string LaunchState { get; set; } = "Idle";
    public static string ServerState { get; set; } = "Idle";
    public static bool IsServerOn { get; set; } = false;
    public static bool IsLaunching { get; set; } = false;
}