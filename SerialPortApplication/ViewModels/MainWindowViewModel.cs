using System;
using System.Collections.Generic;
using System.Text;

using static SerialPortTest.SerialPortTools;
using static SerialPortTest.Extensions;
using System.Runtime.CompilerServices;
using ReactiveUI;
using SerialPortTest;
using System.Linq;
using static SerialPortApplication.CustomSettings;

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
            SelectedParity = GetParityOptionFromString(GetSetting(StringSetting.Parity));
        }

        private string _output = string.Empty;
        public string Output
        {
            get => _output;
            set => SetProperty(ref _output, value);
        }

        private static IEnumerable<SerialPortWrapper> SerialPorts => SerialPortWrapper.GetSerialPorts();

        private IEnumerable<string> _comPorts = SerialPorts.Select(i => i.SerialPortID);
        public IEnumerable<string> ComPorts
        {
            get => _comPorts;
            set => SetProperty(ref _comPorts, value);
        }

        private string _selectedComPort = GetSetting(StringSetting.ComPort);
        public string SelectedComPort
        {
            get => _selectedComPort;
            set => SetProperty(ref _selectedComPort, value, () => {
                OnPropertyChanged(nameof(SelectSerialPort));
                CustomSettings.SetSetting(StringSetting.ComPort, SelectedComPort.Trim());
            });
        }
        public SerialPortWrapper? SelectSerialPort => string.IsNullOrEmpty(SelectedComPort) 
            ? null 
            : SerialPorts.FirstOrDefault(i => i.SerialPortID == SelectedComPort);


        private int _selectedBaudRate = GetSetting(IntSetting.BaudRate);
        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set => SetProperty(ref _selectedBaudRate, value, () => 
            { 
                CustomSettings.SetSetting(IntSetting.BaudRate, SelectedBaudRate); 
            });
        }

        private IEnumerable<int> _baudRates = SerialPortTools.BaudRates;
        public IEnumerable<int> BaudRates
        {
            get => _baudRates;
            set => SetProperty(ref _baudRates, value);
        }

        private int _selectedDataBits = GetSetting(IntSetting.Databits);
        public int SelectedDataBits
        {
            get => _selectedDataBits;
            set => SetProperty(ref _selectedDataBits, value, () => 
            { 
                CustomSettings.SetSetting(IntSetting.Databits, SelectedDataBits); 
            });
        }

        private IEnumerable<int> _dataBits = SerialPortTools.DataBits;
        public IEnumerable<int> DataBits
        {
            get => _dataBits;
            set => SetProperty(ref _dataBits, value);
        }

        private int _selectedStopBits = GetSetting(IntSetting.Stopbits);
        public int SelectedStopBits
        {
            get => _selectedStopBits;
            set => SetProperty(ref _selectedStopBits, value, () => 
            {
                CustomSettings.SetSetting(IntSetting.Stopbits, SelectedStopBits); 
            });
        }

        private IEnumerable<int> _stopBits = SerialPortTools.StopBits;
        public IEnumerable<int> StopBits
        {
            get => _stopBits;
            set => SetProperty(ref _stopBits, value);
        }

        #region Parity
        private ParityOption _selectedParity;

        private static ParityOption GetParityOptionFromString(string parityString)
        {
            return parityString switch
            {
                EvenParity  => ParityOption.Even,
                OddParity   => ParityOption.Odd,
                NoParity    => ParityOption.None,
                _           => ParityOption.None,
            };
        }

        public ParityOption SelectedParity
        {
            get => _selectedParity;
            set => SetProperty(ref _selectedParity, value, () => {
                switch (_selectedParity)
                { 
                    case ParityOption.Even: 
                        EvenParityChecked   = true;
                        OddParityChecked    = false;
                        NoneParityChecked   = false;
                        CustomSettings.SetSetting(StringSetting.Parity, EvenParity);
                        break; 
                    case ParityOption.Odd:
                        EvenParityChecked   = false;
                        OddParityChecked    = true;
                        NoneParityChecked   = false;
                        CustomSettings.SetSetting(StringSetting.Parity, OddParity);
                        break; 
                    case ParityOption.None:
                        EvenParityChecked   = false;
                        OddParityChecked    = false;
                        NoneParityChecked   = true;
                        CustomSettings.SetSetting(StringSetting.Parity, NoParity);
                        break; 
                }
            });
        }

        private bool _evenParityChecked;
        public bool EvenParityChecked
        {
            get => _evenParityChecked;
            set => SetProperty(ref _evenParityChecked, value);
        }

        private bool _oddParityChecked;
        public bool OddParityChecked
        {
            get => _oddParityChecked;
            set => SetProperty(ref _oddParityChecked, value);
        }

        private bool _noneParityChecked;
        public bool NoneParityChecked
        {
            get => _noneParityChecked;
            set => SetProperty(ref _noneParityChecked, value);
        }
        #endregion

        #region Scale stability
        private bool _stabilityIndicatorActive = GetSetting(BoolSetting.StabilityIndicatorActive);
        public bool StabilityIndicatorActive
        {
            get => _stabilityIndicatorActive;
            set => SetProperty(ref _stabilityIndicatorActive, value, () =>
            {
                CustomSettings.SetSetting(BoolSetting.SequenceOfIdenticalReadingsActive, !value);
                CustomSettings.SetSetting(BoolSetting.StabilityIndicatorActive, value);

                SequenceOfIdenticalReadingsActive = !value;
            });
        }

        private bool _sequenceOfIdenticalReadingsActive = GetSetting(BoolSetting.SequenceOfIdenticalReadingsActive);
        public bool SequenceOfIdenticalReadingsActive
        {
            get => _sequenceOfIdenticalReadingsActive;
            set => SetProperty(ref _sequenceOfIdenticalReadingsActive, value, () => 
            { 
                CustomSettings.SetSetting(BoolSetting.StabilityIndicatorActive, !value);
                CustomSettings.SetSetting(BoolSetting.SequenceOfIdenticalReadingsActive, value);

                StabilityIndicatorActive = !value;
            });
        }

        private string _stabilityIndicatorSnippet = GetSetting(StringSetting.StabilityIndicatorSnippet);
        public string StabilityIndicatorSnippet
        {
            get => _stabilityIndicatorSnippet;
            set => SetProperty(ref _stabilityIndicatorSnippet, value, () => 
            {
                CustomSettings.SetSetting(StringSetting.StabilityIndicatorSnippet, value); 
            });
        }

        private int _stabilityIndicatorStartingPosition = GetSetting(IntSetting.StabilityIndicatorStartPosition);
        public int StabilityIndicatorStartingPosition
        {
            get => _stabilityIndicatorStartingPosition;
            set => SetProperty(ref _stabilityIndicatorStartingPosition, value, () => 
            { 
                CustomSettings.SetSetting(IntSetting.StabilityIndicatorStartPosition, StabilityIndicatorStartingPosition);  
            });
        }

        private int _numberOfIdenticalReadings = GetSetting(IntSetting.IdenticalReadingQuantity);
        public int NumberOfIdenticalReadings
        {
            get => _numberOfIdenticalReadings;
            set => SetProperty(ref _numberOfIdenticalReadings, value, () =>
            {
                CustomSettings.SetSetting(IntSetting.IdenticalReadingQuantity, NumberOfIdenticalReadings);
            });
        }
        #endregion

        #region Scale string settings
        private int _weightStartPosition = GetSetting(IntSetting.ScaleStringWeightStartPosition);
        public int WeightStartPosition
        {
            get => _weightStartPosition;
            set => SetProperty(ref _weightStartPosition, value, () => 
            {
                CustomSettings.SetSetting(IntSetting.ScaleStringWeightStartPosition, WeightStartPosition);
            });
        }

        private int _weightEndPosition = GetSetting(IntSetting.ScaleStringWeightEndPosition);
        public int WeightEndPosition
        {
            get => _weightEndPosition;
            set => SetProperty(ref _weightEndPosition, value, () =>
            {
                CustomSettings.SetSetting(IntSetting.ScaleStringWeightEndPosition, WeightEndPosition);
            });
        }

        private bool _stringRequiredLengthActive = GetSetting(BoolSetting.ScaleStringRequiredLength);
        public bool StringRequiredLengthActive
        {
            get => _stringRequiredLengthActive;
            set => SetProperty(ref _stringRequiredLengthActive, value, () =>
            {
                CustomSettings.SetSetting(BoolSetting.ScaleStringRequiredLength, StringRequiredLengthActive);
            });
        }

        private int _scaleStringRequiredLength = GetSetting(IntSetting.ScaleStringRequiredLength);
        public int ScaleStringRequiredLength
        {
            get => _scaleStringRequiredLength;
            set => SetProperty(ref _scaleStringRequiredLength, value, () =>
            {
                CustomSettings.SetSetting(IntSetting.ScaleStringRequiredLength, ScaleStringRequiredLength);
            });
        }
        #endregion
    }
}
