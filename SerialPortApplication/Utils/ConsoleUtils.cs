// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using SerialToSocket.AvaloniaApp.Enums;
using System;

namespace SerialToSocket.AvaloniaApp.Utils
{
    internal static class ConsoleUtils
    {
        /// <summary>
        /// Writes an informational message to the console and invokes the console callback.
        /// </summary>
        /// <param name="value">The message to write to the console.</param>
        /// <param name="consoleCallback">The callback function to execute when writing to the console.</param>
        internal static void WriteInfo(string value, Action<string, ConsoleAlertLevel> consoleCallback)
        {
            _Write(value);
            consoleCallback.Invoke(value, ConsoleAlertLevel.Info);
        }

        /// <summary>
        /// Writes a warning message to the console and invokes the console callback.
        /// </summary>
        /// <param name="value">The message to write to the console.</param>
        /// <param name="consoleCallback">The callback function to execute when writing to the console.</param>
        internal static void WriteWarning(string value, Action<string, ConsoleAlertLevel> consoleCallback)
        {
            _Write(value, ConsoleAlertLevel.Warning);
            consoleCallback.Invoke(value, ConsoleAlertLevel.Warning);
        }

        /// <summary>
        /// Writes an error message to the console and invokes the console callback.
        /// </summary>
        /// <param name="value">The message to write to the console.</param>
        /// <param name="consoleCallback">The callback function to execute when writing to the console.</param>
        internal static void WriteError(string value, Action<string, ConsoleAlertLevel> consoleCallback)
        {
            _Write(value, ConsoleAlertLevel.Error);
            consoleCallback.Invoke(value, ConsoleAlertLevel.Error);
        }

        /// <summary>
        /// Writes a message to the console with the specified alert level.
        /// </summary>
        /// <param name="value">The message to write to the console.</param>
        /// <param name="alertLevel">The level of the console alert (Info, Warning, Error). Default is Info.</param>
        private static void _Write(string value, ConsoleAlertLevel alertLevel = ConsoleAlertLevel.Info)
        {
            switch (alertLevel)
            {
                case ConsoleAlertLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case ConsoleAlertLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case ConsoleAlertLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.Error.WriteLine(value);
        }
    }
}
