using CommunityToolkit.Mvvm.ComponentModel;

namespace dokkaebi_os.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Greeting { get; set; } = "Dokkaebi OS";
}
