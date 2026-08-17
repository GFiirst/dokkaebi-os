using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using dokkaebi_os.Services;

namespace dokkaebi_os.ViewModels;
public partial class ScannerViewModel : ObservableObject
{
    private readonly ScannerService _scannerService = new();

    public ObservableCollection<Device> Devices { get; } = new();

    [ObservableProperty]
    private bool isLoading;

    public ScannerViewModel()
    {
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        if (IsLoading)
            return;

        IsLoading = true;

        try
        {
            Devices.Clear();

            var devices = await _scannerService.GetDevicesAsync();

            foreach (var device in devices)
            {
                Devices.Add(device);
                await Task.Delay(100); 
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}