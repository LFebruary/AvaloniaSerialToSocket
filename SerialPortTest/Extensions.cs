using System;
using System.Collections.Generic;
using System.Linq;

namespace SerialPortTest
{
    internal static partial class Extensions
    {
        internal static string FillBlanks(this string value, int length = 16, char character = ' ', bool prefixTab = false)
        {
            int lengthOfValue = value.Length;
            int lengthOfSpaces = length - lengthOfValue;

            return prefixTab 
                ? $"\t{value}{new string(character, lengthOfSpaces)}" 
                : $"{value}{new string(character, lengthOfSpaces)}";
        }

        internal static bool FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource result)
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

        internal static void CatchSerialPortException(this Action p)
        {
            try
            {
                p.Invoke();
            }
            catch (SerialPortException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.Message);

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
