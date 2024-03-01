// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using System;
using System.Collections.Specialized;
using SerialToSocket.AvaloniaApp.Constants;
using SerialToSocket.AvaloniaApp.Enums.Settings;
using SerialToSocket.AvaloniaApp.Properties;
using SerialToSocket.AvaloniaApp.Utils;

namespace SerialToSocket.AvaloniaApp
{
    /// <summary>
    /// Provides methods for accessing and managing custom application settings.
    /// </summary>
    public static partial class CustomSettings
    {
        #region Default if invalid section
        /// <summary>
        /// Returns the default value if the provided value is null, empty, or consists only of white-space characters; otherwise, returns the provided value.
        /// </summary>
        /// <param name="value">The value to check for validity.</param>
        /// <param name="defaultValue">The default value to return if the provided value is invalid.</param>
        /// <returns>The provided value if valid; otherwise, the default value.</returns>
        private static string DefaultIfInvalid(string value, string defaultValue) => string.IsNullOrWhiteSpace(value) ? defaultValue : value;

        /// <summary>
        /// Returns the default value if the provided value is less than or equal to -1; otherwise, returns the provided value.
        /// </summary>
        /// <param name="value">The value to check for validity.</param>
        /// <param name="defaultValue">The default value to return if the provided value is invalid.</param>
        /// <returns>The provided value if valid; otherwise, the default value.</returns>
        private static int DefaultIfInvalid(int value, int defaultValue) => value <= -1 ? defaultValue : value;
        #endregion

        /// <summary>
        /// Retrieves the value of the specified string setting.
        /// </summary>
        /// <param name="setting">The string setting to retrieve.</param>
        /// <returns>The value of the string setting.</returns>
        public static string GetSetting(StringSetting setting)
        {
            return setting switch
            {
                StringSetting.ComPort => Settings.Default.SerialPort,

                StringSetting.Parity => DefaultIfInvalid(
                    SerialConstants.ParityValues.Contains(Settings.Default.Parity)
                        ? Settings.Default.Parity
                        : SerialConstants.DefaultParity,
                    SerialConstants.DefaultParity),

                StringSetting.FlowControl => DefaultIfInvalid(Settings.Default.Flow_control, SerialConstants.FlowControlNone),
                StringSetting.StabilityIndicatorSnippet => DefaultIfInvalid(Settings.Default.Stability_indicator_snippet, SerialConstants.DefaultStabilityIndicatorSnippet),
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        /// <summary>
        /// Sets the value of the specified string setting.
        /// </summary>
        /// <param name="setting">The string setting to set.</param>
        /// <param name="value">The value to set.</param>
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

        /// <summary>
        /// Retrieves the value of the specified boolean setting.
        /// </summary>
        /// <param name="setting">The boolean setting to retrieve.</param>
        /// <returns>The value of the boolean setting.</returns>
        public static bool GetSetting(BoolSetting setting)
        {
            return setting switch
            {
                BoolSetting.StabilityIndicatorActive => Settings.Default.Stability_indicator_active,
                BoolSetting.SequenceOfIdenticalReadingsActive => Settings.Default.Sequence_of_identical_readings_active,
                BoolSetting.ScaleStringRequiredLength => Settings.Default.Scale_string_must_conform_to_length,
                BoolSetting.TakeFullScaleString => Settings.Default.Take_full_scale_string,
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        /// <summary>
        /// Sets the value of the specified boolean setting.
        /// </summary>
        /// <param name="setting">The boolean setting to set.</param>
        /// <param name="value">The value to set.</param>

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

        /// <summary>
        /// Retrieves the value of the specified integer setting.
        /// </summary>
        /// <param name="setting">The integer setting to retrieve.</param>
        /// <returns>The value of the integer setting.</returns>
        public static int GetSetting(IntSetting setting)
        {
            return setting switch
            {
                IntSetting.BaudRate => DefaultIfInvalid(
                    SerialConstants.BaudRates.Contains(Settings.Default.BaudRate)
                        ? Settings.Default.BaudRate
                        : SerialConstants.DefaultBaudRate,
                   SerialConstants.DefaultBaudRate),

                IntSetting.Databits => DefaultIfInvalid(
                    SerialConstants.DataBits.Contains(Settings.Default.Databits)
                        ? Settings.Default.Databits
                        : SerialConstants.DefaultDatabits,
                    SerialConstants.DefaultDatabits),

                IntSetting.Stopbits => DefaultIfInvalid(
                    SerialConstants.StopBits.Contains(Settings.Default.Stop_bits)
                        ? Settings.Default.Stop_bits
                        : SerialConstants.DefaultStopbits,
                   SerialConstants.DefaultStopbits),


                IntSetting.StabilityIndicatorStartPosition => DefaultIfInvalid(Settings.Default.Stability_indicator_starting_position, SerialConstants.DefaultStabilityIndicatorStartPosition),
                IntSetting.IdenticalReadingQuantity => DefaultIfInvalid(Settings.Default.Number_of_identical_readings, SerialConstants.DefaultIdenticalReadingQuantity),
                IntSetting.ScaleStringWeightStartPosition => DefaultIfInvalid(Settings.Default.Scale_string_weight_start_position, SerialConstants.DefaultScaleStringWeightStartPosition),
                IntSetting.ScaleStringWeightEndPosition => DefaultIfInvalid(Settings.Default.Scale_string_weight_end_position, SerialConstants.DefaultScaleStringWeightEndPosition),
                IntSetting.ScaleStringRequiredLength => DefaultIfInvalid(Settings.Default.Scale_string_minimum_length, SerialConstants.DefaultScaleStringMinimumLength),
                IntSetting.BroadcastPort => DefaultIfInvalid(Settings.Default.BroadcastPort, NetworkUtils.DefaultBroadcastPort),
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        /// <summary>
        /// Sets the value of the specified integer setting.
        /// </summary>
        /// <param name="setting">The integer setting to set.</param>
        /// <param name="value">The value to set.</param>
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

        /// <summary>
        /// Retrieves the value of the specified string collection setting.
        /// </summary>
        /// <param name="setting">The string collection setting to retrieve.</param>
        /// <returns>The value of the string collection setting.</returns>
        internal static StringCollection GetSetting(StringCollectionSetting setting)
        {
            return setting switch
            {
                StringCollectionSetting.CollectionOfReceivedValues => Settings.Default.Received_values,
                _ => throw new ArgumentOutOfRangeException(nameof(setting)),
            };
        }

        /// <summary>
        /// Sets the value of the specified string collection setting.
        /// </summary>
        /// <param name="setting">The string collection setting to set.</param>
        /// <param name="value">The value to set.</param>
        internal static void SetSetting(this StringCollectionSetting setting, StringCollection value)
        {
            switch (setting)
            {
                case StringCollectionSetting.CollectionOfReceivedValues:
                    Settings.Default.Received_values = value;
                    break;
            }
        }

        /// <summary>
        /// Adds a value to the specified string collection setting.
        /// </summary>
        /// <param name="setting">The string collection setting.</param>
        /// <param name="value">The value to add.</param>
        internal static void Add(this StringCollectionSetting setting, string value)
        {
            switch (setting)
            {
                case StringCollectionSetting.CollectionOfReceivedValues:
                    Settings.Default.Received_values.Insert(0, value);
                    break;
            }
        }

        /// <summary>
        /// Clears the collection of received values.
        /// </summary>
        internal static void ClearCollectionOfReceivedValues()
        {
            StringCollectionSetting.CollectionOfReceivedValues.SetSetting([]);
        }
    }
}
