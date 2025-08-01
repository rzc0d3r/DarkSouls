using System;

namespace DarkSouls.Utils
{
    public static class ConsoleUtils
    {
        public static void WriteLine(object obj, ConsoleColor color)
        {
            ConsoleColor fgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(obj);
            Console.ForegroundColor = fgColor;
        }
    }
}
