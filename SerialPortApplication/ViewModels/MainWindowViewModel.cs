using System;
using System.Collections.Generic;
using System.Text;

using static SerialPortTest.SerialPortTools;
using static SerialPortTest.Extensions;
using System.Runtime.CompilerServices;
using ReactiveUI;
using SerialPortTest;
using System.Linq;

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

        private static IEnumerable<SerialPortWrapper> SerialPorts => SerialPortWrapper.GetSerialPorts();

        private IEnumerable<string> _comPorts = SerialPorts.Select(i => i.SerialPortID);
        public IEnumerable<string> ComPorts
        {
            get => _comPorts;
            set => this.RaiseAndSetIfChanged(ref _comPorts, value);
        }

        private string _selectedComPort = string.Empty;
        public string SelectedComPort
        {
            get => _selectedComPort;
            set => this.RaiseAndSetIfChanged(ref _selectedComPort, value, () => {
                this.RaisePropertyChanged(nameof(SelectSerialPort));
            });
        }
        public SerialPortWrapper? SelectSerialPort => string.IsNullOrEmpty(SelectedComPort) 
            ? null 
            : SerialPorts.FirstOrDefault(i => i.SerialPortID == SelectedComPort);

        private void RaiseAndSetIfChanged<T>(ref T selectedComPort, T value, Action p)
        {
            this.RaiseAndSetIfChanged(ref selectedComPort, value);
            p.Invoke();
        }

        private int _selectedBaudRate = -1;
        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set => this.RaiseAndSetIfChanged(ref _selectedBaudRate, value);
        }

        private IEnumerable<int> _baudRates = SerialPortTools.BaudRates;
        public IEnumerable<int> BaudRates
        {
            get => _baudRates;
            set => this.RaiseAndSetIfChanged(ref _baudRates, value);
        }

        private int _selectedDataBits = -1;
        public int SelectedDataBits
        {
            get => _selectedDataBits;
            set => this.RaiseAndSetIfChanged(ref _selectedDataBits, value);
        }

        private IEnumerable<int> _dataBits = SerialPortTools.DataBits;
        public IEnumerable<int> DataBits
        {
            get => _dataBits;
            set => this.RaiseAndSetIfChanged(ref _dataBits, value);
        }

        private int _selectedStopBits = -1;
        public int SelectedStopBits
        {
            get => _selectedStopBits;
            set => this.RaiseAndSetIfChanged(ref _selectedStopBits, value);
        }

        private IEnumerable<int> _stopBits = SerialPortTools.StopBits;
        public IEnumerable<int> StopBits
        {
            get => _stopBits;
            set => this.RaiseAndSetIfChanged(ref _stopBits, value);
        }
    }
}
