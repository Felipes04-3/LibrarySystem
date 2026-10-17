namespace LibrarySystem {

    internal interface IBookItemRepository {

        void AddItem(BookItem item);
        void RemoveItem(int id);
        IReadOnlyList<BookItem> ListAllItems();
        BookItem? GetById(int? id);
    }
}
