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


        public void RemoveItem(int id) {

            var item = GetById(id);

            _itemRepository.RemoveItem(id);
        }


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

            return _itemRepository.ListAllItems();
        }


        public IReadOnlyList<User> ListAllUsers() {

            return _userRepository.ListAllUsers();
        }


        public BookItem? GetById(int? id) {

            if (id == null) 
                throw new ArgumentException("Id must not be null.");

            var item = _itemRepository.ListAllItems().FirstOrDefault(x => x.Id == id);

            if (item == null)
                throw new KeyNotFoundException($"No item found with {id}");
            
            return item;
        }


        public User? GetUserId(int? id) {

            if (id == null) 
                throw new ArgumentException("User id must not be null.");

            var user = _userRepository.ListAllUsers().FirstOrDefault(x => x.Id == id);

            if (user == null)
                throw new KeyNotFoundException($"No user id found with {id}");


            return user;
        }

        public User? ToLogin(string name, string password) {

            var anyUser = ListAllUsers();

            if (anyUser == null) {

                throw new ArgumentNullException("No user registered found.");
            }

            return anyUser.FirstOrDefault(user => user.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && user.Password == password);
        }
    }
}