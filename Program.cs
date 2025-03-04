using PasswordManager.Format;
using PasswordManager.Helper;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            string masterPassword = loginWithMasterPassword(passwordHelper);
            byte[] aesKey = AesEncryptionHelper.DeriveKeyFromPassword(masterPassword);

            while (true)
            {
                Console.Clear();
                EmptyFieldGenerator.generateFields(8);

                PrintMainMenu();


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

        static void DisplayStartScreen()
        {
            Menu.showLogin();
        }

        static void createMasterPassword(MasterPasswordHelper passwordHelper)
        {
            passwordHelper.HashPassword();
        }

        static string loginWithMasterPassword(MasterPasswordHelper passwordHelper)
        {
            DisplayStartScreen();
            PrintCentered.PrintTitle("PASSWORD MANAGER");
            PrintCentered.PrintTextCentered("Please login.");
            EmptyFieldGenerator.generateFields(1);
            PrintCentered.PrintTextCentered("Enter your master password:");

            string pwInput = Password.HidePassword();

            passwordHelper.VerifyMasterPassword(pwInput);

            return pwInput;
        }


        static void PrintMainMenu()
        {
            Menu.showMainMenu();
        }

        static void AddPassword(PasswordManagerOptions manager, byte[] aesKey)
        {
            PasswordManagerOptions.AddPassword(aesKey);
        }

        static void ShowPasswords(PasswordManagerOptions manager, byte[] aesKey)
        {
            manager.listPasswords(aesKey);
        }

        static void DeletePassword(PasswordManagerOptions manager)
        {
            manager.deletePassword();
        }

        static void GeneratePassword(PasswordManagerOptions manager)
        {
            PasswordManagerOptions.GeneratePassword();
        }

        static void SearchPassword(PasswordManagerOptions manager, byte[] aesKey)
        {
            manager.SearchPassword(aesKey);
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
