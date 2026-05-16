using System;

namespace Client.Core.Models;

public class Config
{
    public string Nickname { get; set; } = "bolsonaro";
    public string Version { get; set; } = "1.8.9";
    public int RamLimit { get; set; } = 4096;
}