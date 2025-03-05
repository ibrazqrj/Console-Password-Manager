using PasswordManager.Components;
using PasswordManager.Format;
using PasswordManager.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    class PasswordService
    {
        private string filePath = ConstantsCustom.PasswordFilePath;

        public void AddPassword(string website, string username, string password, byte[] aesKey)
        {
            string encryptedUsername = AesEncryptionHelper.EncryptPassword(username, aesKey);
            string encryptedPassword = AesEncryptionHelper.EncryptPassword(password, aesKey);

            if (!File.Exists(ConstantsCustom.PasswordFilePath))
            {
                File.Create(ConstantsCustom.PasswordFilePath).Close();
            }

            File.AppendAllText(ConstantsCustom.PasswordFilePath, website + " | " + encryptedUsername + " | " + encryptedPassword + Environment.NewLine);
        }

        public List<(string website, string username, string password)> GetPasswords(byte[] aesKey)
        {
            var passwordList = new List<(string, string, string)>();

            if (File.Exists(filePath))
            {
                var savedPasswords = File.ReadAllLines(filePath);
                foreach (var line in savedPasswords)
                {
                    var parts = line.Split("|");
                    if (parts.Length == 3)
                    {
                        string decryptedUsername = AesEncryptionHelper.DecryptPassword(parts[1], aesKey);
                        string decryptedPassword = AesEncryptionHelper.DecryptPassword(parts[2], aesKey);
                        passwordList.Add((parts[0], decryptedUsername, decryptedPassword));
                    }
                }
            }

            return passwordList;
        }

        public void DeletePassword(byte[] aesKey)
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
                        string decryptedUsername = AesEncryptionHelper.DecryptPassword(parts[1], aesKey);
                        string decryptedPassword = AesEncryptionHelper.DecryptPassword(parts[2], aesKey);
                        PrintCentered.PrintTextCentered($"{partCount, -100}");
                        PrintCentered.PrintTextCentered($"Website: {parts[0]}");
                        PrintCentered.PrintTextCentered($"Username: {decryptedUsername}");
                        PrintCentered.PrintTextCentered($"Password: {decryptedPassword}");
                        PrintCentered.PrintSeparator();
                        validPasswords.Add(savedpws);
                        partCount++;
                    }
                }

                EmptyFieldGenerator.GenerateFields(1);
                PrintCentered.PrintTextCentered("Your choice:");
                if (int.TryParse(PrintCentered.PrintInputCentered(), out int index) && index > 0 && index <= savedPasswords.Length)
                {
                    validPasswords.RemoveAt(index - 1);
                    File.WriteAllLines(filePath, validPasswords);
                }
                else
                {
                    EmptyFieldGenerator.GenerateFields(1);
                    PrintCentered.PrintTextCentered("Invalid input! Please try again.");
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

        public List<(string website, string username, string password)> SearchPassword(string searchTerm, byte[] aesKey)
        {
            var matchingEntries = new List<(string, string, string)>();

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
                            string decryptedPassword = AesEncryptionHelper.DecryptPassword(parts[2].Trim(), aesKey);

                            if (parts[0].Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                decryptedUsername.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                decryptedPassword.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                            {
                                matchingEntries.Add((parts[0], decryptedUsername, decryptedPassword));
                            }
                        }
                        catch (Exception ex)
                        {
                            PrintCentered.PrintTextCentered($"Error decrypting entry: {ex.Message}");
                        }
                    }
                }
            }
            return matchingEntries;
        }

        public bool RenewMasterPassword(string oldPassword, string newPassword, byte[] aesKey)
        {
            string filePath = ConstantsCustom.MasterFilePath;

            if (!File.Exists(filePath))
            {
                PrintCentered.PrintTextCentered("No master password set yet. Please set one first.");
                return false;
            }

            string hashedPassword = File.ReadAllText(filePath);
            string[] parts = hashedPassword.Split(":");
            if (parts.Length !=2)
            {
                return false;
            }

            byte[] storedSalt = Convert.FromBase64String(parts[0]);
            string storedHash = parts[1];

            Pbkdf2 pbkdf2 = new Pbkdf2();
            string computedHash = pbkdf2.Hash(oldPassword, storedSalt);

            if (storedHash != computedHash)
            {
                return false; 
            }

            byte[] newSalt = RandomNumberGenerator.GetBytes(128 / 8);
            string newSaltBase64 = Convert.ToBase64String(newSalt);

            string newHash = pbkdf2.Hash(newPassword, newSalt);

            string newHashCombined = $"{newSaltBase64}:{newHash}";

            File.WriteAllText(filePath, newHashCombined);

            return true; 
        }
    }
}
 