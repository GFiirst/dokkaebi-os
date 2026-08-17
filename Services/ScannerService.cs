using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace dokkaebi_os.Services;

public class ScannerService
{
    private readonly ArpService _arpService = new();
    private readonly MacVendorService _macVendorService = new();
    public async Task<List<Device>> GetDevicesAsync()
    {
        var baseIp = GetBaseIp();

        var tasks = Enumerable
            .Range(2, 254)
            .Select(async i =>
            {
                var ip = $"{baseIp}{i}";

                using var ping = new Ping();

                try
                {
                    await ping.SendPingAsync(ip, 200);
                }
                catch
                {
                }
            });

        await Task.WhenAll(tasks);

        var devices = await _arpService.GetDevicesAsync();

        foreach (var device in devices)
        {
            device.Manufacturer = _macVendorService.GetManufacturer(device.Mac);
            if (device.Manufacturer == "???")
            {
                device.Status = "???";
            }
        }

        return devices;
    }

    private string GetBaseIp()
    {
        foreach (var network in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (network.OperationalStatus != OperationalStatus.Up)
                continue;
            
            if (network.NetworkInterfaceType != NetworkInterfaceType.Ethernet &&
                network.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
            {
                continue;
            }

            foreach (var address in network.GetIPProperties().UnicastAddresses)
            {
                if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    continue;

                var ip = address.Address.ToString();
                var parts = ip.Split('.');

                return $"{parts[0]}.{parts[1]}.{parts[2]}.";
            }
        }

        throw new Exception("No IPv4 address found.");
    }
}