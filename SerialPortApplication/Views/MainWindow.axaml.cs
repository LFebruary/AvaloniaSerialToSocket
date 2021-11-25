using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using SerialPortApplication.ViewModels;
using static SerialPortApplication.ViewModels.MainWindowViewModel;

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

        private void Spinner_Spun(object sender, SpinEventArgs spinEventArgs)
        {
            if (sender is ButtonSpinner spinner && DataContext != null && DataContext is MainWindowViewModel vm)
            {
                switch (spinner.Name)
                {
                    case "weightStartSpinner":
                        vm.ChangeSpinnerValue(SetupSpinnerType.WeightStart, spinEventArgs.Direction);
                        break;
                    case "weightEndSpinner":
                        vm.ChangeSpinnerValue(SetupSpinnerType.WeightEnd, spinEventArgs.Direction);
                        break;
                    case "requiredLengthSpinner":
                        vm.ChangeSpinnerValue(SetupSpinnerType.RequiredLength, spinEventArgs.Direction);
                        break;
                    case "sequentialReadingsSpinner":
                        vm.ChangeSpinnerValue(SetupSpinnerType.IdenticalReadingQuantity, spinEventArgs.Direction);
                        break;
                    case "stabilityIndicatorStartSpinner":
                        vm.ChangeSpinnerValue(SetupSpinnerType.StabilityStart, spinEventArgs.Direction);
                        break;
                    case "broadcastPortSpinner":
                        vm.ChangeSpinnerValue(SetupSpinnerType.BroadcastPort, spinEventArgs.Direction);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
