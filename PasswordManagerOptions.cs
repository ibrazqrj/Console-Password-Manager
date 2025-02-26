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
        public string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "passwords.txt");


        public void AddPassword(string inputWebsite, string inputUsername, string inputPassword)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            File.AppendAllText(filePath, inputWebsite + " | " + inputUsername + " | " + inputPassword + Environment.NewLine);
        }

        public void listPasswords(byte[] aesKey)
        {
            var validPasswords = new List<string>();
            var partCount = 1;

            if (File.Exists(filePath))
            {
                var savedPasswords = File.ReadAllLines(filePath);
                foreach (string savedpws in savedPasswords)
                {
                    var parts = savedpws.Split("|");
                    if (parts.Length == 3)
                    {
                        string decryptedPassword = AesEncryptionHelper.DecryptPassword(parts[2], aesKey);
                        Console.WriteLine($"{partCount} | Website / URL : {parts[0],-30} | Username / E-Mail: {parts[1],-30} | Password: {decryptedPassword,-30}");
                        validPasswords.Add(savedpws);
                        partCount++;
                    }
                }
            }
        }

        public void deletePassword()
        {
            var validPasswords = new List<string>();
            var partCount = 1;

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
        }

        public string GeneratePassword(int length)
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

        public void SearchPassword()
        {
            string searchTerm = Console.ReadLine();

            if (File.Exists(filePath))
            {
                var savedPasswords = File.ReadAllLines(filePath);
                bool found = false;

                foreach (string savedpws in savedPasswords)
                {
                    if(savedpws.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        PrintCentered.PrintTextCentered($"Found: {savedpws}");
                        found = true;
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
        }
    }
}
