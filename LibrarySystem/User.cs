using System.Text.RegularExpressions;

namespace LibrarySystem {

    public class User {

        public static string MasterKey { get; } = "040felipe";
        private static int _id = 1310;

        public bool IsAdmin { get; private set; }

        public string? Name { get; private set; }
        public string? Password { get; private set; }
        public string? Email { get; private set; }
        public int Id { get; private set; }
        public DateTime DataCreation { get; }

        public User(string name, string password, string email, bool isAdmin = false) {

            Name = SetName(name);
            Password = SetPassword(password);
            Email = SetEmail(email);
            Id = _id++;
            DataCreation = DateTime.Now;
            IsAdmin = isAdmin;
        }


        public void PromoteToAdmin() {
            this.IsAdmin = true;
        }


        public static bool IsMasterKeyValid(string key) {

            return MasterKey == key;
        }


        public string GetFormattedId => $"00{Id}";


        private string SetPassword(string password) {

            if (password == null) {

                throw new ArgumentNullException("Password cannot be empty.", nameof(password));
            }

            return password;
        }


        private string SetName(string name) {

            int minLength = 4, maxLength = 15;

            if (string.IsNullOrWhiteSpace(name)) {

                throw new ArgumentNullException("Name must not be empty.", nameof(name));
            }

            if (name.Length < minLength || name.Length > maxLength) {

                throw new ArgumentException($"Name length must be between {minLength} to {maxLength} characters.", nameof(name));
            }

            name = string.Concat(name[0].ToString().ToUpper(), name.AsSpan(1));

            return name;
        }


        private string SetEmail(string email) {

            if (string.IsNullOrWhiteSpace(email)) {

                throw new ArgumentNullException("Email must not be empty.", nameof(email));
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Email format is invalid.");
            }

            return email;
        }


        public override string ToString() {

            string role = IsAdmin ? "ADMIN" : "USUÁRIO";

            string hiddenPassword = new string('*', Password?.Length ?? 0);

            return $"[ID: {GetFormattedId.PadRight(8)}] " +
                   $"NOME: {Name?.PadRight(15)} | " +
                   $"SENHA: {hiddenPassword.PadRight(10)} | " +
                   $"FUNÇÃO: {role}";
        }
    }
}
