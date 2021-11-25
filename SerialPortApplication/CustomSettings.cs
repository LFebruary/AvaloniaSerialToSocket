using SerialPortApplication.Properties;
using System;
using System.Collections.Specialized;
using static SerialPortApplication.SerialPortTools;

namespace SerialPortApplication
{
    public static partial class CustomSettings
    {

        private static string DefaultIfInvalid(string value, string defaultValue) => string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        public static string GetSetting(StringSetting setting)
        {
            return setting switch
            {
                StringSetting.ComPort                   => Settings.Default.SerialPort,

                StringSetting.Parity                    => DefaultIfInvalid(
                    ParityValues.Contains(Settings.Default.Parity)
                        ? Settings.Default.Parity
                        : DefaultParity,
                    DefaultParity),

                StringSetting.FlowControl               => DefaultIfInvalid(Settings.Default.Flow_control, FlowControlNone),
                StringSetting.StabilityIndicatorSnippet => DefaultIfInvalid(Settings.Default.Stability_indicator_snippet, DefaultStabilityIndicatorSnippet),
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        public static void SetSetting(this StringSetting setting, string value)
        {
            switch (setting)
            {
                case StringSetting.ComPort:
                    Settings.Default.SerialPort = value;
                    break;
                case StringSetting.Parity:
                    Settings.Default.Parity = value;
                    break;
                case StringSetting.FlowControl:
                    Settings.Default.Flow_control = value;
                    break;
                case StringSetting.StabilityIndicatorSnippet:
                    Settings.Default.Stability_indicator_snippet = value;
                    break;
            }
        }

        private static int DefaultIfInvalid(int value, int defaultValue) => value <= -1 ? defaultValue : value;

        public static int GetSetting(IntSetting setting)
        {
            return setting switch
            {
                IntSetting.BaudRate                         => DefaultIfInvalid(
                    BaudRates.Contains(Settings.Default.BaudRate) 
                        ? Settings.Default.BaudRate 
                        : DefaultBaudRate,
                    DefaultBaudRate),

                IntSetting.Databits                         => DefaultIfInvalid(
                    DataBits.Contains(Settings.Default.Databits)
                        ? Settings.Default.Databits
                        : DefaultDatabits,
                    DefaultDatabits),

                IntSetting.Stopbits                         => DefaultIfInvalid(
                    StopBits.Contains(Settings.Default.Stop_bits)
                        ? Settings.Default.Stop_bits
                        : DefaultStopbits,
                    DefaultStopbits),


                IntSetting.StabilityIndicatorStartPosition  => DefaultIfInvalid(Settings.Default.Stability_indicator_starting_position, DefaultStabilityIndicatorStartPosition),
                IntSetting.IdenticalReadingQuantity         => DefaultIfInvalid(Settings.Default.Number_of_identical_readings, DefaultIdenticalReadingQuantity),
                IntSetting.ScaleStringWeightStartPosition   => DefaultIfInvalid(Settings.Default.Scale_string_weight_start_position, DefaultScaleStringWeightStartPosition),
                IntSetting.ScaleStringWeightEndPosition     => DefaultIfInvalid(Settings.Default.Scale_string_weight_end_position, DefaultScaleStringWeightEndPosition),
                IntSetting.ScaleStringRequiredLength        => DefaultIfInvalid(Settings.Default.Scale_string_minimum_length, DefaultScaleStringMinimumLength),
                IntSetting.BroadcastPort                    => DefaultIfInvalid(Settings.Default.BroadcastPort, SocketTools.DefaultBroadcastPort),
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        public static void SetSetting(this IntSetting setting, int value)
        {
            switch (setting)
            {
                case IntSetting.BaudRate:
                    Settings.Default.BaudRate = value;
                    break;
                case IntSetting.Databits:
                    Settings.Default.Databits = value;
                    break;
                case IntSetting.Stopbits:
                    Settings.Default.Stop_bits = value;
                    break;
                case IntSetting.StabilityIndicatorStartPosition:
                    Settings.Default.Stability_indicator_starting_position = value;
                    break;
                case IntSetting.IdenticalReadingQuantity:
                    Settings.Default.Number_of_identical_readings = value;
                    break;
                case IntSetting.ScaleStringWeightStartPosition:
                    Settings.Default.Scale_string_weight_start_position = value;
                    break;
                case IntSetting.ScaleStringWeightEndPosition:
                    Settings.Default.Scale_string_weight_end_position = value;
                    break;
                case IntSetting.ScaleStringRequiredLength:
                    Settings.Default.Scale_string_minimum_length = value;
                    break;
                case IntSetting.BroadcastPort:
                    Settings.Default.BroadcastPort = value;
                    break;
            }
        }

        public static bool GetSetting(BoolSetting setting)
        {
            return setting switch
            {
                BoolSetting.StabilityIndicatorActive            => Settings.Default.Stability_indicator_active,
                BoolSetting.SequenceOfIdenticalReadingsActive   => Settings.Default.Sequence_of_identical_readings_active,
                BoolSetting.ScaleStringRequiredLength           => Settings.Default.Scale_string_must_conform_to_length,
                BoolSetting.TakeFullScaleString                 => Settings.Default.Take_full_scale_string,
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        public static void SetSetting(this BoolSetting setting, bool value)
        {
            switch (setting)
            {
                case BoolSetting.StabilityIndicatorActive:
                    Settings.Default.Stability_indicator_active = value;
                    break;
                case BoolSetting.SequenceOfIdenticalReadingsActive:
                    Settings.Default.Sequence_of_identical_readings_active = value;
                    break;
                case BoolSetting.ScaleStringRequiredLength:
                    Settings.Default.Scale_string_must_conform_to_length = value;
                    break;
                case BoolSetting.TakeFullScaleString:
                    Settings.Default.Take_full_scale_string = value;
                    break;
            }
        }

        internal static StringCollection GetSetting(StringCollectionSetting setting)
        {
            return setting switch
            {
                StringCollectionSetting.CollectionOfReceivedValues => Settings.Default.Received_values,
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        internal static void SetSetting(this StringCollectionSetting setting, StringCollection value)
        {
            switch (setting)
            {
                case StringCollectionSetting.CollectionOfReceivedValues:
                    Settings.Default.Received_values = value;
                    break;
            }
        }

        internal static void Add(this StringCollectionSetting setting, string value)
        {
            switch (setting)
            {
                case StringCollectionSetting.CollectionOfReceivedValues:
                    Settings.Default.Received_values.Insert(0, value);
                    break;
            }
        }

        internal static void ClearCollectionOfReceivedValues()
        {
            SetSetting(StringCollectionSetting.CollectionOfReceivedValues, new StringCollection());
        }

        public enum StringSetting
        {
            ComPort,
            Parity,
            FlowControl,
            StabilityIndicatorSnippet
        }

        

        public enum IntSetting
        {
            BaudRate,
            Databits,
            Stopbits,
            StabilityIndicatorStartPosition,
            IdenticalReadingQuantity,
            ScaleStringWeightStartPosition,
            ScaleStringWeightEndPosition,
            ScaleStringRequiredLength,
            BroadcastPort
        }

        public enum BoolSetting
        {
            StabilityIndicatorActive,
            SequenceOfIdenticalReadingsActive,
            ScaleStringRequiredLength,
            TakeFullScaleString
        }

        internal enum StringCollectionSetting
        {
            CollectionOfReceivedValues
        }

        internal enum FlowControl
        {
            Ctr_Rts,
            Dsr_Dtr,
            Xon_Xoff,
            None
        }
    }
}
