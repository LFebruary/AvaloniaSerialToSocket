// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using SerialToSocket.AvaloniaApp.Enums;
using SerialToSocket.AvaloniaApp.Exceptions;
using SerialToSocket.AvaloniaApp.Utils;

namespace SerialToSocket.AvaloniaApp
{
    /// <summary>
    /// Provides extension methods for various functionalities.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Fills the string with blanks to the specified length.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="length">The desired length of the resulting string.</param>
        /// <param name="character">The character used to fill the string.</param>
        /// <param name="prefixTab">Indicates whether to prefix the result with a tab.</param>
        /// <returns>A string filled with blanks to the specified length.</returns>
        internal static string FillBlanks(this string value, int length = 16, char character = ' ', bool prefixTab = false)
        {
            int lengthOfValue = value.Length;
            int lengthOfSpaces = length - lengthOfValue;

            return prefixTab
                ? $"\t{value}{new string(character, lengthOfSpaces)}"
                : $"{value}{new string(character, lengthOfSpaces)}";
        }

        /// <summary>
        /// Retrieves the first element of the sequence that satisfies a condition, or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="result">When this method returns, contains the first element in <paramref name="source"/> that passes the test, or the default value for the type if no such element is found.</param>
        /// <returns><c>true</c> if an element in the sequence satisfies the condition; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Invokes the action and catches any SerialPortException, writing the message to the console.
        /// </summary>
        /// <param name="p">The action to invoke.</param>
        /// <param name="consoleCallback">The callback function to execute when writing to the console.</param>
        public static void CatchSerialPortException(this Action p, Action<string, ConsoleAlertLevel> consoleCallback)
        {
            try
            {
                p.Invoke();
            }
            catch (SerialPortException ex)
            {
                ConsoleUtils.WriteError(ex.Message, consoleCallback);
            }
        }
    }
}
