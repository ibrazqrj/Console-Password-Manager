using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    class PrintCentered
    {
        public static void PrintTextCentered(string text)
        {
            int windowWidth = Console.WindowWidth;
            int padding = (windowWidth - text.Length) / 2;
            Console.WriteLine(new string(' ', padding) + text);
        }
    }
}
