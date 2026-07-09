using System.Net.NetworkInformation;

class Program
{
    static async Task Main()
    {   
       string baseIp ="10.0.0.";

       for (int i = 1; i <=254; i++)
        {
            string ip = baseIp + i;

            Console.WriteLine($"Pinging {ip}...");
            
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = await ping.SendPingAsync(ip, 1000); // 1 second timeout

                    if (reply.Status == IPStatus.Success)
                    {
                        Console.WriteLine($"Host {ip} is reachable.");
                    }
                    else
                    {
                        Console.WriteLine($"Host {ip} is not reachable. Status: {reply.Status}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error pinging {ip}: {ex.Message}");
                }
            }
        }
    }
}