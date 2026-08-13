using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

public class ScannerViewModel : ObservableObject
{
    public ObservableCollection<Device> Devices { get; } = new();
}