using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SerialPortApplication.ViewModels;
using SerialPortApplication.Views;
using System.Collections.Specialized;

namespace SerialPortApplication
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            CustomSettings.SetSetting(CustomSettings.StringCollectionSetting.CollectionOfReceivedValues, new StringCollection());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
