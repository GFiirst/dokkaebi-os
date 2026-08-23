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
            .Range(1, 254)
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

        var devices = await _arpService.GetDevicesAsync(baseIp);

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
        try
        {
            using var socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);

            socket.Connect(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 65530));

            if (socket.LocalEndPoint is IPEndPoint localEndPoint)
            {
                return Get24BaseIp(localEndPoint.Address);
            }
        }
        catch (SocketException)
        {

        }

        foreach (var network in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (network.OperationalStatus != OperationalStatus.Up)
                continue;
            
            if (network.NetworkInterfaceType != NetworkInterfaceType.Ethernet &&
                network.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
            {
                continue;
            }

            var properties = network.GetIPProperties();
            var hasIPv4Gateway = properties.GatewayAddresses.Any(gateway =>
                gateway.Address.AddressFamily == AddressFamily.InterNetwork &&
                !gateway.Address.Equals(IPAddress.Any));

            if (!hasIPv4Gateway)
                continue;

            foreach (var address in properties.UnicastAddresses)
            {
                if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    continue;

                return Get24BaseIp(address.Address);
            }
        }

        throw new Exception("No IPv4 address found.");
    }

    private static string Get24BaseIp(IPAddress address)
    {
        var bytes = address.GetAddressBytes();
        return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.";
    }
}
