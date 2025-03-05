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
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Fenstergröße einstellen
            Console.SetWindowSize(150, 30);

            // Instanz des Passwortmanagers
            MasterPasswordHelper passwordHelper = new MasterPasswordHelper();

            passwordHelper.HashPassword();
            string masterPassword = passwordHelper.Login();

            byte[] aesKey = AesEncryptionHelper.DeriveKeyFromPassword(masterPassword);
            PasswordService service = new PasswordService();
            UI ui = new UI(service, aesKey);

            ui.StartMenu();
        }
    }
}
