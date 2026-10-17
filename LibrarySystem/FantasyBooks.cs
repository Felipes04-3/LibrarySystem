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

            if (string.IsNullOrWhiteSpace(text)) {

                throw new ArgumentException("Description must not be empty", nameof(text));
            }

            else if (!(text.Length >= minLength && text.Length <= maxLength)) {

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
