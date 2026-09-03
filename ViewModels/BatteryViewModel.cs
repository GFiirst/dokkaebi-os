using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace dokkaebi_os.ViewModels;

public partial class BatteryViewModel : ViewModelBase
{
    private readonly BatteryService _batteryService = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BatteryFillWidth))]
    private int status = 100;

    public double BatteryFillWidth => Math.Clamp(Status, 0, 100) / 100d * 16d;

    public BatteryViewModel()
    {
        _ = UpdatePeriodicallyAsync();
    }

    public async Task InitializeAsync()
    {
        if (IsLoading)
            return;

        IsLoading = true;

        try
        {
            var battery = await _batteryService.GetBatteryAsync();
            Status = battery.Status;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task UpdatePeriodicallyAsync()
    {
        await InitializeAsync();

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync())
            await InitializeAsync();
    }
}
