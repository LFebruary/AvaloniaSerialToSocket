using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using SerialPortApplication.ViewModels;

namespace SerialPortApplication.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = new MainWindowViewModel(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
