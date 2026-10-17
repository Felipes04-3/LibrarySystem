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
            Console.ForegroundColor = ConsoleColor.Magenta;
            return $"[ID: {FormattedId.PadRight(8)}] " +
                   $"[Date of Creation: {DataCreation}] | " +
                   $"[Title: {Title?.PadRight(15)}";
            
        }
    }
}
