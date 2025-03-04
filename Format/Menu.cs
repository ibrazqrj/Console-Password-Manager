using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Format
{
    class Menu
    {
        public static void showLogin()
        {
            EmptyFieldGenerator.generateFields(3);

            string[] logo = {
                " .--------.",
                " / .------. \\",
                " / /        \\ \\",
                " | |        | |",
                " _| |________| |_",
                ".' |_|        |_| '.",
                "'._____ ____ _____.'",
                "|     .'____'.     |",
                "'.__.'.'    '.'.__.'",
                "'.__  |      |  __.'",
                "|   '.'.____.'.'   |",
                "'.____'.____.'____.'",
                "'.________________.'"
            };

            string dev = " @ibrazqrj";

            foreach (string line in logo)
            {
                PrintCentered.PrintTextCentered(line);
            }

            PrintCentered.PrintTextCentered(dev);
            EmptyFieldGenerator.generateFields(2);
        }


        public static void showMainMenu()
        {
            PrintCentered.PrintTitle("PASSWORDMANAGER");

            EmptyFieldGenerator.generateFields(1);
            PrintCentered.PrintTextCentered("1 | Add password          ");
            PrintCentered.PrintTextCentered("2 | Show passwords        ");
            PrintCentered.PrintTextCentered("3 | Delete password       ");
            PrintCentered.PrintTextCentered("4 | Random password       ");
            PrintCentered.PrintTextCentered("5 | Search password       ");
            PrintCentered.PrintTextCentered("6 | Change master password");
            PrintCentered.PrintTextCentered("7 | Quit                  ");
            EmptyFieldGenerator.generateFields(1);
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-..-~*´¨¯¯¨`*·~-..-~*´¨¯¨`*·~-.¸··._.·´¯)");
            EmptyFieldGenerator.generateFields(1);
        }
    }
}
