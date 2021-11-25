using System;
using System.Collections.Generic;
using ReactiveUI;
using System.Linq;
using static SerialPortApplication.CustomSettings;
using System.Reactive;
using System.IO.Ports;
using SerialPortApplication.Views;
using System.Threading.Tasks;
using static SerialPortApplication.Views.MessageBox;
using System.Diagnostics;

namespace SerialPortApplication.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(MainWindow window): base(window)
        {
            
            //SerialPortTest.Extensions.CatchSerialPortException(() => GetPortAndStartListening(), (string message, ConsoleAlertLevel alertLevel) => 
            //{ 
            //    Output += $"\n{message}";
            //});
            SelectedParity = GetParityOptionFromString(GetSetting(StringSetting.Parity));
            //ListenOnSerialPortCommand = ReactiveCommand.Create(async () =>
            //{
            //    async Task InvalidDataToStartListeningAsync(string message)
            //    {
            //        _ = await Show(parentView, message, "Error", MessageBoxButtons.Ok);
            //        return;
            //    }

            //    var tempValue = !ListeningOnSerialPort;
            //    if (tempValue)
            //    {
            //        if (string.IsNullOrWhiteSpace(SelectedComPort))
            //        {
            //            await InvalidDataToStartListeningAsync("Selected port is blank");
            //        }
            //        else if (SerialPort.GetPortNames().Contains(SelectedComPort))
            //        {
            //            await InvalidDataToStartListeningAsync("Selected port does not exist in ports associated with computer");
            //        }
            //        else if (SelectedBaudRate <= 0 || BaudRates.Contains(SelectedBaudRate) == false)
            //        {
            //            MessageBoxResult result = await Show(parentView, "An invalid baud rate has been specified. Is this correct?", "Error", MessageBoxButtons.YesNo);
            //            if (result == MessageBoxResult.No)
            //            {
            //                return;
            //            }
            //        }
            //        else if (SelectedDataBits <= 0 || DataBits.Contains(SelectedDataBits) == false)
            //        {
            //            await InvalidDataToStartListeningAsync("An invalid databits value has been specified. Reselect databits and try again");
            //        }
            //        else if (SelectedStopBits <= 0 || StopBits.Contains(SelectedStopBits) == false)
            //        {
            //            await InvalidDataToStartListeningAsync("An invalid stop bits value has been specified. Reselect stop bits and try again");
            //        }
            //        else if (SelectedParity == null)
            //        {
            //            await InvalidDataToStartListeningAsync("No parity selected");
            //        }
            //        else if ((StabilityIndicatorActive == false && SequenceOfIdenticalReadingsActive == false)
            //        || (StabilityIndicatorActive && SequenceOfIdenticalReadingsActive))
            //        {
            //            await InvalidDataToStartListeningAsync("Somehow both the stability indicator and sequence of identical readers got the same selection. Reselect one of the options and try again");
            //        }
            //        else if (StabilityIndicatorActive && string.IsNullOrWhiteSpace(StabilityIndicatorSnippet))
            //        {
            //            MessageBoxResult result = await Show(parentView, "Stability indicator active, but no character snippet provided. Is this correct?", "Error", MessageBoxButtons.YesNo);
            //            if (result == MessageBoxResult.No)
            //            {
            //                return;
            //            }
            //        }
            //        else if (StabilityIndicatorStartingPosition <= 0)
            //        {
            //            await InvalidDataToStartListeningAsync("Stability indicator can not start at position lower than one.");
            //        }
            //        else if (SequenceOfIdenticalReadingsActive && (NumberOfIdenticalReadings <= 0))
            //        {
            //            await InvalidDataToStartListeningAsync("Number of readings for sequence of identical readings can not be lower than one.");
            //        }
            //        else if (WeightStartPosition < 0 || WeightEndPosition < 0)
            //        {
            //            await InvalidDataToStartListeningAsync("Scale string settings have to indications positions greater than or equal to zero.");
            //        }
            //        else if ((TakeFullWeightString == false) && (WeightStartPosition != WeightEndPosition) && (WeightEndPosition <= WeightStartPosition))
            //        {
            //            await InvalidDataToStartListeningAsync("Scale string's end position can not be less than starting position.");
            //        }
            //        else if (StringRequiredLengthActive && (ScaleStringRequiredLength <= 0))
            //        {
            //            await InvalidDataToStartListeningAsync("Scale string required length setting requires a length greater than zero");
            //        }


            //        ListeningOnSerialPort = tempValue;
            //        if (ListeningOnSerialPort)
            //        {
            //            Extensions.CatchSerialPortException(() => SerialPortTools.GetPortAndStartListening(), (_, _) => { });
            //        }
            //        else
            //        {
            //            Extensions.CatchSerialPortException(() => SerialPortTools.StopListeningOnPort(), (_, _) => { });
            //        }
                    
            //    }
            //});

            BroadcastSerialValuesCommand = ReactiveCommand.Create(() =>
            {
                if (ListeningOnSerialPort)
                {
                    SocketTools.StartServer();
                }
            });
        }

        public async void ListenOnSerialPortCommand()
        {
            async Task InvalidDataToStartListeningAsync(string message)
            {
                _ = await Show(parentView, message, "Error", MessageBoxButtons.Ok);
            }

            var tempValue = !ListeningOnSerialPort;
            if (tempValue)
            {
               
                if (string.IsNullOrWhiteSpace(SelectedComPort))
                {
                    await InvalidDataToStartListeningAsync("Selected port is blank");
                    return;
                }
                else if (SerialPort.GetPortNames().Contains(SelectedComPort) == false)
                {
                    await InvalidDataToStartListeningAsync("Selected port does not exist in ports associated with computer");
                    return;
                }
                else if (SelectedBaudRate <= 0 || BaudRates.Contains(SelectedBaudRate) == false)
                {
                    MessageBoxResult result = await Show(parentView, "An invalid baud rate has been specified. Is this correct?", "Error", MessageBoxButtons.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else if (SelectedDataBits <= 0 || DataBits.Contains(SelectedDataBits) == false)
                {
                    await InvalidDataToStartListeningAsync("An invalid databits value has been specified. Reselect databits and try again");
                    return;
                }
                else if (SelectedStopBits <= 0 || StopBits.Contains(SelectedStopBits) == false)
                {
                    await InvalidDataToStartListeningAsync("An invalid stop bits value has been specified. Reselect stop bits and try again");
                    return;
                }
                else if (SelectedParity == null)
                {
                    await InvalidDataToStartListeningAsync("No parity selected");
                    return;
                }
                else if ((StabilityIndicatorActive == false && SequenceOfIdenticalReadingsActive == false)
                || (StabilityIndicatorActive && SequenceOfIdenticalReadingsActive))
                {
                    await InvalidDataToStartListeningAsync("Somehow both the stability indicator and sequence of identical readers got the same selection. Reselect one of the options and try again");
                    return;
                }
                else if (StabilityIndicatorActive && string.IsNullOrWhiteSpace(StabilityIndicatorSnippet))
                {
                    MessageBoxResult result = await Show(parentView, "Stability indicator active, but no character snippet provided. Is this correct?", "Error", MessageBoxButtons.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else if (StabilityIndicatorStartingPosition <= 0)
                {
                    await InvalidDataToStartListeningAsync("Stability indicator can not start at position lower than one.");
                    return;
                }
                else if (SequenceOfIdenticalReadingsActive && (NumberOfIdenticalReadings <= 0))
                {
                    await InvalidDataToStartListeningAsync("Number of readings for sequence of identical readings can not be lower than one.");
                    return;
                }
                else if (WeightStartPosition < 0 || WeightEndPosition < 0)
                {
                    await InvalidDataToStartListeningAsync("Scale string settings have to indications positions greater than or equal to zero.");
                    return;
                }
                else if ((TakeFullWeightString == false) && (WeightStartPosition != WeightEndPosition) && (WeightEndPosition <= WeightStartPosition))
                {
                    await InvalidDataToStartListeningAsync("Scale string's end position can not be less than starting position.");
                    return;
                }
                else if (StringRequiredLengthActive && (ScaleStringRequiredLength <= 0))
                {
                    await InvalidDataToStartListeningAsync("Scale string required length setting requires a length greater than zero");
                    return;
                }


                ListeningOnSerialPort = tempValue;
                if (ListeningOnSerialPort)
                {
                    Debug.WriteLine("About to listen on serial port");
                    SerialPortTools.Set
                    Extensions.CatchSerialPortException(() => SerialPortTools.GetPortAndStartListening(), (message, _) => { Debug.WriteLine(message); });
                }
                else
                {
                    Debug.WriteLine("About to stop listening on serial port");
                    Extensions.CatchSerialPortException(() => SerialPortTools.StopListeningOnPort(), (_, _) => { });
                }

            }
        }
        private bool ListeningOnSerialPort      { get; set; } = false;
        private bool BroadcastingSerialValues   { get; set; } = false;

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

        private string _LastReceivedValue = string.Empty;
        public string LastReceivedValue
        {
            get => _LastReceivedValue;
            set => SetProperty(ref _LastReceivedValue, value);
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
        private Parity? _selectedParity = null;

        private static Parity? GetParityOptionFromString(string parityString)
        {
            return parityString switch
            {
                SerialPortTools.EvenParity  => Parity.Even,
                SerialPortTools.OddParity   => Parity.Odd,
                SerialPortTools.NoParity    => Parity.None,
                _           => null,
            };
        }

        public Parity? SelectedParity
        {
            get => _selectedParity;
            set => SetProperty(ref _selectedParity, value, () => {
                switch (_selectedParity)
                {
                    case Parity.Even:
                        EvenParityChecked = true;
                        OddParityChecked = false;
                        NoneParityChecked = false;
                        CustomSettings.SetSetting(StringSetting.Parity, SerialPortTools.EvenParity);
                        break;
                    case Parity.Odd:
                        EvenParityChecked = false;
                        OddParityChecked = true;
                        NoneParityChecked = false;
                        CustomSettings.SetSetting(StringSetting.Parity, SerialPortTools.OddParity);
                        break;
                    case Parity.None:
                        EvenParityChecked = false;
                        OddParityChecked = false;
                        NoneParityChecked = true;
                        CustomSettings.SetSetting(StringSetting.Parity, SerialPortTools.NoParity);
                        break;
                    default:
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

        private int _stabilityIndicatorStartingPosition = /*GetSetting(IntSetting.StabilityIndicatorStartPosition)*/3;
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

        #region Commands
        //public ReactiveCommand<Unit, Task> ListenOnSerialPortCommand    { get; }
        public ReactiveCommand<Unit, Unit> BroadcastSerialValuesCommand { get; }

        private bool _takeFullWeightString = GetSetting(BoolSetting.TakeFullScaleString);
        public bool TakeFullWeightString
        {
            get => _takeFullWeightString;
            set => SetProperty(ref _takeFullWeightString, value);
        }
        #endregion
    }
}
