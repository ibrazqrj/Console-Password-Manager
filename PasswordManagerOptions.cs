using PasswordManager.Components;
using PasswordManager.Format;
using PasswordManager.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace PasswordManager
{
    class PasswordManagerOptions
    {
        public string websiteUrl { get; private set; }
        public string username { get; private set; }
        public string password { get; private set; }
        public string filePath { get; private set; }

        public PasswordManagerOptions()
        {
            filePath = ConstantsCustom.PasswordFilePath;
        }


        public static void AddPassword(byte[] aesKey)
        {
            Console.Clear();
            PrintCentered.PrintTitle("ADD NEW ENTRY");
            PrintCentered.PrintTextCentered("Please fill all fields to save your password!");
            EmptyFieldGenerator.generateFields(1);

            string website = PrintCentered.CenteredInput("Webpage / URL:");
            if (string.IsNullOrEmpty(website))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            EmptyFieldGenerator.generateFields(1);

            string username = PrintCentered.CenteredInput("Username / E-Mail:");
            if (string.IsNullOrEmpty(username))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            EmptyFieldGenerator.generateFields(1);

            string password = PrintCentered.CenteredInput("Password:");
            if (string.IsNullOrEmpty(password))
            {
                PrintCentered.PrintTextCentered("This field can't be empty!");
                return;
            }
            PrintCentered.PrintTextCentered(" ");
            string encryptedUsername = AesEncryptionHelper.EncryptPassword(username, aesKey);
            string encryptedPassword = AesEncryptionHelper.EncryptPassword(password, aesKey);

            if (!File.Exists(ConstantsCustom.PasswordFilePath))
            {
                File.Create(ConstantsCustom.PasswordFilePath).Close();
            }

            File.AppendAllText(ConstantsCustom.PasswordFilePath, website + " | " + encryptedUsername + " | " + encryptedPassword + Environment.NewLine);

            EmptyFieldGenerator.generateFields(1);
            PrintCentered.PrintTextCentered("Entry successfully added!");
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }


        public void listPasswords(byte[] aesKey)
        {
            var validPasswords = new List<string>();
            var partCount = 1;

            Console.Clear();
            PrintCentered.PrintTitle("PASSWORDS");
            EmptyFieldGenerator.generateFields(1);
            EmptyFieldGenerator.generateFields(1);

            if (File.Exists(filePath))
            {
                var savedPasswords = File.ReadAllLines(filePath);
                foreach (string savedpws in savedPasswords)
                {
                    var parts = savedpws.Split("|");
                    if (parts.Length == 3)
                    {
                        string decryptedUsername = AesEncryptionHelper.DecryptPassword(parts[1], aesKey);
                        string decryptedPassword = AesEncryptionHelper.DecryptPassword(parts[2], aesKey);
                        Console.WriteLine($"{partCount} | Website / URL : {parts[0],-30} | Username / E-Mail: {decryptedUsername,-30} | Password: {decryptedPassword,-30}");
                        validPasswords.Add(savedpws);
                        partCount++;
                    }
                }
            }

            EmptyFieldGenerator.generateFields(1);

            EmptyFieldGenerator.generateFields(1);
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            EmptyFieldGenerator.generateFields(1);

            Console.ReadKey();
            Console.Clear();
        }


        public void deletePassword()
        {
            var validPasswords = new List<string>();
            var partCount = 1;

            Console.Clear();
            PrintCentered.PrintTitle("DELETE ENTRY");
            PrintCentered.PrintTextCentered(" ");

            if (File.Exists(filePath))
            {
                var savedPasswords = File.ReadAllLines(filePath);
                foreach (string savedpws in savedPasswords)
                {
                    var parts = savedpws.Split("|");
                    if (parts.Length == 3)
                    {
                        Console.WriteLine($"{partCount} | Website / URL : {parts[0],-30} | Username / E-Mail: {parts[1],-30} | Password: {parts[2],-30}");
                        validPasswords.Add(savedpws);
                        partCount++;
                    }
                }
                Console.WriteLine("Choose the entry you want to delete:");
                if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= savedPasswords.Length)
                {
                    validPasswords.RemoveAt(index - 1);
                    File.WriteAllLines(filePath, validPasswords);
                }
                else
                {
                    Console.WriteLine("Invalid input! Please try again.");
                }
            }

            EmptyFieldGenerator.generateFields(1);
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }


        public static void GeneratePassword()
        {
            Console.Clear();
            EmptyFieldGenerator.generateFields(12);

            PrintCentered.PrintTitle("GENERATE PASSWORD");
            PrintCentered.PrintTextCentered("Enter the desired password length (1-130):");
            EmptyFieldGenerator.generateFields(1);

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

                int startX = consoleWidth / 2 - digitInput.Length / 2;
                int startY = Console.CursorTop;

                Console.SetCursorPosition(0, startY);
                Console.Write(new string(' ', consoleWidth));
                Console.SetCursorPosition(startX, startY);
                Console.Write(digitInput);

            } while (key.Key != ConsoleKey.Enter);

            if (int.TryParse(digitInput, out int length) && length > 0 && length <= 130)
            {
                string generatedPassword = PasswordLogic(length);
                EmptyFieldGenerator.generateFields(1);
                PrintCentered.PrintTextCentered($"Generated password: {generatedPassword}");
            }
            else if (length > 130)
            {
                EmptyFieldGenerator.generateFields(1);
                PrintCentered.PrintTextCentered("Your number input is too high!");
            }
            else
            {
                EmptyFieldGenerator.generateFields(1);
                PrintCentered.PrintTextCentered("Invalid input!");
            }

            EmptyFieldGenerator.generateFields(1);
            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }


        public static string PasswordLogic(int length)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$/&";

            string allChars = upper + lower + digits + special;
            Random random = new Random();
            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password);
        }


        public void SearchPassword(byte[] aesKey)
        {
            Console.Clear();
            PrintCentered.PrintTitle("SEARCH FOR AN ENTRY");
            EmptyFieldGenerator.generateFields(1);

            string searchTerm = PrintCentered.CenteredInput("Enter the website or username you are looking for:");
            EmptyFieldGenerator.generateFields(2);
            PrintCentered.PrintSeparator();
            EmptyFieldGenerator.generateFields(1);

            if (File.Exists(filePath))
            {
                var savedPasswords = File.ReadAllLines(filePath);
                bool found = false;

                foreach (string savedpws in savedPasswords)
                {
                    var parts = savedpws.Split("|");
                    if (parts.Length == 3)
                    {
                        try
                        {
                            string decryptedUsername = AesEncryptionHelper.DecryptPassword(parts[1].Trim(), aesKey);
                            string decryptedPassword= AesEncryptionHelper.DecryptPassword(parts[2].Trim(), aesKey);

                            if (parts[0].Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                decryptedUsername.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                decryptedPassword.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                            {
                                PrintCentered.PrintTextCentered($"Found: Website: {parts[0]} | Username: {decryptedUsername} | Password: {decryptedPassword}");
                                found = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error decrypting entry: {ex.Message}");
                        }
                    }
                }
                if (!found)
                {
                    PrintCentered.PrintTextCentered("No matchin entries found.");
                }
            }
            else
            {
                Console.WriteLine("No passwords stored yet.");
            }

            EmptyFieldGenerator.generateFields(1);
            PrintCentered.PrintSeparator();
            EmptyFieldGenerator.generateFields(1);

            PrintCentered.PrintTextCentered("Press a random key to return to the menu.");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
