namespace LibrarySystem {

    public interface IUserRepository {

        void AddUser(User user, string providedKey, User actor);
        void RemoveUser(int id, string rpovidedKey, User actor);
        IReadOnlyList<User> ListAllUsers();
        User? GetUserId(int? id);
    }
}
