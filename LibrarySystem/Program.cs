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

while (exit == 1) {

    try {

        User userLogged = InputHandler.SetupSystem(manager);

        InputHandler.MainMenu(manager, userLogged);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Deseja fechar o sistema completamente? [s/n]");
        Console.ResetColor();
        if (Console.ReadLine()?.ToLower() == "s") break;
    }
    catch (NavigationException) {

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("--- Operação cancelada. Retornando ao início do sistema ---");
        Console.ResetColor();

        string? option = InputHelper.ExitStringOption("Gostaria de encerrar o sistema? ");

        if (option.Equals("s", StringComparison.OrdinalIgnoreCase)) {

            exit = 0;
            break;
        }

        continue;
        
    }
    catch (Exception ex) {

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Erro: {ex.Message}");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nPressione qualquer tecla para reiniciar...");
        Console.ResetColor();
        Console.ReadKey();
    }
}