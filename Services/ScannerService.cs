using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace dokkaebi_os.Services;

public class ScannerService
{
    public async Task<List<Device>> GetDevicesAsync()
    {
        const string baseIp = "10.0.0.";

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
}