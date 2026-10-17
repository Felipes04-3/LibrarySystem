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

                    int choice = InputHelper.ExitIntOption("\nEscolha uma opção: ");

                    switch (choice)
                    {

                        case 1:
                            InputHelper.LoadingWait("Carregando", 3);

                            var listAllUsers = manager.ListAllUsers();

                            foreach (var user in listAllUsers) {

                                Console.WriteLine(user);
                            }

                            ControllerUserMenu(userLogged, manager);
                            break;

                        case 2:
                            userLogged = SetupSystem(manager);

                            if (userLogged == null) {

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Nenhum usuário encontrado com os dados passados.");
                                Console.ResetColor();
                                continue;
                            }
                            else {

                                UserLoggedMenu(userLogged);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Usuário: {userLogged} logado com sucesso.");
                                Console.ResetColor();
                            }
                            break;

                        case 3:
                            var list = manager.ListAllItems();

                            if (list.Count == 0) { throw new ArgumentException("No items added yet."); }
                            else {

                                foreach (var item in list) {

                                    Console.WriteLine(item);
                                }

                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Pressione qualquer tecla para continuar...");
                                Console.ResetColor();
                                Console.ReadKey();
                            }
                            break;

                        case 4:
                            string title = InputHelper.ExitStringOption("\nAdicione o título da obra: ");
                            string description = InputHelper.ExitStringOption("\nDê uma breve descrição à obra: ");

                            description = FantasyBooks.ValidateDescription(description).Trim();

                            var newBook = new FantasyBooks(title, description);

                            manager.AddItem(newBook);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Livro {newBook.Title} adicionado com sucesso!");
                            Console.ResetColor();

                            InputHelper.LoadingWait("Retornando ao menu", 4);
                            break;

                        case 5:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Saiba que ação irá indisponibilizar acessos posteriores.");
                            Console.WriteLine("Será necessário devolver/adicionar a obra novamente.");
                            Console.ResetColor();

                            int id = InputHelper.ExitIntOption("\n[0 = não] ou digite o ID. Prosseguir? ");
                            InputHelper.LoadingWait("Aguarde", 3);

                            manager.RemoveItem(id);
                            break;

                        case 6:
                            var itemList = manager.ListAllItems();
                            string bookName = InputHelper.ExitStringOption("\nDigite o nome da obra: ");

                            bookName = $"{char.ToUpper(bookName[0])}{bookName.AsSpan(1)}";

                            foreach (var book in itemList) {

                                if (book.Title == bookName) {

                                    Console.WriteLine(book);
                                }
                            }
                            break;

                        case 0:
                            stayInMenu = false;
                            break;
                    }
                }
                catch (NavigationException)
                {
                    // propagate cancellation to the top-level program handler
                    throw;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{ex.Message}");
                    Console.ResetColor();
                    Console.ReadKey();
                    continue;
                }
            }
        }
   

        public static void ControllerUserMenu(User userLogged, OrderManager manager) {

            bool stayInUserMenu = true;

            while (stayInUserMenu) {

                try {

                    Console.Clear();

                    UserLoggedMenu(userLogged);
                    Console.WriteLine("========================================");
                    Console.WriteLine("       Usuários Cadastrados    ");
                    Console.WriteLine("========================================");
                    Console.WriteLine(" [1] Listar Usuários");
                    Console.WriteLine(" [2] Adicionar Novo Usuário");
                    Console.WriteLine(" [3] Remover Usuário");
                    Console.WriteLine(" [4] Buscar Usuário Existente");
                    Console.WriteLine(" [5] Voltar ao Menu Principal");
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine(" [0] Sair do Programa");
                    Console.WriteLine("========================================");

                    int userMenuOptions = InputHelper.ExitIntOption("\nAlguma ação a ser feita? ");

                    switch (userMenuOptions) {

                        case 1:
                            var userList = manager.ListAllUsers();

                            foreach (var user in userList) Console.WriteLine(user);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Pressione qualque tecla para continuar...");
                            Console.ResetColor();
                            Console.ReadKey();
                            break;

                        case 2:
                            string name = InputHelper.ExitStringOption("\nDigite o nome: ");
                            string password = InputHelper.ExitStringOption("\nDigite a senha: ");
                            string email = InputHelper.ExitStringOption("\nDigite o email: ");

                            var newUser = new User(name, password, email);
                            string masterKey = InputHelper.ExitStringOption("\nSenha de admin: ");
                            manager.AddUser(newUser, masterKey, userLogged);

                            InputHelper.LoadingWait("Voltando ao menu de usuário", 3);
                            break;

                        case 3:
                            string adminPass = InputHelper.ExitStringOption("\nSenha de admin: ");
                            int idToRemove = InputHelper.ExitIntOption("\nID do usuário a remover: ");

                            InputHelper.ExitOption("\nConfirma a remoção? (Qualquer tecla para sim/ 0 para cancelar)");

                            InputHelper.ExitIntOption("\nDigite o ID do usuário a ser digitado: ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            manager.RemoveUser(idToRemove, adminPass, userLogged);
                            Console.ResetColor();

                            InputHelper.LoadingWait(" ", 3);
                            break;

                        case 4:
                            string nameSearch = InputHelper.ExitStringOption("\nNome do usuário: ");

                            nameSearch = string.Concat(nameSearch[0].ToString().ToUpper(), nameSearch.AsSpan(1));

                            var foundUsers = manager.ListAllUsers().Where(u => u.Name.Contains(nameSearch));
                            foreach (var u in foundUsers) Console.WriteLine(u);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\nPressione qualquer tecla para continuar");
                            Console.ResetColor();
                            Console.ReadKey();
                            break;

                        case 5:
                            stayInUserMenu = false;
                            break;

                        default:
                            break;
                    }
                }
                catch (NavigationException)
                {
                    // propagate cancellation to the top-level program handler
                    throw;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{ex.Message}");
                    Console.ResetColor();
                    Console.ReadKey();
                }
            }
        }


        public static void UserLoggedMenu(User loggedUser) {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("====================================================");
            Console.WriteLine($" USER: {loggedUser.Name?.ToUpper()} | ID: {loggedUser.GetFormattedId}");
            Console.WriteLine($" ACCESS: {loggedUser.Email}");
            Console.WriteLine($" CREATION DATE: {loggedUser.DataCreation:dd/MM/yyyy}");
            Console.WriteLine("====================================================");
            Console.ResetColor();
        }

        public static User SetupSystem(OrderManager manager) {

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("||===============================================||");
            Console.WriteLine("||-SEJA BEM VINDO AO SISTEMA DE GESTÃO - LIBRARY-||");
            Console.WriteLine("||===============================================||");
            Console.ResetColor();

            var userList = manager.ListAllUsers();

            if (userList != null && userList.Count > 0 ) {

                // Keep prompting for credentials until a valid user logs in or the user cancels (ExitOption throws NavigationException on "0").
                while (true) {
                    string name = InputHelper.ExitStringOption("\nDigite seu nome de usuário: ");
                    string password = InputHelper.ExitStringOption("\nDigite sua senha: ");

                    var admin = manager.ToLogin(name, password);

                    if (admin != null) return admin;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Nenhum usuário encontrado com os dados passados. Tente novamente ou digite 0 para cancelar.");
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Antes de começar, vamos cadastrar o administrador");
            Console.ResetColor();

            InputHelper.LoadingWait("Inicializando dados admin", 3);

            bool success = false;

            while (!success) {

                try {

                    string name = InputHelper.ExitStringOption("\nDigite seu nome de admin:");
                    string email = InputHelper.ExitStringOption("\nDigite seu e-mail:");
                    InputHelper.LoadingWait("senha admin configurada por default");
                    string password = "040felipe";

                    User admin = new User(name, password, email);

                    manager.RegisterFirstAdmin(admin);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Admin configurado com sucesso.");
                    Console.ResetColor();

                    InputHelper.LoadingWait("Carregando o sistema", 3);

                    success = true;

                    return admin;
                }
                catch (NavigationException)
                {
                    // user requested cancellation (entered 0) - let Program handle it
                    throw;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}. Tente novamente.");
                    Console.ResetColor();
                }
            }

            return null;
        }
    }
}
