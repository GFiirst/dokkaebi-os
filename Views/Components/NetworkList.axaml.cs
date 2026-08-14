using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dokkaebi_os.ViewModels;

namespace dokkaebi_os.Views.Components
{
    public partial class NetworkList : UserControl
    {
        public NetworkList()
        {
            InitializeComponent();
            DataContext = new ScannerViewModel(); 
        }
    }
}
