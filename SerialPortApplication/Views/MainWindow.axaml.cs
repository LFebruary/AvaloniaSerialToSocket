// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using SerialToSocket.AvaloniaApp.ViewModels;
using static SerialToSocket.AvaloniaApp.ViewModels.MainWindowViewModel;

namespace SerialToSocket.AvaloniaApp.Views
{
    /// <summary>
    /// Represents the main window of the application.
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = new MainWindowViewModel(this);
        }

        /// <summary>
        /// Initializes Avalonia components for the MainWindow.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Handles the event when a spinner control is spun.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="spinEventArgs">The SpinEventArgs containing spin event information.</param>
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
