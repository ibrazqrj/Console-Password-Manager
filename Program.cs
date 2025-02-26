using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PasswordManager
{
    internal class Program
    {
        private Timer _timer = null;

        static void Main(string[] args)
        {
            // Fenstergröße einstellen
            Console.SetWindowSize(150, 30);


            // Instanz des Passwortmanagers
            PasswordManagerOptions manager = new PasswordManagerOptions();
            MasterPasswordHelper passwordHelper = new MasterPasswordHelper();

            createMasterPassword(passwordHelper);
            string masterPassword = loginWithMasterPassword(passwordHelper);
            byte[] aesKey = AesEncryptionHelper.DeriveKeyFromPassword(masterPassword);

            while (true)
            {
                Console.Clear();
                int emptyFields = 8;

                for (int i = 0; i <= emptyFields; i++)
                {
                    PrintCentered.PrintTextCentered(" ");
                }

                PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORDMANAGER.-~*´¨¯¨`*·~-.¸··._.·´¯)");
                PrintMainMenu();
                PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-..-~*´¨¯¯¨`*·~-..-~*´¨¯¨`*·~-.¸··._.·´¯)");
                PrintCentered.PrintTextCentered(" ");

                string prompt = "Enter your choice: ";
                int screenWidth = Console.WindowWidth;
                int textWidth = prompt.Length;
                int leftPadding = (screenWidth - textWidth) / 2;

                Console.SetCursorPosition(leftPadding, Console.CursorTop);
                Console.Write(prompt);

                string choice = Console.ReadLine();

                // Menü-Optionen
                switch (choice)
                {
                    case "1":
                        AddPassword(manager, aesKey);
                        break;
                    case "2":
                        ShowPasswords(manager, aesKey);
                        break;
                    case "3":
                        DeletePassword(manager);
                        break;
                    case "4":
                        GeneratePassword(manager);
                        break;
                    case "5":
                        SearchPassword(manager, aesKey);
                        break;
                    case "6":
                        RenewMasterPassword(passwordHelper);
                        break;
                    case "7":
                        ExitProgram();
                        break;
                    default:
                        PrintCentered.PrintTextCentered("Invalid input! Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Startbildschirm mit Logo
        static void DisplayStartScreen()
        {

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

            // Logo und Entwicklername zentrieren
            foreach (string line in logo)
            {
                PrintCentered.PrintTextCentered(line);
            }

            PrintCentered.PrintTextCentered(dev);
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered(" ");
        }

        static void createMasterPassword(MasterPasswordHelper passwordHelper)
        {
            passwordHelper.HashPassword();
        }

        static string loginWithMasterPassword(MasterPasswordHelper passwordHelper)
        {
            ConsoleKeyInfo key;
            int consoleWidth = Console.WindowWidth;
            int emptyFields = 3; 

            for(int i = 0; i <= emptyFields; i++)
            {
                PrintCentered.PrintTextCentered(" ");
            }
            DisplayStartScreen();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORD MANAGER.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered("Please login.");
            PrintCentered.PrintTextCentered("");
            PrintCentered.PrintTextCentered("Enter your master password:");

            string pwInput = HidePassword();

            passwordHelper.VerifyMasterPassword(pwInput);

            return pwInput;
        }

        static string HidePassword()
        {
            string pass = "";
            ConsoleKeyInfo key;
            int consoleWidth = Console.WindowWidth;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1];
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                }

                int startX = (consoleWidth / 2) - (pass.Length / 2);
                int startY = Console.CursorTop;

                Console.SetCursorPosition(0, startY);
                Console.Write(new string(' ', consoleWidth));
                Console.SetCursorPosition(startX, startY);
                Console.Write(new string('*', pass.Length));

            } while (key.Key != ConsoleKey.Enter);

            return pass;
        }


        static void PrintMainMenu()
        {
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("1 | Add password          ");
            PrintCentered.PrintTextCentered("2 | Show passwords        ");
            PrintCentered.PrintTextCentered("3 | Delete password       ");
            PrintCentered.PrintTextCentered("4 | Random password       ");
            PrintCentered.PrintTextCentered("5 | Search password       ");
            PrintCentered.PrintTextCentered("6 | Change master password");
            PrintCentered.PrintTextCentered("7 | Quit                  ");
            PrintCentered.PrintTextCentered(" ");
        }

        static void AddPassword(PasswordManagerOptions manager, byte[] aesKey)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.CREATE ENTRY.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered("Please fill all fields to save your password!");
            PrintCentered.PrintTextCentered(" ");

            string inputWebsite = CenteredInput("Webpage / URL:");
            if (string.IsNullOrEmpty(inputWebsite))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            PrintCentered.PrintTextCentered(" ");

            string inputUsername = CenteredInput("Username / E-Mail:");
            if (string.IsNullOrEmpty(inputUsername))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            PrintCentered.PrintTextCentered(" ");

            string inputPassword = CenteredInput("Password:");
            if (string.IsNullOrEmpty(inputPassword))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            PrintCentered.PrintTextCentered(" ");
            string encryptedUsername = AesEncryptionHelper.EncryptPassword(inputUsername, aesKey);
            string encryptedPassword = AesEncryptionHelper.EncryptPassword(inputPassword, aesKey);

            manager.AddPassword(inputWebsite, encryptedUsername, encryptedPassword);

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Entry successfully added!");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        static string CenteredInput(string prompt)
        {
            PrintCentered.PrintTextCentered(prompt);
            string input = "";
            ConsoleKeyInfo key;
            int consoleWidth = Console.WindowWidth;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                }

                int startX = (consoleWidth / 2) - (input.Length / 2);
                int startY = Console.CursorTop;

                Console.SetCursorPosition(0, startY);
                Console.Write(new string(' ', consoleWidth));
                Console.SetCursorPosition(startX, startY);
                Console.Write(input);

            } while (key.Key != ConsoleKey.Enter);

            return input;
        }


        static void ShowPasswords(PasswordManagerOptions manager, byte[] aesKey)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORDS.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("------------------------------------------------------------------------------------------------------------------------------------------------------");
            PrintCentered.PrintTextCentered(" ");

            manager.listPasswords(aesKey);
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("------------------------------------------------------------------------------------------------------------------------------------------------------");

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("------------------------------------------------------------------------------------------------------------------------------------------------------");

            Console.ReadKey();
            Console.Clear();
        }

        static void DeletePassword(PasswordManagerOptions manager)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.DELETE ENTRY.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered(" ");

            manager.deletePassword();

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }
        static void GeneratePassword(PasswordManagerOptions manager)
        {
            Console.Clear();
            int emptyFields = 12;

            for (int i = 0; i <= emptyFields; i++)
            {
                PrintCentered.PrintTextCentered(" ");
            }

            PrintCentered.PrintTextCentered("(¯·._.··¸.-~*´¨¯¨*·~-.GENERATE PASSWORD.-~*´¨¯¨*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered("Enter the desired password length (1-130):");
            PrintCentered.PrintTextCentered(" ");

            string digitInput = "";
            ConsoleKeyInfo key;
            int consoleWidth = Console.WindowWidth;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && digitInput.Length > 0)
                {
                    digitInput = digitInput[..^1];
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    digitInput += key.KeyChar;
                }

                int startX = (consoleWidth / 2) - (digitInput.Length / 2);
                int startY = Console.CursorTop;

                Console.SetCursorPosition(0, startY);
                Console.Write(new string(' ', consoleWidth));
                Console.SetCursorPosition(startX, startY);
                Console.Write(digitInput);

            } while (key.Key != ConsoleKey.Enter);

            if (int.TryParse(digitInput, out int length) && length > 0 && length <= 130)
            {
                string generatedPassword = manager.GeneratePassword(length);
                PrintCentered.PrintTextCentered(" ");
                PrintCentered.PrintTextCentered($"Generated password: {generatedPassword}");
            }
            else if (length > 130)
            {
                PrintCentered.PrintTextCentered("Your number input is too high!");
            }
            else
            {
                PrintCentered.PrintTextCentered("Invalid input!");
            }

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }


        static void SearchPassword(PasswordManagerOptions manager, byte[] aesKey)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.SEARCH FOR AN ENTRY.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered(" ");
            manager.SearchPassword(aesKey);

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("--------------------------------------------------------------------------------------------------------");
            PrintCentered.PrintTextCentered(" ");

            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        static void RenewMasterPassword(MasterPasswordHelper passwordHelper)
        {
            passwordHelper.ChangeMasterPassword();
        }

        static void ExitProgram()
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("Password manager shutting down...");
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Press a random key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
