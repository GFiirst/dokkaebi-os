using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace dokkaebi_os.Services;

public class ScannerService
{
    public async Task<List<Device>> GetDevicesAsync()
    {
        var baseIp = GetBaseIp();

        var tasks = Enumerable
            .Range(2, 253)
            .Select(async i =>
            {
                var ip = $"{baseIp}{i}";

                using var ping = new Ping();

                try
                {
                    var reply = await ping.SendPingAsync(ip, 100);

                    if (reply.Status == IPStatus.Success)
                    {
                        return new Device
                        {
                            Ip = ip
                        };
                    }
                }
                catch
                {

                }

                return null;
            });

        var results = await Task.WhenAll(tasks);

        return results
            .Where(device => device is not null)
            .Cast<Device>()
            .ToList();
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

        throw new Exception("Nenhum endereço IPv4 encontrado.");
    }
}