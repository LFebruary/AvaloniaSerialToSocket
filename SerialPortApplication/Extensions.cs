using System;
using System.Collections.Generic;
using System.Linq;

namespace SerialPortApplication
{
    public static partial class Extensions
    {
        internal static string FillBlanks(this string value, int length = 16, char character = ' ', bool prefixTab = false)
        {
            int lengthOfValue = value.Length;
            int lengthOfSpaces = length - lengthOfValue;

            return prefixTab 
                ? $"\t{value}{new string(character, lengthOfSpaces)}" 
                : $"{value}{new string(character, lengthOfSpaces)}";
        }

        internal static bool FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource? result)
        {
            if (source.Any(predicate))
            {
                result = source.First(predicate);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public static void CatchSerialPortException(this Action p, Action<string, ConsoleAlertLevel> consoleCallback)
        {
            try
            {
                p.Invoke();
            }
            catch (SerialPortException ex)
            {
                ConsoleWriteError(ex.Message, consoleCallback);
            }
        }

        internal static void ConsoleWriteInfo(string value, Action<string, ConsoleAlertLevel> consoleCallback)
        {
            ConsoleWrite(value);
            consoleCallback.Invoke(value, ConsoleAlertLevel.Info);
        }

        internal static void ConsoleWriteWarning(string value, Action<string, ConsoleAlertLevel> consoleCallback)
        {
            ConsoleWrite(value, ConsoleAlertLevel.Warning);
            consoleCallback.Invoke(value, ConsoleAlertLevel.Warning);
        }

        internal static void ConsoleWriteError(string value, Action<string, ConsoleAlertLevel> consoleCallback)
        {
            ConsoleWrite(value, ConsoleAlertLevel.Error);
            consoleCallback.Invoke(value, ConsoleAlertLevel.Error);
        }

        private static void ConsoleWrite(string value, ConsoleAlertLevel alertLevel = ConsoleAlertLevel.Info)
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

        public enum ConsoleAlertLevel
        {
            Info,
            Warning,
            Error
        }
    }
}
