// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using Avalonia.Controls;
using SerialToSocket.AvaloniaApp;
using SerialToSocket.AvaloniaApp.Constants;
using SerialToSocket.AvaloniaApp.Enums.Settings;
using SerialToSocket.AvaloniaApp.Exceptions;
using SerialToSocket.AvaloniaApp.Models;
using SerialToSocket.AvaloniaApp.Utils;
using SerialToSocket.AvaloniaApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using static SerialToSocket.AvaloniaApp.CustomSettings;
using static SerialToSocket.AvaloniaApp.Views.MessageBox;

namespace SerialToSocket.AvaloniaApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(MainWindow window) : base(window)
        {
            SelectedParity = _GetParityOptionFromString(GetSetting(StringSetting.Parity));
            SelectedFlowControl = _GetFlowControlFromString(GetSetting(StringSetting.FlowControl));
        }

        public void BroadcastSerialValuesCommand()
        {
            if (ListeningOnSerialPort)
            {
                bool tempValue = !BroadcastingSerialValues;
                if (tempValue)
                {
                    NetworkUtils.StartServer();
                }
                else
                {
                    NetworkUtils.StopServer();
                }
                BroadcastingSerialValues = tempValue;
            }
        }
        public async void ListenOnSerialPortCommand()
        {
            async Task ErrorMessageBox(string message)
            {
                _ = await Show(ParentView, message, "Error", MessageBoxButtons.Ok);
            }

            bool tempValue = !ListeningOnSerialPort;
            if (tempValue)
            {

                if (string.IsNullOrWhiteSpace(SelectedComPort))
                {
                    await ErrorMessageBox("Selected port is blank");
                    return;
                }
                else if (SerialPort.GetPortNames().Contains(SelectedComPort) == false)
                {
                    await ErrorMessageBox("Selected port does not exist in ports associated with computer");
                    return;
                }
                else if (SelectedBaudRate <= 0 || BaudRates.Contains(SelectedBaudRate) == false)
                {
                    MessageBoxResult result = await Show(ParentView, "An invalid baud rate has been specified. Is this correct?", "Error", MessageBoxButtons.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else if (SelectedDataBits <= 0 || DataBits.Contains(SelectedDataBits) == false)
                {
                    await ErrorMessageBox("An invalid databits value has been specified. Reselect databits and try again");
                    return;
                }
                else if (SelectedStopBits <= 0 || StopBits.Contains(SelectedStopBits) == false)
                {
                    await ErrorMessageBox("An invalid stop bits value has been specified. Reselect stop bits and try again");
                    return;
                }
                else if (SelectedParity == null)
                {
                    await ErrorMessageBox("No parity selected");
                    return;
                }
                else if (StabilityIndicatorActive == false && SequenceOfIdenticalReadingsActive == false
                || StabilityIndicatorActive && SequenceOfIdenticalReadingsActive)
                {
                    await ErrorMessageBox("Somehow both the stability indicator and sequence of identical readers got the same selection. Reselect one of the options and try again");
                    return;
                }
                else if (StabilityIndicatorActive && string.IsNullOrWhiteSpace(StabilityIndicatorSnippet))
                {
                    MessageBoxResult result = await Show(ParentView, "Stability indicator active, but no character snippet provided. Is this correct?", "Error", MessageBoxButtons.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else if (StabilityIndicatorStartingPosition <= 0)
                {
                    await ErrorMessageBox("Stability indicator can not start at position lower than one.");
                    return;
                }
                else if (SequenceOfIdenticalReadingsActive && NumberOfIdenticalReadings <= 0)
                {
                    await ErrorMessageBox("Number of readings for sequence of identical readings can not be lower than one.");
                    return;
                }
                else if (WeightStartPosition < 0 || WeightEndPosition < 0)
                {
                    await ErrorMessageBox("Scale string settings have to indications positions greater than or equal to zero.");
                    return;
                }
                else if (TakeFullWeightString == false && WeightStartPosition != WeightEndPosition && WeightEndPosition <= WeightStartPosition)
                {
                    await ErrorMessageBox("Scale string's end position can not be less than starting position.");
                    return;
                }
                else if (StringRequiredLengthActive && ScaleStringRequiredLength <= 0)
                {
                    await ErrorMessageBox("Scale string required length setting requires a length greater than zero");
                    return;
                }

                try
                {
                    SerialPortUtils.StartListening();
                    SerialPortUtils.OnValueUpdated = (value) =>
                    {
                        LastReceivedValue = value;
                    };
                }
                catch (SerialPortException exception)
                {
                    await ErrorMessageBox(exception.Message);
                    SerialPortUtils.ForceClose();
                    return;
                }
            }
            else
            {
                SerialPortUtils.OnValueUpdated = (_) => { };
                SerialPortUtils.StopListening();

                if (BroadcastingSerialValues)
                {
                    NetworkUtils.StopServer();
                    BroadcastingSerialValues = false;
                }
            }

            ListeningOnSerialPort = tempValue;
        }

        private bool _listeningOnSerialPort = false;
        private bool ListeningOnSerialPort
        {
            get => _listeningOnSerialPort;
            set => SetProperty(ref _listeningOnSerialPort, value, () =>
            {
                OnPropertyChanged(nameof(ListenToSerialButtonText));
                OnPropertyChanged(nameof(ListenToSerialCaptionText));
            });
        }

        private bool _broadcastingSerialValues = false;
        private bool BroadcastingSerialValues
        {
            get => _broadcastingSerialValues;
            set => SetProperty(ref _broadcastingSerialValues, value, () =>
            {
                OnPropertyChanged(nameof(SocketConnectionCaptionText));
                OnPropertyChanged(nameof(SocketConnectionButtonText));
            });
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
            set => SetProperty(ref _selectedComPort, value, () =>
            {
                OnPropertyChanged(nameof(SelectSerialPort));
                StringSetting.ComPort.SetSetting(SelectedComPort.Trim());
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
                IntSetting.BaudRate.SetSetting(SelectedBaudRate);
            });
        }

        private string _lastReceivedValue = string.Empty;
        public string LastReceivedValue
        {
            get => _lastReceivedValue;
            set
            {
                if (_lastReceivedValue != value)
                {
                    _lastReceivedValue = value;
                    OnPropertyChanged();
                }

                StringCollectionSetting.CollectionOfReceivedValues.Add(LastReceivedValue);
                OnPropertyChanged(nameof(LastProcessedValue));

            }
        }

        private static StringCollection ReceivedValues => GetSetting(StringCollectionSetting.CollectionOfReceivedValues);

        public string LastProcessedValue => _ProcessReading(LastReceivedValue);

        private string _ProcessReading(string lastReceivedValue)
        {
            if (StabilityIndicatorActive)
            {
                if (lastReceivedValue.Contains(StabilityIndicatorSnippet) == false)
                {
                    return invalidValue;
                }

                string refSnippet = lastReceivedValue.Substring(StabilityIndicatorStartingPosition - 1, StabilityIndicatorSnippet.Length);
                if (refSnippet != StabilityIndicatorSnippet)
                {
                    return invalidValue;
                }
            }
            else if (SequenceOfIdenticalReadingsActive)
            {
                if (NumberOfIdenticalReadings > ReceivedValues.Count)
                {
                    Debug.WriteLine("NumberOfIdenticalReadings > ReceivedValues.Count");
                    return invalidValue;
                }
                else if (string.IsNullOrWhiteSpace(lastReceivedValue))
                {
                    Debug.WriteLine("string.IsNullOrWhiteSpace(lastReceivedValue)");
                    return invalidValue;
                }
                else
                {
                    string identicalReadingToLookFor = lastReceivedValue;
                    Debug.WriteLine($"identicalReadingToLookFor: {identicalReadingToLookFor}");
                    Debug.WriteLine($"NumberOfIdenticalReadings: {NumberOfIdenticalReadings}");
                    for (int i = 0; i < NumberOfIdenticalReadings; i++)
                    {
                        Debug.WriteLine($"i = {i}");
                        if (ReceivedValues[i] == identicalReadingToLookFor)
                        {
                            Debug.WriteLine("ReceivedValues[i] == identicalReadingToLookFor");
                            continue;
                        }
                        else
                        {
                            Debug.WriteLine($"Invalid value encountered in top {NumberOfIdenticalReadings} readings in list");
                            return invalidValue;
                        }
                    }
                }
            }
            if (WeightStartPosition >= 1 && WeightStartPosition > WeightEndPosition)
            {
                return invalidValue;
            }
            else if (WeightStartPosition > lastReceivedValue.Length)
            {
                return invalidValue;
            }
            else if (StringRequiredLengthActive && ScaleStringRequiredLength > 0 && lastReceivedValue.Length != ScaleStringRequiredLength)
            {
                return invalidValue;
            }
            else
            {
                string userSpecifiedString = lastReceivedValue[(WeightStartPosition - 1)..WeightEndPosition];
                return userSpecifiedString.Any(char.IsLetter)
                    ? $"{userSpecifiedString} ({new string(userSpecifiedString.Where(i => char.IsNumber(i) || i == '.' || i == ',').ToArray())})"
                    : userSpecifiedString;
            }
        }

        private const string invalidValue = "NAN";

        private IEnumerable<int> _baudRates = SerialConstants.BaudRates;
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
                IntSetting.Databits.SetSetting(SelectedDataBits);
            });
        }

        private IEnumerable<int> _dataBits = SerialConstants.DataBits;
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
                IntSetting.Stopbits.SetSetting(SelectedStopBits);
            });
        }

        private IEnumerable<int> _stopBits = SerialConstants.StopBits;
        public IEnumerable<int> StopBits
        {
            get => _stopBits;
            set => SetProperty(ref _stopBits, value);
        }

        private FlowControl? _selectedFlowControl;
        private static FlowControl? _GetFlowControlFromString(string flowControlString)
        {
            return flowControlString switch
            {
                SerialConstants.FlowControlCtsRts => FlowControl.Ctr_Rts,
                SerialConstants.FlowControlDsrDtr => FlowControl.Dsr_Dtr,
                SerialConstants.FlowControlXonXoff => FlowControl.Xon_Xoff,
                SerialConstants.FlowControlNone => FlowControl.None,
                _ => null,
            };
        }
        private FlowControl? SelectedFlowControl
        {
            get => _selectedFlowControl;
            set => SetProperty(ref _selectedFlowControl, value, () =>
            {
                SerialPortUtils.SetFlowControl(SelectedFlowControl);
                switch (_selectedFlowControl)
                {
                    case FlowControl.Ctr_Rts:
                        CtsRtsSelected = true;
                        DsrDtrSelected = false;
                        XonXoffSelected = false;
                        NoneSelected = false;
                        StringSetting.FlowControl.SetSetting(SerialConstants.FlowControlCtsRts);
                        break;
                    case FlowControl.Dsr_Dtr:
                        CtsRtsSelected = false;
                        DsrDtrSelected = true;
                        XonXoffSelected = false;
                        NoneSelected = false;
                        StringSetting.FlowControl.SetSetting(SerialConstants.FlowControlDsrDtr);
                        break;
                    case FlowControl.Xon_Xoff:
                        CtsRtsSelected = false;
                        DsrDtrSelected = false;
                        XonXoffSelected = true;
                        NoneSelected = false;
                        StringSetting.FlowControl.SetSetting(SerialConstants.FlowControlXonXoff);
                        break;
                    case FlowControl.None:
                        CtsRtsSelected = false;
                        DsrDtrSelected = false;
                        XonXoffSelected = true;
                        NoneSelected = true;
                        StringSetting.FlowControl.SetSetting(SerialConstants.FlowControlNone);
                        break;
                }
            });
        }

        private bool _ctsRtsSelected;
        public bool CtsRtsSelected
        {
            get => _ctsRtsSelected;
            set => SetProperty(ref _ctsRtsSelected, value);
        }

        private bool _dsrDtrSelected;
        public bool DsrDtrSelected
        {
            get => _dsrDtrSelected;
            set => SetProperty(ref _dsrDtrSelected, value);
        }

        private bool _xonXoffSelected;
        public bool XonXoffSelected
        {
            get => _xonXoffSelected;
            set => SetProperty(ref _xonXoffSelected, value);
        }

        private bool _noneSelected;
        public bool NoneSelected
        {
            get => _noneSelected;
            set => SetProperty(ref _noneSelected, value);
        }
        #region Parity
        private Parity? _selectedParity = null;

        private static Parity? _GetParityOptionFromString(string parityKey)
        {
            return parityKey switch
            {
                SerialConstants.EvenParity => Parity.Even,
                SerialConstants.OddParity => Parity.Odd,
                SerialConstants.NoParity => Parity.None,
                _ => null,
            };
        }

        public Parity? SelectedParity
        {
            get => _selectedParity;
            set => SetProperty(ref _selectedParity, value, () =>
            {
                switch (_selectedParity)
                {
                    case Parity.Even:
                        EvenParityChecked = true;
                        OddParityChecked = false;
                        NoneParityChecked = false;
                        StringSetting.Parity.SetSetting(SerialConstants.EvenParity);
                        break;
                    case Parity.Odd:
                        EvenParityChecked = false;
                        OddParityChecked = true;
                        NoneParityChecked = false;
                        StringSetting.Parity.SetSetting(SerialConstants.OddParity);
                        break;
                    case Parity.None:
                        EvenParityChecked = false;
                        OddParityChecked = false;
                        NoneParityChecked = true;
                        StringSetting.Parity.SetSetting(SerialConstants.NoParity);
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
                BoolSetting.SequenceOfIdenticalReadingsActive.SetSetting(!value);
                BoolSetting.StabilityIndicatorActive.SetSetting(value);

                SequenceOfIdenticalReadingsActive = !value;
                OnPropertyChanged(nameof(LastProcessedValue));
            });
        }

        private bool _sequenceOfIdenticalReadingsActive = GetSetting(BoolSetting.SequenceOfIdenticalReadingsActive);
        public bool SequenceOfIdenticalReadingsActive
        {
            get => _sequenceOfIdenticalReadingsActive;
            set => SetProperty(ref _sequenceOfIdenticalReadingsActive, value, () =>
            {
                BoolSetting.StabilityIndicatorActive.SetSetting(!value);
                BoolSetting.SequenceOfIdenticalReadingsActive.SetSetting(value);

                StabilityIndicatorActive = !value;
                OnPropertyChanged(nameof(LastProcessedValue));
            });
        }

        private string _stabilityIndicatorSnippet = GetSetting(StringSetting.StabilityIndicatorSnippet);
        public string StabilityIndicatorSnippet
        {
            get => _stabilityIndicatorSnippet;
            set => SetProperty(ref _stabilityIndicatorSnippet, value, () =>
            {
                StringSetting.StabilityIndicatorSnippet.SetSetting(value);
                OnPropertyChanged(nameof(LastProcessedValue));
            });
        }

        private int _stabilityIndicatorStartingPosition = GetSetting(IntSetting.StabilityIndicatorStartPosition);
        public int StabilityIndicatorStartingPosition
        {
            get => _stabilityIndicatorStartingPosition;
            set => SetProperty(ref _stabilityIndicatorStartingPosition, value, () =>
            {
                IntSetting.StabilityIndicatorStartPosition.SetSetting(StabilityIndicatorStartingPosition);
                OnPropertyChanged(nameof(LastProcessedValue));
            });
        }

        private int _numberOfIdenticalReadings = GetSetting(IntSetting.IdenticalReadingQuantity);
        public int NumberOfIdenticalReadings
        {
            get => _numberOfIdenticalReadings;
            set => SetProperty(ref _numberOfIdenticalReadings, value, () =>
            {
                IntSetting.IdenticalReadingQuantity.SetSetting(NumberOfIdenticalReadings);
                OnPropertyChanged(nameof(LastProcessedValue));
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
                IntSetting.ScaleStringWeightStartPosition.SetSetting(WeightStartPosition);
                OnPropertyChanged(nameof(LastProcessedValue));
            });
        }

        private int _weightEndPosition = GetSetting(IntSetting.ScaleStringWeightEndPosition);
        public int WeightEndPosition
        {
            get => _weightEndPosition;
            set => SetProperty(ref _weightEndPosition, value, () =>
            {
                IntSetting.ScaleStringWeightEndPosition.SetSetting(WeightEndPosition);
                OnPropertyChanged(nameof(LastProcessedValue));
            });
        }

        private bool _stringRequiredLengthActive = GetSetting(BoolSetting.ScaleStringRequiredLength);
        public bool StringRequiredLengthActive
        {
            get => _stringRequiredLengthActive;
            set => SetProperty(ref _stringRequiredLengthActive, value, () =>
            {
                BoolSetting.ScaleStringRequiredLength.SetSetting(StringRequiredLengthActive);
                OnPropertyChanged(nameof(LastProcessedValue));
            });
        }

        private int _scaleStringRequiredLength = GetSetting(IntSetting.ScaleStringRequiredLength);
        public int ScaleStringRequiredLength
        {
            get => _scaleStringRequiredLength;
            set => SetProperty(ref _scaleStringRequiredLength, value, () =>
            {
                IntSetting.ScaleStringRequiredLength.SetSetting(ScaleStringRequiredLength);
                OnPropertyChanged(nameof(LastProcessedValue));
            });
        }
        #endregion

        private bool _takeFullWeightString = GetSetting(BoolSetting.TakeFullScaleString);
        public bool TakeFullWeightString
        {
            get => _takeFullWeightString;
            set => SetProperty(ref _takeFullWeightString, value);
        }
        public string ListenToSerialButtonText
        {
            get => $"{(ListeningOnSerialPort ? "Stop" : "Start")} listening on serial port";
        }

        public string ListenToSerialCaptionText
        {
            get
            {
                if (ListeningOnSerialPort == false)
                {
                    return "Currently not listening to any serial port";
                }
                else if (SerialPortUtils.IsOpen)
                {
                    return $"Currently listening to serial port {SelectedComPort} with:\nBaudrate {SelectedBaudRate}\nDatabits {SelectedDataBits}\nStop bits {SelectedStopBits}";
                }
                else
                {
                    return "Serial port is not open, but window attempting to listen to it. Restart program and try again.";
                }
            }
        }

        public string SocketConnectionCaptionText => BroadcastingSerialValues
            ? $"Currently broadcasting values over\nsocket connection via: {NetworkUtils.IpAddress}:{NetworkUtils.Port}"
            : "Values are not being broadcasted over\nsocket connection.";

        public string SocketConnectionButtonText => $"{(BroadcastingSerialValues ? "Stop" : "Start")} broadcasting serial readings";

        public enum SetupSpinnerType
        {
            StabilityStart,
            IdenticalReadingQuantity,
            WeightStart,
            WeightEnd,
            RequiredLength,
            BroadcastPort
        }

        internal void ChangeSpinnerValue(SetupSpinnerType selectedSpinner, SpinDirection direction)
        {
            int ChangeBasedOnDirection(int value) => direction == SpinDirection.Increase ? ++value : --value;

            int BoundValue(int valueToBound, int minValue = 1, int maxValue = int.MaxValue)
            {
                return valueToBound > maxValue
                    ? maxValue
                    : valueToBound < minValue
                        ? minValue
                        : valueToBound;
            }

            switch (selectedSpinner)
            {
                case SetupSpinnerType.StabilityStart:
                    StabilityIndicatorStartingPosition = BoundValue(ChangeBasedOnDirection(StabilityIndicatorStartingPosition));
                    break;
                case SetupSpinnerType.IdenticalReadingQuantity:
                    NumberOfIdenticalReadings = BoundValue(ChangeBasedOnDirection(NumberOfIdenticalReadings));
                    break;
                case SetupSpinnerType.WeightStart:
                    WeightStartPosition = BoundValue(ChangeBasedOnDirection(WeightStartPosition), 0);
                    break;
                case SetupSpinnerType.WeightEnd:
                    WeightEndPosition = BoundValue(ChangeBasedOnDirection(WeightEndPosition), WeightStartPosition);
                    break;
                case SetupSpinnerType.RequiredLength:
                    ScaleStringRequiredLength = BoundValue(ChangeBasedOnDirection(ScaleStringRequiredLength));
                    break;
                case SetupSpinnerType.BroadcastPort:
                    IntSetting.BroadcastPort.SetSetting(ChangeBasedOnDirection(NetworkUtils.Port));
                    OnPropertyChanged(nameof(Port));
                    break;
            }
        }

        public static int Port => NetworkUtils.Port;
    }
}
