// See https://aka.ms/new-console-template for more information

using ConsoleApp;
using ConsoleUI;
using DAL;
using Domain;
using GameEngine;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

class Program
{
    private static GameOptions? gameOptions;
    private static IGameRepository? gameRepository;

    static void Main()
    {
        // ================== SETUP =====================
        var gameOptions = new GameOptions();

        var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared";
        connectionString = connectionString.Replace("<%temppath%>", Path.GetTempPath());

        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        //system will dispose (close down) the db connection when this block of code is over
        //using var db = new AppDbContext(contextOptions);

        gameRepository = CreateGameRepositoryEF(connectionString);

        var mainMenu = ProgramMenus.GetMainMenu(
            gameOptions,
            ProgramMenus.GetOptionsMenu(gameOptions),
            NewGame,
            LoadGame,
            ChangeRepository,
            About
        );

        // ================== MAIN =====================
        while (true)
        {
            var result = mainMenu.Run();
            if (result == "RETURN_TO_MAIN_MENU")
            {
                break;
            }
        }

        mainMenu.Run();

        // ================ THE END ==================
        return;


        // ================== NEW GAME =====================
        string? NewGame()
        {
            // game logic, shared between console and web
            var gameEngine = new UnoGameEngine(gameOptions);

            ProgramMenus.ChoosePlayType(gameEngine);

            // set up players
            //PlayerSetup.ConfigurePlayers(gameEngine);

            // set up the table
            gameEngine.ShuffleAndDistributeCards();

            // console controller for game loop
            var gameController = new GameController(gameEngine, gameRepository);
            gameEngine.State.IsInitialised = true;
            gameController.Run();
            return null;
        }

        // ================== LOAD GAME =====================
        string? LoadGame()
        {
            Console.WriteLine("Saved games");
            var saveGameList = gameRepository?.GetSavedGameIdentifiers();

            if (saveGameList?.Count == 0)
            {
                Console.WriteLine("No saved games found.");
                return null;
            }

            var saveGameListDisplay = saveGameList.Select((s, i) => (i + 1) + " - " + s).ToList();

            if (saveGameListDisplay.Count == 0) return null;

            Guid gameId;
            while (true)
            {
                Console.WriteLine(string.Join("\n", saveGameListDisplay));
                Console.Write($"Select game to load (1..{saveGameListDisplay.Count}):");
                var userChoiceStr = Console.ReadLine();
                if (int.TryParse(userChoiceStr, out var userChoice))
                {
                    if (userChoice < 1 || userChoice > saveGameListDisplay.Count)
                    {
                        Console.WriteLine("Not in range...");
                        continue;
                    }

                    gameId = saveGameList[userChoice - 1].id;
                    Console.WriteLine($"Loading file: {gameId}");

                    break;
                }

                Console.WriteLine("Parse error...");
            }


            var gameState = gameRepository?.LoadGame(gameId);

            var gameEngine = new UnoGameEngine(gameOptions)
            {
                State = gameState
            };

            var gameController = new GameController(gameEngine, gameRepository);

            gameController.Run();

            return null;
        }
        
        // ================== ABOUT =====================

        string? About()
        {
            //IGameRepository gameRepository;
            string repositoryType = gameRepository?.GetType().ToString() ?? "null"; // Get the runtime type name
            Console.Clear();
            Console.WriteLine("This console application was created by Mehis Kasonen");
            Console.WriteLine("UniID: mekaso");
            Console.WriteLine("Code: 38910164717");
            Console.WriteLine("Press Enter to return to Main Menu...");

            Console.WriteLine(repositoryType);
            //Console.ReadKey(true);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        return "RETURN_TO_MAIN_MENU";
                    }
                }
            }

        }
    }
    
    static IGameRepository CreateGameRepositoryEF(string connectionString)
    {
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        var db = new AppDbContext(contextOptions);
        //db.Database.Migrate();
        return new GameRepositoryEF(db);
    }


    static IGameRepository CreateGameRepositoryFileSystem()
    {
        return new GameRepositoryFileSystem();
    }
    
    static string? ChangeRepository()
    {
        Console.WriteLine("Choose repository type:");
        Console.WriteLine("1. Entity Framework");
        Console.WriteLine("2. File System");
        var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared";
        connectionString = connectionString.Replace("<%temppath%>", Path.GetTempPath());
        
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                gameRepository = CreateGameRepositoryEF(connectionString);
                break;
            case "2":
                gameRepository = CreateGameRepositoryFileSystem();
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }

        return null;
    }
}


