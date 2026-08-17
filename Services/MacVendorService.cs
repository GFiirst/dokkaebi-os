using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dokkaebi_os.Services;

public class MacVendorService
{
    private readonly Dictionary<string, string> _vendors = new();

    public MacVendorService()
    {
        LoadFromCsv();
    }

    private void LoadFromCsv()
    {
        var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "mac-vendors.csv");
        
        if (!File.Exists(csvPath))
            return;

        var lines = File.ReadAllLines(csvPath);
        
        foreach (var line in lines.Skip(1))
        {
            var parts = line.Split(',');
            if (parts.Length >= 2)
            {
                var macPrefix = parts[0].Trim();
                var vendorName = parts[1].Trim().Trim('"');
                
                if (!string.IsNullOrEmpty(macPrefix) && !string.IsNullOrEmpty(vendorName))
                {
                    _vendors[macPrefix] = vendorName;
                }
            }
        }
    }

    public string? GetManufacturer(string? mac)
    {
        if (string.IsNullOrWhiteSpace(mac))
            return "???";

        var normalizedMac = mac
            .Replace("-", ":")
            .ToUpperInvariant();

        var parts = normalizedMac.Split(':');

        if (parts.Length < 3)
            return "???";

        var oui = $"{parts[0]}:{parts[1]}:{parts[2]}";

        return _vendors.TryGetValue(oui, out var manufacturer)
            ? manufacturer
            : "???";
    }
}