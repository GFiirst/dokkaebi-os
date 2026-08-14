using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace dokkaebi_os.Services;

public class ScannerService
{
    public async Task<List<Device>> GetDevicesAsync()
    {
        var baseIp ="10.0.0.";

        var devices = new List<Device>();

        for(int i = 2; i <=254; i++)
        {
            var ip = baseIp + i;

            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = await ping.SendPingAsync(ip, 20);

                     if (reply.Status == IPStatus.Success)
                    {
                        devices.Add(new Device{ Ip = ip});
                    }
                }
                catch{}
            }
        }

        return devices;
    }
}