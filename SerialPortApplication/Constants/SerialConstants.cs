// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SerialToSocket.AvaloniaApp.Constants
{
    public class SerialConstants
    {
        public static readonly IList<int> BaudRates = new ReadOnlyCollection<int>(
            [
                110,
                300,
                600,
                1200,
                2400,
                4800,
                9600,
                19200
            ]);

        public static readonly int DefaultBaudRate = 9600;

        public static readonly IList<int> DataBits = new ReadOnlyCollection<int>(
            [
                6,
                7,
                8
            ]);

        public static readonly int DefaultDatabits = 8;

        public static readonly IList<int> StopBits = new ReadOnlyCollection<int>(
            [
                1,
                2
            ]);

        public static readonly int DefaultStopbits = 1;

        public static readonly int DefaultStabilityIndicatorStartPosition = 1;
        public static readonly int DefaultIdenticalReadingQuantity = 5;
        public static readonly int DefaultScaleStringWeightStartPosition = 11;
        public static readonly int DefaultScaleStringWeightEndPosition = 19;
        public static readonly int DefaultScaleStringMinimumLength = 17;

        public const string DefaultParity = NoParity;
        public const string DefaultStabilityIndicatorSnippet = "ST";

        public static readonly IList<string> ParityValues = new ReadOnlyCollection<string>(
            [
                EvenParity,
                OddParity,
                NoParity
            ]);

        public const string EvenParity = "Even";
        public const string OddParity = "Odd";
        public const string NoParity = "None";

        internal const string FlowControlCtsRts = "CTS/RTS";
        internal const string FlowControlDsrDtr = "DSR/DTS";
        internal const string FlowControlXonXoff = "XON/XOFF";
        internal const string FlowControlNone = "NONE";
        internal const string FlowControlDefault = FlowControlNone;
    }
}
