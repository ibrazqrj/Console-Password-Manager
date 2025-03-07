using PasswordManager.Components;
using PasswordManager.Format;
using PasswordManager.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    class UIService
    {
        private PasswordService passwordService;
        private byte[] aesKey;

        public UIService(PasswordService service, byte[] key)
        {
            passwordService = service;
            aesKey = key;
        }

        public void StartMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\x1b[3J");
                EmptyFieldGenerator.GenerateFields(7);
                UIHelper.PrintTitle("PASSWORDMANAGER");

                EmptyFieldGenerator.GenerateFields(1);
                UIHelper.PrintTextCentered("1 | Add password          ");
                UIHelper.PrintTextCentered("2 | Show passwords        ");
                UIHelper.PrintTextCentered("3 | Delete password       ");
                UIHelper.PrintTextCentered("4 | Random password       ");
                UIHelper.PrintTextCentered("5 | Search password       ");
                UIHelper.PrintTextCentered("6 | Change master password");
                UIHelper.PrintTextCentered("7 | Quit                  ");
                EmptyFieldGenerator.GenerateFields(1);
                UIHelper.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-..-~*´¨¯¯¨`*·~-..-~*´¨¯¨`*·~-.¸··._.·´¯)");
                EmptyFieldGenerator.GenerateFields(1);

                UIHelper.PrintTextCentered("Your choice:");
                string choice = UIHelper.PrintInputCentered();


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
                        UIHelper.PrintTextCentered("Invalid input! Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddPasswordUI()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            UIHelper.PrintTitle("ADD NEW ENTRY");
            UIHelper.PrintTextCentered("Please fill all fields to save your password!");
            EmptyFieldGenerator.GenerateFields(1);

            string website = UIHelper.CenteredInput("Webpage / URL:");
            if (string.IsNullOrEmpty(website))
            {
                UIHelper.PrintTextCentered("This field can't be empty!");
                return;
            }
            EmptyFieldGenerator.GenerateFields(1);

            string username = UIHelper.CenteredInput("Username / E-Mail:");
            if (string.IsNullOrEmpty(username))
            {
                UIHelper.PrintTextCentered("This field can't be empty!");
                return;
            }
            EmptyFieldGenerator.GenerateFields(1);

            string password = UIHelper.CenteredInput("Password (or type in 'GeneratePassword' to generate one):");
            if (string.IsNullOrEmpty(password))
            {
                UIHelper.PrintTextCentered("This field can't be empty!");
                return;
            }
            EmptyFieldGenerator.GenerateFields(1);

            if (password.ToUpper() == "GENERATEPASSWORD")
            {
                var length = UIHelper.CenteredInput("Enter the desired password length (1-130):");
                if (int.TryParse(length, out int pwLength) && pwLength > 0 && pwLength <= 130)
                {
                    password = passwordService.GeneratePassword(pwLength);
                }
                else
                {
                    UIHelper.PrintTextCentered("Invalid input! Defaulting to 16 characters.");
                    password = passwordService.GeneratePassword(16);
                }
            }

            UIHelper.PrintTextCentered("Choose a category:");
            for (int i = 0; i < predefinedCategories.Count; i++)
            {
                UIHelper.PrintTextCentered($"{i + 1} | {predefinedCategories[i]}");
            }
            EmptyFieldGenerator.GenerateFields(1);
            UIHelper.PrintTextCentered("Enter the Number of your choice:");

            string categoryChoice = UIHelper.PrintInputCentered();
            int categoryIndex;

            if(!int.TryParse(categoryChoice, out categoryIndex) || categoryIndex < 1 || categoryIndex > predefinedCategories.Count)
            {
                UIHelper.PrintTextCentered("Invalid choice! Defaulting to 'Others'");
                categoryIndex = predefinedCategories.Count;
            }

            string category = predefinedCategories[categoryIndex - 1];

            passwordService.AddPassword(website, username, password, category, aesKey);

            EmptyFieldGenerator.GenerateFields(1);
            UIHelper.PrintTextCentered("Entry successfully added!");
            UIHelper.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        private readonly List<string> predefinedCategories = new List<string>
        {
            "Email       ",
            "Gaming      ",
            "Social Media",
            "Work        ",
            "Banking     ",
            "Shopping    ",
            "Others      "
        };

        private void ShowPasswordsUI()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("\x1b[3J");
            UIHelper.PrintTitle("SAVED PASSWORDS");
            EmptyFieldGenerator.GenerateFields(1);
            UIHelper.PrintSeparator();

            var passwords = passwordService.GetPasswords(aesKey);
            if (passwords.Count == 0)
            {
                UIHelper.PrintTextCentered("No passwords saved yet!");
                Console.ReadKey();
                return;
            }

            var categories = passwords.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();

            UIHelper.PrintTextCentered("Choose a category: ");
            EmptyFieldGenerator.GenerateFields(1);

            for (int i = 0; i < categories.Count; i++)
            {
                UIHelper.PrintTextCentered($"{i + 1} | {categories[i]}");
            }

            EmptyFieldGenerator.GenerateFields(1);
            UIHelper.PrintTextCentered("Enter the number of your choice: ");

            string choice = UIHelper.PrintInputCentered();

            if (!int.TryParse(choice, out int selectedIndex) || selectedIndex < 1 || selectedIndex > categories.Count)
            {
                UIHelper.PrintTextCentered("Invalid choice! Returning to menu.");
                Console.ReadKey();
                return;
            }

            string selectedCategory = categories[selectedIndex - 1];

            var filteredPasswords = passwords.Where(p => p.Category == selectedCategory).ToList();
            int totalEntries = filteredPasswords.Count;
            int pageSize = 4;
            int currentPage = 0;

            while (true)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                EmptyFieldGenerator.GenerateFields(3);
                UIHelper.PrintTitle($"SAVED PASSWORDS - CATEGORY: {selectedCategory}");
                EmptyFieldGenerator.GenerateFields(1);

                int startIndex = currentPage * pageSize;
                int endIndex = Math.Min(startIndex + pageSize, totalEntries);

                for (int i = startIndex; i < endIndex; i++)
                {
                    var entry = filteredPasswords[i];
                    UIHelper.PrintTextCentered($"Website: {entry.Website}");
                    UIHelper.PrintTextCentered($"Username: {entry.Username}");
                    UIHelper.PrintTextCentered($"Password: {entry.EncryptedPassword}");
                    UIHelper.PrintSeparator();
                }

                EmptyFieldGenerator.GenerateFields(2);
                UIHelper.PrintTextCentered($"Page {currentPage + 1} / {(totalEntries + pageSize - 1) / pageSize}");
                UIHelper.PrintTextCentered("[←] Previous  |  [→] Next  |  [Q] Quit");

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.RightArrow && endIndex < totalEntries)
                {
                    currentPage++;
                }
                else if ( key == ConsoleKey.LeftArrow && currentPage > 0)
                {
                    currentPage--;
                }
                else if (key == ConsoleKey.Q)
                {
                    break;
                }
            }


            EmptyFieldGenerator.GenerateFields(1);
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        private void DeletePasswordUI()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            UIHelper.PrintTitle("DELETE ENTRY");
            EmptyFieldGenerator.GenerateFields(1);

            UIHelper.PrintTextCentered("Choose the entry you want to delete:");
            EmptyFieldGenerator.GenerateFields(1);
            UIHelper.PrintSeparator();

            passwordService.DeletePassword(aesKey);

            EmptyFieldGenerator.GenerateFields(2);
            UIHelper.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        private void GeneratePasswordUI()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            EmptyFieldGenerator.GenerateFields(12);

            UIHelper.PrintTitle("GENERATE PASSWORD");
            UIHelper.PrintTextCentered("Enter the desired password length (1-130):");
            EmptyFieldGenerator.GenerateFields(1);

            string digitInput = UIHelper.PrintInputCentered();

            if (int.TryParse(digitInput, out int length) && length > 0 && length <= 130)
            {
                string generatedPassword = passwordService.GeneratePassword(length);
                EmptyFieldGenerator.GenerateFields(1);
                UIHelper.PrintTextCentered($"Generated password: {generatedPassword}");
            }
            else if (length > 130)
            {
                EmptyFieldGenerator.GenerateFields(1);
                UIHelper.PrintTextCentered("Your number input is too high!");
            }
            else
            {
                EmptyFieldGenerator.GenerateFields(1);
                UIHelper.PrintTextCentered("Invalid input!");
            }

            EmptyFieldGenerator.GenerateFields(1);
            UIHelper.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        private void SearchPasswordUI()
        {
            
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            EmptyFieldGenerator.GenerateFields(7);
            UIHelper.PrintTitle("SEARCH FOR AN ENTRY");
            EmptyFieldGenerator.GenerateFields(1);
            string searchTerm = UIHelper.CenteredInput("Enter the website or username you are looking for:");

            var results = passwordService.SearchPassword(searchTerm, aesKey);
            EmptyFieldGenerator.GenerateFields(1);
            UIHelper.PrintTextCentered("Result(s):");
            UIHelper.PrintSeparator();
            if(results.Count > 0)
            {
                foreach (var entry in results)
                {
                    UIHelper.PrintTextCentered($"Website: {entry.website}");
                    UIHelper.PrintTextCentered($"Username: {entry.username}");
                    UIHelper.PrintTextCentered($"Password: {entry.password}");
                    UIHelper.PrintSeparator();
                }
            }
            else
            {
                UIHelper.PrintTextCentered("No entries found!");
            }

            EmptyFieldGenerator.GenerateFields(1);

            UIHelper.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        private void RenewMasterPasswordUI()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            UIHelper.PrintTitle("CHANGE MASTER PASSWORD");

            string oldPassword = UIHelper.CenteredInput("Enter current master password:");

            EmptyFieldGenerator.GenerateFields(1);

            string newPassword = UIHelper.CenteredInput("Enter new master password:");

            EmptyFieldGenerator.GenerateFields(1);

            string confirmPassword = UIHelper.CenteredInput("Confirm new master password:");

            EmptyFieldGenerator.GenerateFields(1);

            if (newPassword != confirmPassword)
            {
                UIHelper.PrintTextCentered("Passwords do not match! Press any key to try again.");
                Console.ReadKey();
                return;
            }

            bool success = passwordService.RenewMasterPassword(oldPassword, newPassword, aesKey);

            if (success)
            {
                UIHelper.PrintTextCentered("Master password updated successfully!");
            }
            else
            {
                UIHelper.PrintTextCentered("Incorrect current master password! Press any key to try again.");
            }

            Console.ReadKey();
        }

        private void ExitProgramUI()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");

            EmptyFieldGenerator.GenerateFields(3);

            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣤⣀⢠⡤⠤⠖⠒⠒⠒⠲⣆⠀⠀⠀⠀⣾⠋⠉⠉⠛⢷⠀⣴⠖⠒⠤⣄⠀⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣤⠤⠶⢺⣾⣏⠁⠀⠀⣧⣼⣇⣀⠀⠀⠀⡀⠀⠘⡆⠀⠀⢰⣏⠀⠀⠀⠀⠘⣿⡟⠀⠀⢠⢃⣼⡏⠉⠙⢳⡆⠀⠀⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⣀⡤⠴⠒⠋⠙⣇⣿⠀⠀⠀⣿⣿⠀⠀⠀⢸⣿⣿⣿⠃⠀⢰⣿⡀⠀⠹⡄⠀⢸⣿⠀⠀⠀⠀⠀⢹⡇⠀⠀⢸⡿⣽⠀⠀⠀⡜⠀⣀⡤⠖⠓⠢⢤⣀⠀");
            UIHelper.PrintTextCentered("⣠⡴⠒⠉⠁⠀⠀⠀⠀⠀⠸⣿⡇⠀⠀⠘⠛⠃⠀⠀⠈⡟⠉⣿⠀⠀⠘⠛⠃⠀⠀⢷⠀⢸⣿⠀⠀⢠⡀⠀⠀⠀⠀⠀⣿⢧⡇⠀⠀⠸⠗⠚⠁⠀⠀⠀⣀⣠⣾⠃");
            UIHelper.PrintTextCentered("⣿⡇⠀⠀⠀⠀⠀⠀⣶⣶⣿⢿⢹⠀⠀⠀⢀⣀⠀⠀⠀⢳⠀⣿⠀⠀⢀⣀⣤⠀⠀⠘⣇⢸⡏⠀⠀⢸⣧⠀⠀⠀⠀⢸⣿⡿⠀⠀⢀⠀⠀⠀⢀⣤⣶⣿⠿⠛⠁⠀");
            UIHelper.PrintTextCentered("⢧⣹⣶⣾⣿⡄⠀⠀⠸⡟⠋⠘⡜⡆⠀⠀⢻⣿⡇⠀⠀⢸⡀⣿⠀⠀⢸⣿⡿⡇⠀⠀⢸⣿⡇⠀⠀⢸⡿⡆⠀⠀⠀⣾⣿⠃⠀⠀⣾⡇⠀⠀⠈⡟⠉⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠘⣿⡿⠿⢿⣧⠀⠀⠀⢳⡀⠀⣇⢱⠀⠀⠈⣿⣷⠀⣀⣸⣷⣿⣤⣤⣼⠋⣇⣹⣶⣶⣾⣿⡿⢲⣶⣾⡇⣿⣤⣀⣀⣿⡏⠀⠀⣼⡏⢧⠀⠀⠀⣇⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠸⡞⣇⠀⠀⠀⢧⠀⢸⣈⣷⣶⣶⣿⣿⣿⣿⣿⣿⣿⣽⣿⡏⢀⡼⠟⠛⠻⢿⡿⠿⠿⣿⣁⣿⣿⣿⣿⣿⣿⣿⣶⣴⢿⠁⢸⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⢹⣼⣦⣤⣶⣿⠁⣀⣿⠿⠿⣿⣫⣿⠉⠁⠀⠀⠀⡏⠀⣴⠏⠀⠀⠀⠀⠀⠹⣆⠀⢠⣿⠀⠀⠀⢈⠟⢻⡿⠿⣅⣘⡆⣸⣇⠀⠀⢸⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠻⠿⠿⠛⠃⢠⣿⣷⣄⠀⠈⠙⠋⠀⠀⠀⠀⣸⢁⡾⠁⠀⠀⣠⣤⡀⠀⠀⠸⣤⡞⡇⠀⠀⠀⢸⣰⣿⠃⠀⠀⢹⣿⣿⣿⣿⣦⣼⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⢿⣿⣿⣷⣄⠀⠀⠀⠀⠀⠀⣿⣾⠇⠀⠀⣸⣿⣿⢿⠀⠀⠀⣿⢁⡇⠀⠀⢀⣿⣿⡏⠀⠀⠀⡼⠀⢙⣿⠛⠻⣏⡀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢿⣿⣿⣷⠀⠀⠀⠀⢸⡿⡿⠀⠀⠀⡏⢹⠟⡟⠀⠀⠀⡿⢸⠀⠀⠀⢸⣿⡿⠀⠀⠀⢠⠇⡰⢋⡏⠀⠀⠀⢙⡆⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡿⡿⠀⠀⠀⠀⣸⡇⡇⠀⠀⠀⠻⠾⠞⠁⠀⠀⢀⡇⡏⠀⠀⠀⢸⣿⠃⠀⠀⠀⡼⣰⠃⡞⠀⠀⠀⠀⡾⠁⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡇⡇⠀⠀⠀⠀⣿⣇⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣃⡇⠀⠀⠀⠀⠀⠀⠀⠀⣼⣷⠃⣼⡀⠀⠀⢀⡞⠁⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⢸⠃⠀⠀⠀⢀⡇⢿⣿⣧⣀⠀⠀⠀⠀⠀⣠⣾⣿⣿⣧⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⣽⣿⣷⣤⡞⠁⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⣼⣤⣶⣶⣶⡿⠁⠈⢿⣿⣿⣿⣿⣿⣿⣿⠿⠃⢸⣿⣿⣷⣤⣄⣀⣀⣤⣾⣏⣤⡟⠁⠀⠈⠻⡍⠀⠀⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠿⠿⠿⠟⠛⠁⠀⠀⠀⠉⠛⠛⠛⠛⠉⠁⠀⠀⠀⠙⠿⢿⣿⣿⡿⠿⠋⢀⣿⣿⣧⡀⠀⠀⣠⡇⠀⠀⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣿⣿⣿⠟⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            UIHelper.PrintTextCentered("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");

            UIHelper.PrintTextCentered("Password manager shutting down...");
            UIHelper.PrintTextCentered("Thank you for using the Password Manager.");
            UIHelper.PrintTextCentered("Press any key to exit.");

            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
