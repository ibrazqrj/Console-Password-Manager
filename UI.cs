using PasswordManager.Components;
using PasswordManager.Format;
using PasswordManager.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    class UI
    {
        private PasswordService passwordService;
        private byte[] aesKey;

        public UI(PasswordService service, byte[] key)
        {
            passwordService = service;
            aesKey = key;
        }

        public void StartMenu()
        {
            while (true)
            {
                Console.Clear();
                EmptyFieldGenerator.GenerateFields(7);
                PrintCentered.PrintTitle("PASSWORDMANAGER");

                EmptyFieldGenerator.GenerateFields(1);
                PrintCentered.PrintTextCentered("1 | Add password          ");
                PrintCentered.PrintTextCentered("2 | Show passwords        ");
                PrintCentered.PrintTextCentered("3 | Delete password       ");
                PrintCentered.PrintTextCentered("4 | Random password       ");
                PrintCentered.PrintTextCentered("5 | Search password       ");
                PrintCentered.PrintTextCentered("6 | Change master password");
                PrintCentered.PrintTextCentered("7 | Quit                  ");
                EmptyFieldGenerator.GenerateFields(1);
                PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-..-~*´¨¯¯¨`*·~-..-~*´¨¯¨`*·~-.¸··._.·´¯)");
                EmptyFieldGenerator.GenerateFields(1);

                PrintCentered.PrintTextCentered("Your choice:");
                string choice = PrintCentered.PrintInputCentered();


                switch (choice)
                {
                    case "1":
                        AddPasswordUI();
                        break;
                    case "2":
                        ShowPasswordsUI();
                        break;
                    case "3":
                        DeletePasswordUI();
                        break;
                    case "4":
                        GeneratePasswordUI();
                        break;
                    case "5":
                        SearchPasswordUI();
                        break;
                    case "6":
                        RenewMasterPasswordUI();
                        break;
                    case "7":
                        ExitProgramUI();
                        break;
                    default:
                        PrintCentered.PrintTextCentered("Invalid input! Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddPasswordUI()
        {
            Console.Clear();
            PrintCentered.PrintTitle("ADD NEW ENTRY");
            PrintCentered.PrintTextCentered("Please fill all fields to save your password!");
            EmptyFieldGenerator.GenerateFields(1);

            string website = PrintCentered.CenteredInput("Webpage / URL:");
            if (string.IsNullOrEmpty(website))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            EmptyFieldGenerator.GenerateFields(1);

            string username = PrintCentered.CenteredInput("Username / E-Mail:");
            if (string.IsNullOrEmpty(username))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            EmptyFieldGenerator.GenerateFields(1);

            string password = PrintCentered.CenteredInput("Password:");
            if (string.IsNullOrEmpty(password))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            PrintCentered.PrintTextCentered(" ");

            passwordService.AddPassword(website, username, password, aesKey);

            EmptyFieldGenerator.GenerateFields(1);
            PrintCentered.PrintTextCentered("Entry successfully added!");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        private void ShowPasswordsUI()
        {
            Console.Clear();
            EmptyFieldGenerator.GenerateFields(7);
            PrintCentered.PrintTitle("SAVED PASSWORDS");
            EmptyFieldGenerator.GenerateFields(1);
            PrintCentered.PrintSeparator();
            var passwords = passwordService.GetPasswords(aesKey);
            if (passwords.Count == 0)
            {
                PrintCentered.PrintTextCentered("No passwords saved yet!");
            }
            else
            {
                foreach (var (website, username, password) in passwords)
                {
                    PrintCentered.PrintTextCentered("Website: " + website);
                    PrintCentered.PrintTextCentered("Username: " + username);
                    PrintCentered.PrintTextCentered("Password: " + password);
                    PrintCentered.PrintSeparator();
                }
            }

            passwordService.GetPasswords(aesKey);

            EmptyFieldGenerator.GenerateFields(1);
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        private void DeletePasswordUI()
        {
            Console.Clear();
            PrintCentered.PrintTitle("DELETE ENTRY");
            EmptyFieldGenerator.GenerateFields(1);

            PrintCentered.PrintTextCentered("Choose the entry you want to delete:");
            EmptyFieldGenerator.GenerateFields(1);
            PrintCentered.PrintSeparator();

            passwordService.DeletePassword(aesKey);

            EmptyFieldGenerator.GenerateFields(2);
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        private void GeneratePasswordUI()
        {
            Console.Clear();
            EmptyFieldGenerator.GenerateFields(12);

            PrintCentered.PrintTitle("GENERATE PASSWORD");
            PrintCentered.PrintTextCentered("Enter the desired password length (1-130):");
            EmptyFieldGenerator.GenerateFields(1);

            string digitInput = PrintCentered.PrintInputCentered();

            if (int.TryParse(digitInput, out int length) && length > 0 && length <= 130)
            {
                string generatedPassword = passwordService.GeneratePassword(length);
                EmptyFieldGenerator.GenerateFields(1);
                PrintCentered.PrintTextCentered($"Generated password: {generatedPassword}");
            }
            else if (length > 130)
            {
                EmptyFieldGenerator.GenerateFields(1);
                PrintCentered.PrintTextCentered("Your number input is too high!");
            }
            else
            {
                EmptyFieldGenerator.GenerateFields(1);
                PrintCentered.PrintTextCentered("Invalid input!");
            }

            EmptyFieldGenerator.GenerateFields(1);
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        private void SearchPasswordUI()
        {
            
            Console.Clear();
            EmptyFieldGenerator.GenerateFields(7);
            PrintCentered.PrintTitle("SEARCH FOR AN ENTRY");
            EmptyFieldGenerator.GenerateFields(1);
            string searchTerm = PrintCentered.CenteredInput("Enter the website or username you are looking for:");

            var results = passwordService.SearchPassword(searchTerm, aesKey);
            EmptyFieldGenerator.GenerateFields(1);
            PrintCentered.PrintTextCentered("Result(s):");
            PrintCentered.PrintSeparator();
            if(results.Count > 0)
            {
                foreach (var entry in results)
                {
                    PrintCentered.PrintTextCentered($"Website: {entry.website}");
                    PrintCentered.PrintTextCentered($"Username: {entry.username}");
                    PrintCentered.PrintTextCentered($"Password: {entry.password}");
                    PrintCentered.PrintSeparator();
                }
            }
            else
            {
                PrintCentered.PrintTextCentered("No entries found!");
            }

            EmptyFieldGenerator.GenerateFields(1);

            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }

        private void RenewMasterPasswordUI()
        {
            Console.Clear();
            PrintCentered.PrintTitle("CHANGE MASTER PASSWORD");

            string oldPassword = PrintCentered.CenteredInput("Enter current master password:");

            EmptyFieldGenerator.GenerateFields(1);

            string newPassword = PrintCentered.CenteredInput("Enter new master password:");

            EmptyFieldGenerator.GenerateFields(1);

            string confirmPassword = PrintCentered.CenteredInput("Confirm new master password:");

            EmptyFieldGenerator.GenerateFields(1);

            if (newPassword != confirmPassword)
            {
                PrintCentered.PrintTextCentered("Passwords do not match! Press any key to try again.");
                Console.ReadKey();
                return;
            }

            bool success = passwordService.RenewMasterPassword(oldPassword, newPassword, aesKey);

            if (success)
            {
                PrintCentered.PrintTextCentered("Master password updated successfully!");
            }
            else
            {
                PrintCentered.PrintTextCentered("Incorrect current master password! Press any key to try again.");
            }

            Console.ReadKey();
        }

        private void ExitProgramUI()
        {
            Console.Clear();

            EmptyFieldGenerator.GenerateFields(3);

            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣤⣀⢠⡤⠤⠖⠒⠒⠒⠲⣆⠀⠀⠀⠀⣾⠋⠉⠉⠛⢷⠀⣴⠖⠒⠤⣄⠀⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣤⠤⠶⢺⣾⣏⠁⠀⠀⣧⣼⣇⣀⠀⠀⠀⡀⠀⠘⡆⠀⠀⢰⣏⠀⠀⠀⠀⠘⣿⡟⠀⠀⢠⢃⣼⡏⠉⠙⢳⡆⠀⠀⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⣀⡤⠴⠒⠋⠙⣇⣿⠀⠀⠀⣿⣿⠀⠀⠀⢸⣿⣿⣿⠃⠀⢰⣿⡀⠀⠹⡄⠀⢸⣿⠀⠀⠀⠀⠀⢹⡇⠀⠀⢸⡿⣽⠀⠀⠀⡜⠀⣀⡤⠖⠓⠢⢤⣀⠀");
            PrintCentered.PrintTextCentered("⣠⡴⠒⠉⠁⠀⠀⠀⠀⠀⠸⣿⡇⠀⠀⠘⠛⠃⠀⠀⠈⡟⠉⣿⠀⠀⠘⠛⠃⠀⠀⢷⠀⢸⣿⠀⠀⢠⡀⠀⠀⠀⠀⠀⣿⢧⡇⠀⠀⠸⠗⠚⠁⠀⠀⠀⣀⣠⣾⠃");
            PrintCentered.PrintTextCentered("⣿⡇⠀⠀⠀⠀⠀⠀⣶⣶⣿⢿⢹⠀⠀⠀⢀⣀⠀⠀⠀⢳⠀⣿⠀⠀⢀⣀⣤⠀⠀⠘⣇⢸⡏⠀⠀⢸⣧⠀⠀⠀⠀⢸⣿⡿⠀⠀⢀⠀⠀⠀⢀⣤⣶⣿⠿⠛⠁⠀");
            PrintCentered.PrintTextCentered("⢧⣹⣶⣾⣿⡄⠀⠀⠸⡟⠋⠘⡜⡆⠀⠀⢻⣿⡇⠀⠀⢸⡀⣿⠀⠀⢸⣿⡿⡇⠀⠀⢸⣿⡇⠀⠀⢸⡿⡆⠀⠀⠀⣾⣿⠃⠀⠀⣾⡇⠀⠀⠈⡟⠉⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠘⣿⡿⠿⢿⣧⠀⠀⠀⢳⡀⠀⣇⢱⠀⠀⠈⣿⣷⠀⣀⣸⣷⣿⣤⣤⣼⠋⣇⣹⣶⣶⣾⣿⡿⢲⣶⣾⡇⣿⣤⣀⣀⣿⡏⠀⠀⣼⡏⢧⠀⠀⠀⣇⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠸⡞⣇⠀⠀⠀⢧⠀⢸⣈⣷⣶⣶⣿⣿⣿⣿⣿⣿⣿⣽⣿⡏⢀⡼⠟⠛⠻⢿⡿⠿⠿⣿⣁⣿⣿⣿⣿⣿⣿⣿⣶⣴⢿⠁⢸⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⢹⣼⣦⣤⣶⣿⠁⣀⣿⠿⠿⣿⣫⣿⠉⠁⠀⠀⠀⡏⠀⣴⠏⠀⠀⠀⠀⠀⠹⣆⠀⢠⣿⠀⠀⠀⢈⠟⢻⡿⠿⣅⣘⡆⣸⣇⠀⠀⢸⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠻⠿⠿⠛⠃⢠⣿⣷⣄⠀⠈⠙⠋⠀⠀⠀⠀⣸⢁⡾⠁⠀⠀⣠⣤⡀⠀⠀⠸⣤⡞⡇⠀⠀⠀⢸⣰⣿⠃⠀⠀⢹⣿⣿⣿⣿⣦⣼⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⢿⣿⣿⣷⣄⠀⠀⠀⠀⠀⠀⣿⣾⠇⠀⠀⣸⣿⣿⢿⠀⠀⠀⣿⢁⡇⠀⠀⢀⣿⣿⡏⠀⠀⠀⡼⠀⢙⣿⠛⠻⣏⡀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢿⣿⣿⣷⠀⠀⠀⠀⢸⡿⡿⠀⠀⠀⡏⢹⠟⡟⠀⠀⠀⡿⢸⠀⠀⠀⢸⣿⡿⠀⠀⠀⢠⠇⡰⢋⡏⠀⠀⠀⢙⡆⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡿⡿⠀⠀⠀⠀⣸⡇⡇⠀⠀⠀⠻⠾⠞⠁⠀⠀⢀⡇⡏⠀⠀⠀⢸⣿⠃⠀⠀⠀⡼⣰⠃⡞⠀⠀⠀⠀⡾⠁⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡇⡇⠀⠀⠀⠀⣿⣇⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣃⡇⠀⠀⠀⠀⠀⠀⠀⠀⣼⣷⠃⣼⡀⠀⠀⢀⡞⠁⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⢸⠃⠀⠀⠀⢀⡇⢿⣿⣧⣀⠀⠀⠀⠀⠀⣠⣾⣿⣿⣧⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⣽⣿⣷⣤⡞⠁⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⣼⣤⣶⣶⣶⡿⠁⠈⢿⣿⣿⣿⣿⣿⣿⣿⠿⠃⢸⣿⣿⣷⣤⣄⣀⣀⣤⣾⣏⣤⡟⠁⠀⠈⠻⡍⠀⠀⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠿⠿⠿⠟⠛⠁⠀⠀⠀⠉⠛⠛⠛⠛⠉⠁⠀⠀⠀⠙⠿⢿⣿⣿⡿⠿⠋⢀⣿⣿⣧⡀⠀⠀⣠⡇⠀⠀⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣿⣿⣿⠟⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            PrintCentered.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");

            PrintCentered.PrintTextCentered("Password manager shutting down...");
            PrintCentered.PrintTextCentered("Thank you for using the Password Manager.");
            PrintCentered.PrintTextCentered("Press any key to exit.");

            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
