using System;

namespace Client.Core.Models;

public class ServerConfig
{
    public bool AutoRun { get; set; } = false;
    public string IP { get; set; } = string.Empty;
    public string Port { get; set; } = "25565";
}