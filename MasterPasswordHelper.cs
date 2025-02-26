using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    class MasterPasswordHelper
    {
        public string filePath { get; private set; }

        public MasterPasswordHelper()
        {
            string folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PMiz");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            filePath = Path.Combine(folderPath, "master.txt");
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
                int emptyFields = 12;

                for (int i = 0; i <= emptyFields; i++)
                {
                    PrintCentered.PrintTextCentered(" ");
                }
                PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORD MANAGER.-~*´¨¯¨`*·~-.¸··._.·´¯)");
                PrintCentered.PrintTextCentered("To use the Password manager, you need to create a master password. Don't worry, your master password will get hashed and can't be unhashed anymore.");
                PrintCentered.PrintTextCentered("");
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

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: masterpassword!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

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

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: inputPassword!,
                    salt: convertedSalt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                string hashCombinedSalt = $"{saltBase64}:{hashed}";

                if(hashCombinedSalt != storedHashCombinedSalt)
                {
                    for (int i = 0; i <= emptyFields; i++)
                    {
                        PrintCentered.PrintTextCentered(" ");
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

                    PrintCentered.PrintTextCentered("(¯`·._.··¸.-~*´¨¯¨`*·~-.PASSWORD MANAGER.-~*´¨¯¨`*·~-.¸··._.·´¯)");
                    PrintCentered.PrintTextCentered("Please enter your new master password you want to use for the future.");
                    PrintCentered.PrintTextCentered("");
                    PrintCentered.PrintTextCentered("Enter your new master password:");

                    string masterpassword = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(masterpassword))
                    {
                        PrintCentered.PrintTextCentered("Master password cannot be empty! Please try again.");
                        return;
                    }

                    byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
                    string saltBase64 = Convert.ToBase64String(salt);

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: masterpassword!,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8));

                    string hashCombinesSalt = $"{saltBase64}:{hashed}";

                    File.WriteAllText(filePath, hashCombinesSalt);
                    Console.Clear();
                }
            }
        }
    }
}
