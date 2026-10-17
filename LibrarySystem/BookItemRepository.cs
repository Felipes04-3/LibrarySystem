namespace LibrarySystem {

    internal class BookItemRepository : IBookItemRepository {

        private readonly List<BookItem> _bookcase = new ();

  
        public BookItemRepository() { }


        public void AddItem(BookItem item) {

            _bookcase.Add(item);
        }


        public void RemoveItem(int id) {

            var item = GetById(id);

            if(item != null) {

                _bookcase.Remove(item);
            }
        }


        public IReadOnlyList<BookItem> ListAllItems() {

            return _bookcase;
        }


        protected BookItemRepository(BookItem books) {


        }


        public BookItem? GetById(int? id) {

            return _bookcase.FirstOrDefault(x => x.Id == id);
        }
    }
}