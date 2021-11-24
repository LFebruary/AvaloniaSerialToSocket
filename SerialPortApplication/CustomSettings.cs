using SerialPortApplication.Properties;
using SerialPortTest;
using System;
using static SerialPortTest.SerialPortTools;

namespace SerialPortApplication
{
    public static partial class CustomSettings
    {
        private static string DefaultIfInvalid(string value, string defaultValue) => string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        public static string GetSetting(StringSetting setting)
        {
            return setting switch
            {
                StringSetting.ComPort                   => Settings.Default.Port,

                StringSetting.Parity                    => DefaultIfInvalid(
                    ParityValues.Contains(Settings.Default.Parity)
                        ? Settings.Default.Parity
                        : DefaultParity,
                    DefaultParity),

                StringSetting.FlowControl               => Settings.Default.Flow_control,
                StringSetting.StabilityIndicatorSnippet => DefaultIfInvalid(Settings.Default.Stability_indicator_snippet, DefaultStabilityIndicatorSnippet),
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        public static void SetSetting(this StringSetting setting, string value)
        {
            switch (setting)
            {
                case StringSetting.ComPort:
                    Settings.Default.Port = value;
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
                IntSetting.ScaleStringRequiredLength         => DefaultIfInvalid(Settings.Default.Scale_string_minimum_length, DefaultScaleStringMinimumLength),
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
            }
        }

        public static bool GetSetting(BoolSetting setting)
        {
            return setting switch
            {
                BoolSetting.StabilityIndicatorActive            => Settings.Default.Stability_indicator_active,
                BoolSetting.SequenceOfIdenticalReadingsActive   => Settings.Default.Sequence_of_identical_readings_active,
                BoolSetting.ScaleStringRequiredLength      => Settings.Default.Scale_string_must_conform_to_length,
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
            }
        }

        public enum StringSetting
        {
            ComPort,
            Parity,
            FlowControl,
            StabilityIndicatorSnippet
        }

        public enum ParityOption
        {
            Even,
            Odd,
            None
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
            ScaleStringRequiredLength
        }

        public enum BoolSetting
        {
            StabilityIndicatorActive,
            SequenceOfIdenticalReadingsActive,
            ScaleStringRequiredLength
        }
    }
}
