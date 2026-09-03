using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dokkaebi_os.ViewModels;

namespace dokkaebi_os.Views.Components
{
    public partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            InitializeComponent();
            DataContext = new BatteryViewModel();
        }
    }
}
