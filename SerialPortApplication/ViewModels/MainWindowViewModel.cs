using System;
using System.Collections.Generic;
using System.Text;

using static SerialPortTest.SerialPortTools;
using static SerialPortTest.Extensions;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace SerialPortApplication.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            //SerialPortTest.Extensions.CatchSerialPortException(() => GetPortAndStartListening(), (string message, ConsoleAlertLevel alertLevel) => 
            //{ 
            //    Output += $"\n{message}";
            //});
        }

        private string _output = string.Empty;
        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }


    }
}