/*// ================== SETUP =====================
var gameOptions = new GameOptions();

var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared";
connectionString = connectionString.Replace("<%temppath%>", Path.GetTempPath());

var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;
    
//system will dispose (close down) the db connection when this block of code is over
using var db = new AppDbContext(contextOptions);

var gameRepository = new GameRepositoryEF(db);

//gameRepository = new GameRepositoryEF(db);

var mainMenu = ProgramMenus.GetMainMenu(
    gameOptions,
    ProgramMenus.GetOptionsMenu(gameOptions),
    NewGame,
    LoadGame,
    Configuration,
    About,
    gameRepository
);

// ================== MAIN =====================
while (true)
{
    var result = mainMenu.Run();
    if (result == "RETURN_TO_MAIN_MENU")
    {
        break;
    }
}

mainMenu.Run();

// ================ THE END ==================
return;


// ================== NEW GAME =====================
string? NewGame()
{
    // game logic, shared between console and web
    var gameEngine = new UnoGameEngine(gameOptions);
    
    ProgramMenus.ChoosePlayType(gameEngine);
    
    // set up players
    //PlayerSetup.ConfigurePlayers(gameEngine);

    // set up the table
    gameEngine.ShuffleAndDistributeCards();

    // console controller for game loop
    var gameController = new GameController(gameEngine, gameRepository);
    gameEngine.State.IsInitialised = true;

    gameController.Run();
    return null;
}

// ================== LOAD GAME =====================
string? LoadGame()
{
    Console.WriteLine("Saved games");
    var saveGameList = gameRepository?.GetSavedGameIdentifiers();
    
    if (saveGameList?.Count == 0)
    {
        Console.WriteLine("No saved games found.");
        return null;
    }
    
    var saveGameListDisplay = saveGameList.Select((s, i) => (i + 1) + " - " + s).ToList();

    if (saveGameListDisplay.Count == 0) return null;

    Guid gameId;
    while (true)
    {
        Console.WriteLine(string.Join("\n", saveGameListDisplay));
        Console.Write($"Select game to load (1..{saveGameListDisplay.Count}):");
        var userChoiceStr = Console.ReadLine();
        if (int.TryParse(userChoiceStr, out var userChoice))
        {
            if (userChoice < 1 || userChoice > saveGameListDisplay.Count)
            {
                Console.WriteLine("Not in range...");
                continue;
            }

            gameId = saveGameList[userChoice - 1].id;
            Console.WriteLine($"Loading file: {gameId}");

            break;
        }

        Console.WriteLine("Parse error...");
    }


    var gameState = gameRepository?.LoadGame(gameId);

    var gameEngine = new UnoGameEngine(gameOptions)
    {
        State = gameState
    };
    
    var gameController = new GameController(gameEngine, gameRepository);

    gameController.Run();

    return null;
}

// ================== CONFIGURATION =====================
string? Configuration()
{
    //var gameEngine = new UnoGameEngine(gameOptions);

    string selectedRepository = "null";
    Action<string> onRepositoryChosen = repository => selectedRepository = repository;
    
    //ProgramMenus.ChooseRepositoryType(gameEngine, onRepositoryChosen, gameRepository);
    
    switch (ProgramMenus.ChooseRepositoryType(onRepositoryChosen))
    {
        case "EF":
            gameRepository = ();
            break;
        case "FileSystem":
            gameRepository = new GameRepositoryFileSystem();
            break;
        default:
            // Handle unknown repository type or other cases
            break;
    }
    return null;
}


// ================== ABOUT =====================

string? About()
{
    //IGameRepository gameRepository;
    string repositoryType = gameRepository?.GetType().ToString() ?? "null"; // Get the runtime type name
    Console.Clear();
    Console.WriteLine("This console application was created by Mehis Kasonen");
    Console.WriteLine("UniID: mekaso");
    Console.WriteLine("Code: 38910164717");
    Console.WriteLine("Press Enter to return to Main Menu...");
    
    Console.WriteLine(repositoryType);
    //Console.ReadKey(true);
    
    while (true)
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                return "RETURN_TO_MAIN_MENU";            
            }
        }
    }*/


