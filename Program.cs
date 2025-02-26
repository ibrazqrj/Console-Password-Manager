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
                int emptyFields = 9;

                for (int i = 0; i <= emptyFields; i++)
                {
                    PrintCentered.PrintTextCentered(" ");
                }
                PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORDMANAGER.-~*´¨¯¨`*·~-.¸··._.·´¯)");
                PrintMainMenu();
                PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-..-~*´¨¯¯¨`*·~-..-~*´¨¯¨`*·~-.¸··._.·´¯)");

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
                        SearchPassword(manager);
                        break;
                    case "6":
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

            string dev = "@ibrazqrj";

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
                key = Console.ReadKey(true); // Read key without displaying it

                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1]; // Remove last character
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                }

                // Get cursor position
                int startX = (consoleWidth / 2) - (pass.Length / 2);
                int startY = Console.CursorTop;

                // Clear previous line and reprint stars centered
                Console.SetCursorPosition(0, startY);
                Console.Write(new string(' ', consoleWidth)); // Erase old characters
                Console.SetCursorPosition(startX, startY);
                Console.Write(new string('*', pass.Length)); // Print masked input

            } while (key.Key != ConsoleKey.Enter); // Stop when Enter is pressed

            return pass;
        }


        // Hauptmenü anzeigen
        static void PrintMainMenu()
        {
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("1. Add password");
            PrintCentered.PrintTextCentered("2. Show passwords");
            PrintCentered.PrintTextCentered("3. Delete password");
            PrintCentered.PrintTextCentered("4. Generate password");
            PrintCentered.PrintTextCentered("5. Search password");
            PrintCentered.PrintTextCentered("6. Quit");
            PrintCentered.PrintTextCentered(" ");
        }

        // Passwort hinzufügen
        static void AddPassword(PasswordManagerOptions manager, byte[] aesKey)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.CREATE ENTRY.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered("Please fill all fields to save your password! If ");
            PrintCentered.PrintTextCentered(" ");

            // Benutzereingaben
            PrintCentered.PrintTextCentered("Webpage / URL:");
            string inputWebsite = Console.ReadLine();
            if (string.IsNullOrEmpty(inputWebsite))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }

            PrintCentered.PrintTextCentered("Username / E-Mail:");
            string inputUsername = Console.ReadLine();
            if (string.IsNullOrEmpty(inputUsername))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }

            PrintCentered.PrintTextCentered("Password:");
            string inputPassword = Console.ReadLine();
            if (string.IsNullOrEmpty(inputPassword))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }

            string encryptedPassword = AesEncryptionHelper.EncryptPassword(inputPassword, aesKey);

            // Passwort speichern
            manager.AddPassword(inputWebsite, inputUsername, encryptedPassword);

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Entry successfully added!");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        // Passwörter anzeigen
        static void ShowPasswords(PasswordManagerOptions manager, byte[] aesKey)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORDS.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered(" ");

            manager.listPasswords(aesKey);

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        // Passwort löschen
        static void DeletePassword(PasswordManagerOptions manager)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.DELETE ENTRY.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered(" ");

            // Passwörter anzeigen und zum Löschen auswählen
            manager.deletePassword();

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        static void GeneratePassword(PasswordManagerOptions manager)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.GENERATE PASSWORD.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered("Enter the desired password length:");

            if(int.TryParse(Console.ReadLine(), out int length) && length > 0)
            {
                string generatedPassword = manager.GeneratePassword(length);
                PrintCentered.PrintTextCentered($"Generated password: {generatedPassword}");
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

        static void SearchPassword(PasswordManagerOptions manager)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("Enter the website or username you are looking for:");
            manager.SearchPassword();

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }
        // Programm beenden
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
