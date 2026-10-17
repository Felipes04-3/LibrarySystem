namespace LibrarySystem {

    internal class UserRepository : IUserRepository {

        private readonly List<User> _users = new();

        public UserRepository() { }


        public void AddUser(User user, string providedKey, User actor) {

            _users.Add(user);
        }


        public void RemoveUser(int id, string providedKey, User actor) {

            var user = GetUserId(id);
            _users.Remove(user);
        }


        public IReadOnlyList<User> ListAllUsers() {

            return _users;
        }


        public User? GetUserId(int? id) {

            if (id == null) {

                throw new ArgumentNullException("Id is invalid!", nameof(id));
            }

            return _users.FirstOrDefault(x => x.Id == id);
        }
    }
}
