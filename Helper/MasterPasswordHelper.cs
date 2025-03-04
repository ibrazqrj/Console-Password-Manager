using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Format;
using Microsoft.VisualBasic;
using PasswordManager.Components;

namespace PasswordManager.Helper
{
    class MasterPasswordHelper
    {
        private readonly string filePath;
        private readonly Pbkdf2 pbkdf2 = new();

        public MasterPasswordHelper()
        {
            string folderPath = ConstantsCustom.FolderPath;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            filePath = ConstantsCustom.MasterFilePath;
        }

        public void HashPassword()
        {

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            if (new FileInfo(filePath).Length == 0)
            {

                Console.Clear();
                EmptyFieldGenerator.generateFields(12);
                
                PrintCentered.PrintTitle("PASSWORD MANAGER");
                PrintCentered.PrintTextCentered("To use the Password manager, you need to create a master password. Don't worry, your master password will get hashed and can't be unhashed anymore.");
                EmptyFieldGenerator.generateFields(1);

                string prompt = "Enter your master password:";
                int screenWidth = Console.WindowWidth;
                int textWidth = prompt.Length;
                int leftPadding = (screenWidth - textWidth) / 2 ;

                Console.SetCursorPosition(leftPadding, Console.CursorTop);
                Console.Write(prompt);

                string masterpassword = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(masterpassword))
                {
                    PrintCentered.PrintTextCentered("Master password cannot be empty! Please try again.");
                    return;
                }

                byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
                string saltBase64 = Convert.ToBase64String(salt);

                Pbkdf2 pbkdf2 = new Pbkdf2();
                string hashed = pbkdf2.hash(masterpassword, salt); 

                string hashCombinesSalt = $"{saltBase64}:{hashed}";

                File.AppendAllText(filePath, hashCombinesSalt);
                Console.Clear();
            }
        }


        public void VerifyMasterPassword(string inputPassword)
        {
            int emptyFields = 12;

            Console.Clear();

            if (string.IsNullOrWhiteSpace(inputPassword))
            {
                PrintCentered.PrintTextCentered("Master password cannot be empty! Please try again.");
                return;
            }
            else
            {
                if (!File.Exists(filePath))
                {
                    Environment.Exit(0);
                }

                string[] parts = File.ReadAllText(filePath).Split(':');

                if (parts.Length != 2)
                {
                    Environment.Exit(0);
                }

                string storedSalt = parts[0];
                string storedHash = parts[1];
                string storedHashCombinedSalt = $"{storedSalt}:{storedHash}";
                byte[] convertedSalt = Convert.FromBase64String(storedSalt);
                string saltBase64 = Convert.ToBase64String(convertedSalt);

                Pbkdf2 pbkdf2 = new Pbkdf2();
                string hashed = pbkdf2.hash(inputPassword, convertedSalt);

                string hashCombinedSalt = $"{saltBase64}:{hashed}";

                if (hashCombinedSalt != storedHashCombinedSalt)
                {
                    for (int i = 0; i <= emptyFields; i++)
                    {
                        EmptyFieldGenerator.generateFields(1);
                    }
                    PrintCentered.PrintTextCentered("Master password is wrong!");
                    Environment.Exit(0);
                    Console.Clear();
                }
                Console.Clear();
            }
        }

        public void ChangeMasterPassword()
        {
            if (File.Exists(filePath))
            {
                if (new FileInfo(filePath).Length != 0)
                {

                    Console.Clear();

                    PrintCentered.PrintTitle("Password Manager");
                    PrintCentered.PrintTextCentered("Please enter your new master password you want to use for the future.");
                    EmptyFieldGenerator.generateFields(1);
                    PrintCentered.PrintTextCentered("Enter your new master password:");

                    string masterpassword = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(masterpassword))
                    {
                        PrintCentered.PrintTextCentered("Master password cannot be empty! Please try again.");
                        return;
                    } 

                    byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
                    string saltBase64 = Convert.ToBase64String(salt);

                    Pbkdf2 pbkdf2 = new Pbkdf2();
                    string hashed = pbkdf2.hash(masterpassword, salt);

                    string hashCombinesSalt = $"{saltBase64}:{hashed}";

                    File.WriteAllText(filePath, hashCombinesSalt);
                    Console.Clear();
                }
            }
        }
    }
}
