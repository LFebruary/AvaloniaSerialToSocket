// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using SerialToSocket.AvaloniaApp.ViewModels;

namespace SerialToSocket.AvaloniaApp
{
    public class ViewLocator : IDataTemplate
    {
        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }

        Control? ITemplate<object?, Control?>.Build(object? param)
        {
            if (param is null)
                return new TextBlock { Text = "Not Found: Builder param not specified" };

            string name = param.GetType().FullName!.Replace("ViewModel", "View");

            Type? type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }
    }
}
