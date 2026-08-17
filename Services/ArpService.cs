using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace dokkaebi_os.Services;

public class ArpService
{
    public async Task<List<Device>> GetDevicesAsync()
    {
        if (OperatingSystem.IsLinux())
        {
            return await GetLinuxDevicesAsync();
        }

        if (OperatingSystem.IsWindows())
        {
            return await GetWindowsDevicesAsync();
        }

        throw new PlatformNotSupportedException(
            "Operating system not supported.");
    }

    private async Task<List<Device>> GetLinuxDevicesAsync()
    {
        var output = await ExecuteCommandAsync("ip", "neigh");

        return ParseLinuxOutput(output);
    }

    private async Task<List<Device>> GetWindowsDevicesAsync()
    {
        var output = await ExecuteCommandAsync("arp", "-a");

        return ParseWindowsOutput(output);
    }


    private async Task<string> ExecuteCommandAsync(
        string fileName,
        string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);

        if (process == null)
            return "";

        var output = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        return output;
    }

    private List<Device> ParseWindowsOutput(string output)
        {
            var devices = new List<Device>();

            foreach (var line in output.Split('\n'))
            {
                var parts = line.Split(
                    new[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    continue;
                }

                var ip = parts[0];
                var mac = parts[1];

                if (!IsIPv4(ip))
                {
                    continue;
                }

                devices.Add(new Device
                {
                    Ip = ip,
                    Mac = mac,
                    Status = "아군"
                });
            }

            return devices;
        }

   private List<Device> ParseLinuxOutput(string output)
    {
        var devices = new List<Device>();

        foreach (var line in output.Split('\n'))
        {
            var parts = line.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5)
            {
                continue;
            }

            var ip = parts[0];

            var macIndex = Array.IndexOf(parts, "lladdr");

            if (macIndex == -1)
            {
                continue;
            }

            if (macIndex + 1 >= parts.Length)
            {
                continue;
            }

            var mac = parts[macIndex + 1];

            if (!IsIPv4(ip))
            {
                continue;
            }

            devices.Add(new Device
            {
                Ip = ip,
                Mac = mac,
                Status = "아군"
            });
        }

        return devices;
    }

    private bool IsIPv4(string value)
    {
        return System.Net.IPAddress.TryParse(value, out var address)
            && address.AddressFamily ==
               System.Net.Sockets.AddressFamily.InterNetwork;
    }
}