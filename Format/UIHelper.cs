﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Format
{
    public static class UIHelper
    {
        public static void PrintTextCentered(string text)
        {
            int padding = GetCenteredPadding(text.Length);
            Console.WriteLine(new string(' ', padding) + text);
        }

        public static void PrintTitle(string text)
        {
            string titlePartOne = "(¯`·._.··¸.-~*´¨¯¨`*·~-.";
            string titlePartTwo = "-~*´¨¯¨`*·~-.¸··._.·´¯)";
            int padding = GetCenteredPadding(text.Length + titlePartOne.Length + titlePartTwo.Length);
            Console.WriteLine(new string(' ', padding) + titlePartOne + text.ToUpper() + titlePartTwo);
        }

        public static void PrintSeparator()
        {
            UIHelper.PrintTextCentered("--------------------------------------------------------------------------------------------------------");
        }

        public static string PrintInputCentered()
        {
            return GetCenteredInput(allowDigitsOnly: true, maxLength: 3);
        }

        public static string CenteredInput(string prompt)
        {
            PrintTextCentered(prompt);
            return GetCenteredInput();
        }

        private static int GetCenteredPadding(int textLength)
        {
            return (Console.WindowWidth - textLength) / 2;
        }

        private static string GetCenteredInput(bool allowDigitsOnly = false, int maxLength = 50)
        {
            string input = "";
            ConsoleKeyInfo key;
            int startY = Console.CursorTop;

            while (true)
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter && input.Length > 0)
                    break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    input = input[..^1];
                else if (!char.IsControl(key.KeyChar) && (!allowDigitsOnly || char.IsDigit(key.KeyChar)))
                    if (input.Length < maxLength) input += key.KeyChar;

                int startX = GetCenteredPadding(input.Length);

                Console.SetCursorPosition(0, startY);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(startX, startY);
                Console.Write(input);
            }

            return input;
        }
    }
}
