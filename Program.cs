namespace PasswordManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Fenstergröße einstellen
            Console.SetWindowSize(150, 30);

            // Instanz des Passwortmanagers
            PasswordManagerOptions manager = new PasswordManagerOptions();
            MasterPasswordHelper passwordHelper = new MasterPasswordHelper();

            createMasterPassword(passwordHelper);
            loginWithMasterPassword(passwordHelper);

            // ASCII-Logo und Startbildschirm
            DisplayStartScreen();

            while (true)
            {
                Console.Clear();
                PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORDMANAGER.-~*´¨¯¨`*·~-.¸··._.·´¯)");
                PrintMainMenu();

                string choice = Console.ReadLine();

                // Menü-Optionen
                switch (choice)
                {
                    case "1":
                        AddPassword(manager);
                        break;
                    case "2":
                        ShowPasswords(manager);
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
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered(" ");

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
            string start = "PRESS ENTER TO START!";

            // Logo und Entwicklername zentrieren
            foreach (string line in logo)
            {
                PrintCentered.PrintTextCentered(line);
            }

            PrintCentered.PrintTextCentered(dev);
            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered(start);
            Console.ReadLine();
        }

        static void createMasterPassword(MasterPasswordHelper passwordHelper)
        {
            passwordHelper.HashPassword();
        }

        static void loginWithMasterPassword(MasterPasswordHelper passwordHelper)
        {
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORD MANAGER.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered("Please login.");
            PrintCentered.PrintTextCentered("");
            PrintCentered.PrintTextCentered("Enter your master password:");

            string pwInput = Console.ReadLine();

            passwordHelper.VerifyMasterPassword(pwInput);
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
        static void AddPassword(PasswordManagerOptions manager)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.CREATE ENTRY.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered("Please fill all fields to save your password!");
            PrintCentered.PrintTextCentered(" ");

            // Benutzereingaben
            PrintCentered.PrintTextCentered("Webpage / URL:");
            string inputWebsite = Console.ReadLine();

            PrintCentered.PrintTextCentered("Username / E-Mail:");
            string inputUsername = Console.ReadLine();

            PrintCentered.PrintTextCentered("Password:");
            string inputPassword = Console.ReadLine();

            // Passwort speichern
            manager.AddPassword(inputWebsite, inputUsername, inputPassword);

            PrintCentered.PrintTextCentered(" ");
            PrintCentered.PrintTextCentered("Entry successfully added!");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        // Passwörter anzeigen
        static void ShowPasswords(PasswordManagerOptions manager)
        {
            Console.Clear();
            PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORDS.-~*´¨¯¨`*·~-.¸··._.·´¯)");
            PrintCentered.PrintTextCentered(" ");

            manager.listPasswords();

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
