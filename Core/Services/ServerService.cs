using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Client.Shared;

namespace Client.Core.Services;

public static class ServerService
{
    private static Process? _serverProcess;

    public static void StartServer()
    {
        try
        {
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                Data.ServerState = "Server already running.";
                Data.IsServerOn = true;
                return;
            }

            string userPath = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile);

            string serverPath = Path.Combine(
                userPath,
                "Downloads",
                "server-lan",
                "start.bat"
            );

            if (!File.Exists(serverPath))
            {
                Data.ServerState = "Server file not found.";
                Data.IsServerOn = false;
                return;
            }

            ProcessStartInfo startInfo = new()
            {
                FileName = serverPath,
                WorkingDirectory =
                    Path.GetDirectoryName(serverPath) ?? userPath,

                UseShellExecute = true,
                CreateNoWindow = false
            };

            _serverProcess = Process.Start(startInfo);

            if (_serverProcess != null)
            {
                Data.ServerState = "Running...";
                Data.IsServerOn = true;
            }
            else
            {
                Data.ServerState = "Failed to start server.";
                Data.IsServerOn = false;
            }
        }
        catch (Win32Exception)
        {
            Data.ServerState = "Permission denied.";
            Data.IsServerOn = false;
        }
        catch (Exception ex)
        {
            Data.ServerState = ex.Message;
            Data.IsServerOn = false;
        }
    }

    public static void StopServer()
    {
        try
        {
            if (_serverProcess == null || _serverProcess.HasExited)
            {
                Data.ServerState = "Server is not running.";
                Data.IsServerOn = false;
                return;
            }

            _serverProcess.Kill(true);
            _serverProcess.WaitForExit();

            Data.ServerState = "Stopped.";
            Data.IsServerOn = false;

            _serverProcess.Dispose();
            _serverProcess = null;
        }
        catch (Exception ex)
        {
            Data.ServerState = ex.Message;
        }
    }

    public static string GetLocalIP()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up)
                continue;

            if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                continue;

            IPInterfaceProperties ipProps = ni.GetIPProperties();

            foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
            {
                if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    string ip = addr.Address.ToString();

                    // filtra IPs LAN comuns
                    if (ip.StartsWith("192.168.") ||
                        ip.StartsWith("10.") ||
                        ip.StartsWith("172."))
                    {
                        return ip;
                    }
                }
            }
        }

        return "127.0.0.1";
    }

    public static string GetServerAddress()
    {
        return $"{GetLocalIP()}:{Data.ServerConfig.Port}";
    }

    public static async Task<string?> FindServerAsync(int port = 25565, int timeout = 250)
    {
        string localIp = GetLocalIP();

        string[] parts = localIp.Split('.');

        if (parts.Length != 4)
            return null;

        string subnet = $"{parts[0]}.{parts[1]}.{parts[2]}";

        List<Task<string?>> tasks = [];

        for (int i = 1; i <= 254; i++)
        {
            string ip = $"{subnet}.{i}";

            tasks.Add(CheckServerAsync(ip, port, timeout));
        }

        while (tasks.Count > 0)
        {
            Task<string?> finished = await Task.WhenAny(tasks);

            tasks.Remove(finished);

            string? result = await finished;

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static async Task<string?> CheckServerAsync(string ip, int port, int timeout)
    {
        using TcpClient client = new();

        try
        {
            CancellationTokenSource cts = new(timeout);

            await client.ConnectAsync(
                IPAddress.Parse(ip),
                port,
                cts.Token);

            return $"{ip}:{port}";
        }
        catch
        {
            return null;
        }
    }

}
