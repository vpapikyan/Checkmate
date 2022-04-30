using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmateUI.UIFunctions
{
    public static class UIFunction
    {
        public static void Write(string text, bool isNewLine = false)
        {
            Console.Write(text);
            if (isNewLine)
            {
                Console.WriteLine();
            }
        }
        public static string Read()
        {
            return Console.ReadLine();
        }
        public static void Clear()
        {
            Console.Clear();
        }
        public static ConsoleKey GetKey()
        {
            return Console.ReadKey().Key;
        }
    }
}
