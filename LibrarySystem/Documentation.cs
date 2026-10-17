// Documentation and project reference (do not uncomment)
// This file contains the project's source code reordered by recommended structure.
// Order: usings -> exceptions -> helpers -> domain models -> interfaces -> repositories -> services -> UI -> program

/*
// Program.cs (entry point)
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
global using LibrarySystem.Exceptions;
global using LibrarySystem;
global using LibrarySystem.Helpers;

IBookItemRepository bookRepo = new BookItemRepository();
IUserRepository userRepo = new UserRepository();

OrderManager manager = new OrderManager(bookRepo, userRepo);

Console.ReadKey();

int exit = 1;

while (true) {
    try {
        User userLogged = InputHandler.SetupSystem(manager);
        InputHandler.MainMenu(manager, userLogged);

        Console.WriteLine("Deseja fechar o sistema completamente? [s/n]");
        if (Console.ReadLine()?.ToLower() == "s") break;
    }
    catch (NavigationException) {
        Console.Clear();
        Console.WriteLine("--- Operação cancelada. Retornando ao início do sistema ---");
    }
    catch (Exception ex) {
        Console.WriteLine($"Erro: {ex.Message}");
        Console.WriteLine("Pressione qualquer tecla para reiniciar...");
        Console.ReadKey();
    }
}

// NavigationException.cs
namespace LibrarySystem.Exceptions {
    public class NavigationException : Exception {
        public NavigationException(string message) : base(message) { }
        public NavigationException() : base() { }
    }
}

// Helpers: InputHelper.cs
using LibrarySystem.Exceptions;

namespace LibrarySystem.Helpers {
    public static class InputHelper {
        public static string ExitOption(string message) {
            Console.Write($"{message} ");
            string? input = Console.ReadLine().Trim();
            if (input == "0") {
                LoadingWait();
                throw new NavigationException();
            }
            return input ?? string.Empty;
        }

        public static int ExitIntOption(string message) {
            try {
                string intText = ExitOption(message).Trim();
                if (int.TryParse(intText, out int number)) return number;
                Console.WriteLine("Ops! Digite um número válido.");
            }
            catch(Exception ex) {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            return ExitIntOption(message);
        }

        public static string ExitStringOption(string message) {
            try {
                string? input = ExitOption(message).Trim();
                if (!(int.TryParse(input, out _))) return input;
                Console.WriteLine("Ops! Digite um texto válido.");
            }
            catch(Exception ex) {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            return ExitStringOption(message);
        }

        public static void LoadingWait(string message = "Voltando ao menu", int dots = 3) {
            Console.Write(message);
            for (int i = 0; i < dots; i++) {
                Thread.Sleep(500);
                Console.Write(" .");
            }
            Console.WriteLine();
            Thread.Sleep(200);
        }
    }
}

// Domain: BookItem.cs
namespace LibrarySystem {
    internal abstract class BookItem {
        private static int _identify = 2300;
        public string? Title { get; set; }
        public DateTime DataCreation { get; }
        public int Id { get; private set; }

        protected BookItem() {
            this.Id = _identify++;
            this.DataCreation = DateTime.Now;
        }

        public string FormattedId => $"00{Id}";

        protected abstract string ValidateTitle(string item);

        public override string ToString() {
            return $"[ID: {FormattedId.PadRight(8)}] " +
                   $"[Date of Creation: {DataCreation}] | " +
                   $"[Title: {Title?.PadRight(15)}";
        }
    }
}

// Domain: FantasyBooks.cs
namespace LibrarySystem {
    internal class FantasyBooks : BookItem {
        public string Descriptions { get; set; }

        public FantasyBooks(string title, string text) {
            this.Title = ValidateTitle(title);
            Descriptions = SetDescriptions(text);
        }

        protected override string ValidateTitle(string item) {
            int minLength = 4, maxLength = 15;
            if (!string.IsNullOrWhiteSpace(item)) {
                item = string.Concat(item[0].ToString().ToUpper(), item.AsSpan(1));
            }
            else if (Title?.Length < minLength && Title.Length > maxLength) {
                throw new ArgumentException($"Title must be between {minLength} and {maxLength} characters.");
            }
            return item;
        }

        private static string SetDescriptions(string text) {
            int minLength = 10, maxLength = 50;
            if (!string.IsNullOrWhiteSpace(text)) {
                throw new ArgumentException("Description must not be empty", nameof(text));
            }
            else if (!(text.Length > minLength && text.Length < maxLength)) {
                throw new ArgumentException("Description length must be between minimun of 10 to maximum of 50 characters", nameof(text));
            }
            return text;
        }

        public static string ValidateDescription(string description) {
            SetDescriptions(description);
            return description;
        }

        public string GetDescription => Descriptions;
    }
}

// Interfaces
namespace LibrarySystem {
    internal interface IBookItemRepository {
        void AddItem(BookItem item);
        void RemoveItem(int id);
        IReadOnlyList<BookItem> ListAllItems();
        BookItem? GetById(int? id);
    }
}

namespace LibrarySystem {
    public interface IUserRepository {
        void AddUser(User user, string providedKey, User actor);
        void RemoveUser(int id, string rpovidedKey, User actor);
        IReadOnlyList<User> ListAllUsers();
        User? GetUserId(int? id);
    }
}

// Repositories
namespace LibrarySystem {
    internal class BookItemRepository : IBookItemRepository {
        private readonly List<BookItem> _bookcase = new ();
        public BookItemRepository() { }
        public void AddItem(BookItem item) { _bookcase.Add(item); }
        public void RemoveItem(int id) {
            var item = GetById(id);
            if(item != null) { _bookcase.Remove(item); }
        }
        public IReadOnlyList<BookItem> ListAllItems() { return _bookcase; }
        protected BookItemRepository(BookItem books) { }
        public BookItem? GetById(int? id) { return _bookcase.FirstOrDefault(x => x.Id == id); }
    }
}

namespace LibrarySystem {
    internal class UserRepository : IUserRepository {
        private readonly List<User> _users = new();
        public UserRepository() { }
        public void AddUser(User user, string providedKey, User actor) { _users.Add(user); }
        public void RemoveUser(int id, string providedKey, User actor) { var user = GetUserId(id); _users.Remove(user); }
        public IReadOnlyList<User> ListAllUsers() { return _users; }
        public User? GetUserId(int? id) {
            if (id == null) { throw new ArgumentNullException("Id is invalid!", nameof(id)); }
            return _users.FirstOrDefault(x => x.Id == id);
        }
    }
}

// Domain: User.cs
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

        public void PromoteToAdmin() { this.IsAdmin = true; }
        public static bool IsMasterKeyValid(string key) { return MasterKey == key; }
        public string GetFormattedId => $"00{Id}";

        private string SetPassword(string password) {
            if (password == null) { throw new ArgumentNullException("Password cannot be empty.", nameof(password)); }
            return password;
        }

        private string SetName(string name) {
            int minLength = 4, maxLength = 15;
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException("Name must not be empty.", nameof(name)); }
            if (name.Length < minLength || name.Length > maxLength) { throw new ArgumentException($"Name length must be between {minLength} to {maxLength} characters.", nameof(name)); }
            name = string.Concat(name[0].ToString().ToUpper(), name.AsSpan(1));
            return name;
        }

        private string SetEmail(string email) {
            if (string.IsNullOrWhiteSpace(email)) { throw new ArgumentNullException("Email must not be empty.", nameof(email)); }
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) { throw new ArgumentException("Email format is invalid."); }
            return email;
        }

        public override string ToString() {
            string role = IsAdmin ? "ADMIN" : "USER";
            string hiddenPassword = new string('*', Password?.Length ?? 0);
            return $"[ID: {GetFormattedId.PadRight(8)}] " +
                   $"Name: {Name?.PadRight(15)} | " +
                   $"Password: {hiddenPassword.PadRight(10)} | " +
                   $"Role: {role}";
        }
    }
}

// Service: OrderManager.cs
using System.Xml.Linq;

namespace LibrarySystem {
    internal class OrderManager : IBookItemRepository, IUserRepository {
        private readonly IBookItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;

        public OrderManager(IBookItemRepository repository, IUserRepository user) { _itemRepository = repository; _userRepository = user; }

        public void RegisterFirstAdmin(User newUser) {
            if (ListAllUsers().Count > 0)
                throw new UnauthorizedAccessException("Already exists registered users.");
            newUser.PromoteToAdmin();
            _userRepository.AddUser(newUser, User.MasterKey, newUser);
        }

        public void AddItem(BookItem newItem) {
            var savedItems = ListAllItems();
            foreach (var book in savedItems) {
                if (string.Equals(book?.Title, newItem.Title, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("This book already exists.", nameof(newItem));
            }
            _itemRepository.AddItem(newItem);
        }

        public void AddUser(User newUser, string providedKey, User actor) {
            if (actor == null || !actor.IsAdmin)
                throw new UnauthorizedAccessException("Only administrators can add new users.");
            if (!User.IsMasterKeyValid(providedKey))
                throw new UnauthorizedAccessException("MasterKey is invalid.");
            var savedUsers = ListAllUsers();
            foreach (var user in savedUsers) {
                if (string.Equals(user?.Name, newUser.Name, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("This user already exists.", nameof(newUser));
            }
            Console.WriteLine($"User {newUser.Name} added by {actor.Name}!");
            _userRepository.AddUser(newUser, User.MasterKey, actor);
        }

        public void RemoveItem(int id) { var item = GetById(id); _itemRepository.RemoveItem(id); }

        public void RemoveUser(int id, string providedKey, User actor) {
            if (actor == null || !actor.IsAdmin)
                throw new UnauthorizedAccessException("Only administrators can remove users.");
            if (!User.IsMasterKeyValid(providedKey)) 
                throw new UnauthorizedAccessException("MasterKey is invalid.");
            var userToDelete = GetUserId(id);
            if (actor.Id == id) 
                throw new ArgumentException("The administrator cannot remove your own account.");
            _userRepository.RemoveUser(id, User.MasterKey, actor);
            Console.WriteLine($"User {userToDelete?.Name} (ID: {id}) successfully removed by {actor.Name}.");
        }

        public IReadOnlyList<BookItem> ListAllItems() {
            var items = _itemRepository.ListAllItems();
            if (items.Count == 0) throw new ArgumentException("No items added yet.");
            return items;
        }

        public IReadOnlyList<User> ListAllUsers() { return _userRepository.ListAllUsers(); }

        public BookItem? GetById(int? id) {
            if (id == null) throw new ArgumentException("Id must not be null.");
            var item = _itemRepository.ListAllItems().FirstOrDefault(x => x.Id == id);
            if (item == null) throw new KeyNotFoundException($"No item found with {id}");
            return item;
        }

        public User? GetUserId(int? id) {
            if (id == null) throw new ArgumentException("User id must not be null.");
            var user = _userRepository.ListAllUsers().FirstOrDefault(x => x.Id == id);
            if (user == null) throw new KeyNotFoundException($"No user id found with {id}");
            return user;
        }

        public User? ToLogin(string name, string password) {
            var anyUser = ListAllUsers();
            if (anyUser is null) { throw new ArgumentException("No user registered found."); }
            return anyUser.FirstOrDefault(user => user.Name == name && user.Password == password);
        }
    }
}

// UI: InputHandler.cs (main methods shown)
using LibrarySystem;
using LibrarySystem.Helpers;

namespace LibrarySystem {
    internal class InputHandler {
        public InputHandler() { }

        public static void MainMenu(OrderManager manager, User userLogged) {
            bool stayInMenu = true;
            while (stayInMenu) {
                try {
                    Console.Clear();
                    UserLoggedMenu(userLogged);
                    Console.WriteLine("========================================");
                    Console.WriteLine("       SISTEMA DE GESTÃO - BIBLIOTECA    ");
                    Console.WriteLine("========================================");
                    Console.WriteLine(" [1] Ir para Menu de Usuários");
                    Console.WriteLine(" [2] Logar Outro Usuário");
                    Console.WriteLine(" [3] Listar Itens do Acervo");
                    Console.WriteLine(" [4] Adicionar Novo Livro");
                    Console.WriteLine(" [5] Pegar Livro pelo ID");
                    Console.WriteLine(" [6] Buscar Livro Específico");
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine(" [0] Sair do Programa");
                    Console.WriteLine("========================================");

                    int choice = InputHelper.ExitIntOption("Escolha uma opção: ");

                    switch (choice)
                    {
                        case 1:
                            InputHelper.LoadingWait("Carregando", 3);
                            var listAllUsers = manager.ListAllUsers();
                            foreach (var user in listAllUsers) { Console.WriteLine(user); }
                            ControllerUserMenu(userLogged, manager);
                            break;

                        case 2:
                            Console.Clear();
                            userLogged = SetupSystem(manager);
                            break;

                        case 3:
                            var list = manager.ListAllItems();
                            if (list != null) {
                                foreach (var item in list) { Console.WriteLine(item); }
                            }
                            break;

                        case 4:
                            string title = InputHelper.ExitStringOption("Adicione o título da obra: ");
                            string description = InputHelper.ExitStringOption("Dê uma breve descrição à obra: ");
                            description = FantasyBooks.ValidateDescription(description).Trim();
                            var newBook = new FantasyBooks(title, description);
                            manager.AddItem(newBook);
                            Console.WriteLine($"Livro {newBook.Title} adicionado com sucesso!");
                            InputHelper.LoadingWait("Retornando ao menu", 3);
                            break;

                        case 5:
                            Console.WriteLine("Saiba que ação irá indisponibilizar acessos posteriores.");
                            Console.WriteLine("Será necessário devolver/adicionar a obra novamente.");
                            int id = InputHelper.ExitIntOption("[0 = não] ou digite o ID. Prosseguir? ");
                            InputHelper.LoadingWait("Aguarde", 3);
                            manager.RemoveItem(id);
                            break;

                        case 6:
                            var itemList = manager.ListAllItems();
                            string bookName = InputHelper.ExitStringOption("Digite o nome da obra: ");
                            bookName = $"{char.ToUpper(bookName[0])}{bookName.AsSpan(1)}";
                            foreach (var book in itemList) { if (book.Title == bookName) { Console.WriteLine(book); } }
                            break;

                        case 0:
                            stayInMenu = false;
                            break;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Erro: {ex.Message}");
                    Console.ReadKey();
                    continue;
                }
            }
        }

        // ControllerUserMenu, UserLoggedMenu and SetupSystem are implemented in the original project files.
    }
}
*/