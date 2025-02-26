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

        public static int PrintInputCentered()
        {
            string input = "";
            ConsoleKeyInfo key;
            int consoleWidth = Console.WindowWidth;
            int startY = Console.CursorTop;

            while (true)
            {
                key = Console.ReadKey(true); // Lesen ohne Anzeige

                if (key.Key == ConsoleKey.Enter && input.Length > 0)
                {
                    break; // Eingabe abgeschlossen
                }
                else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1]; // Letztes Zeichen entfernen
                }
                else if (char.IsDigit(key.KeyChar) && input.Length < 3) // Nur Zahlen zulassen (max. 3 Stellen für 130)
                {
                    input += key.KeyChar;
                }

                // Zentrierte Position berechnen
                int startX = (consoleWidth / 2) - (input.Length / 2);

                // Vorherige Eingabe überschreiben
                Console.SetCursorPosition(0, startY);
                Console.Write(new string(' ', consoleWidth)); // Zeile leeren
                Console.SetCursorPosition(startX, startY);
                Console.Write(input);
            }

            return int.TryParse(input, out int result) ? result : -1; // Falls ungültig, Rückgabe -1
        }

    }
}
