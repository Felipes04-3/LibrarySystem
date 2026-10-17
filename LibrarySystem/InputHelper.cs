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

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ops! Digite um número válido.");
                Console.ResetColor();
            }
            catch(NavigationException) {
                // Propagate navigation cancel so caller can handle exiting the app
                throw;
            }
            catch(Exception ex) {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro: {ex.Message}");
                Console.ResetColor();
                return 0;
            }

            return ExitIntOption(message);
        }


        public static string ExitStringOption(string message) {

            try {

                string? input = ExitOption(message).Trim();

                if (!(int.TryParse(input, out _))) return input;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ops! Digite um texto válido.");
                Console.ResetColor();
            }
            catch(NavigationException) {
                // Propagate navigation cancel so caller can handle exiting the app
                throw;
            }
            catch(Exception ex) {

                Console.WriteLine($"Erro: {ex.Message}");
            }

            return ExitStringOption(message);
        }


        public static void LoadingWait(string message = "Voltando ao menu", int dots = 3) {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            Console.ResetColor();

            for (int i = 0; i < dots; i++) {

                Thread.Sleep(500);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" .");
                Console.ResetColor();
            }

            Console.WriteLine();
            Thread.Sleep(200);
        }
    }
}