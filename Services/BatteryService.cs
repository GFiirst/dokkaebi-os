using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class BatteryService
{
    public async Task<Battery> GetBatteryAsync()
    {
        if (OperatingSystem.IsWindows())
            return GetWindowsBattery();

        if (OperatingSystem.IsLinux())
            return await GetLinuxBatteryAsync();

        return new Battery
        {
            Status = 100
        };
    }

    private static Battery GetWindowsBattery()
    {
        if (!GetSystemPowerStatus(out var powerStatus) ||
            powerStatus.BatteryFlag == 128 ||
            powerStatus.BatteryLifePercent > 100)
        {
            return new Battery { Status = 100 };
        }

        return CreateBattery(powerStatus.BatteryLifePercent);
    }

    private static async Task<Battery> GetLinuxBatteryAsync()
    {
        const string powerSupplyPath = "/sys/class/power_supply";

        if (!Directory.Exists(powerSupplyPath))
            return new Battery { Status = 100 };

        try
        {
            var batteryPath = Directory
                .EnumerateDirectories(powerSupplyPath, "BAT*")
                .FirstOrDefault();

            if (batteryPath is null)
                return new Battery { Status = 100 };

            var capacityPath = Path.Combine(batteryPath, "capacity");

            if (!File.Exists(capacityPath))
                return new Battery { Status = 100 };

            var capacityText = await File.ReadAllTextAsync(capacityPath);

            if (!int.TryParse(
                    capacityText.Trim(),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var capacity) ||
                capacity is < 0 or > 100)
            {
                return new Battery { Status = 100 };
            }

            return CreateBattery(capacity);
        }
        catch (IOException)
        {
            return new Battery { Status = 100 };
        }
        catch (UnauthorizedAccessException)
        {
            return new Battery { Status = 100 };
        }
    }

    private static Battery CreateBattery(int percentage) => new()
    {
        Status = percentage
    };

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSystemPowerStatus(out SystemPowerStatus systemPowerStatus);

    [StructLayout(LayoutKind.Sequential)]
    private struct SystemPowerStatus
    {
        public byte AcLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte SystemStatusFlag;
        public uint BatteryLifeTime;
        public uint BatteryFullLifeTime;
    }
}
