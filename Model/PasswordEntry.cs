using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Model
{
    class PasswordEntry
    {
        public string Website { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public string Category { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PasswordEntry other)
            {
                return Website == other.Website &&
                       Username == other.Username &&
                       EncryptedPassword == other.EncryptedPassword &&
                       Category == other.Category;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Website, Username, EncryptedPassword, Category);
        }
    }
}
