using Domain;
using GameEngine;
using MenuSystem;

namespace ConsoleApp;

public static class PlayerSetup
{
    
    internal static string? SetPlayerCount(UnoGameEngine gameEngine, EPlayerType playerType1, EPlayerType playerType2)
{
    int playerCount = 0;

    while (true)
    {
        Console.Clear();
        Console.Write($"Set {playerType1} and {playerType2} Players (1 - {gameEngine.GetMaxAmountOfPlayers() - playerCount})[{playerCount}]:");
        var input = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            break;
        }

        if (int.TryParse(input, out int count) && count >= 1 && count <= gameEngine.GetMaxAmountOfPlayers() - playerCount)
        {
            playerCount = count;
            break;
        }

        Console.WriteLine("Invalid input. Please enter a number within the allowed range.");
    }

    if (playerType1 == EPlayerType.Human && playerType2 == EPlayerType.Human)
    {
        // Human vs Human: Ask for names for each human player
        for (int i = 0; i < playerCount; i++)
        {
            string? playerName = "";
            while (true)
            {
                Console.Write($"Player {i + 1} name (min 1 letter):");
                playerName = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
                Console.WriteLine("Invalid input. Please enter a valid name.");
            }

            gameEngine.State.Players.Add(new Player()
            {
                NickName = playerName,
                PlayerType = EPlayerType.Human
            });
        }
    }
    else if ((playerType1 == EPlayerType.Human && playerType2 == EPlayerType.AI) || (playerType1 == EPlayerType.AI && playerType2 == EPlayerType.Human))
    {
        int humanPlayerCount = 0;
        
        // Human vs AI or AI vs Human: Ask for names for each human player and AI players
        while (true)
        {
            Console.Write($"How many Human players? (0 - {playerCount - 1})[{humanPlayerCount}]:");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                break;
            }

            if (int.TryParse(input, out int count) && count >= 0 && count <= playerCount - 1)
            {
                humanPlayerCount = count;
                break;
            }

            Console.WriteLine("Invalid input. Please enter a number within the allowed range.");
        }

        int aiPlayerCount = playerCount - humanPlayerCount;

        for (int i = 0; i < humanPlayerCount; i++)
        {
            string? playerName = "";
            while (true)
            {
                Console.Write($"Human Player {i + 1} name (min 1 letter):");
                playerName = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
                Console.WriteLine("Invalid input. Please enter a valid name.");
            }

            gameEngine.State.Players.Add(new Player()
            {
                NickName = playerName,
                PlayerType = EPlayerType.Human
            });
        }

        for (int i = 0; i < aiPlayerCount; i++)
        {
            string? playerName = "";
            while (true)
            {
                Console.Write($"AI Player {i + 1} name (min 1 letter):");
                playerName = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
                Console.WriteLine("Invalid input. Please enter a valid name.");
            }

            gameEngine.State.Players.Add(new Player()
            {
                NickName = playerName,
                PlayerType = EPlayerType.AI
            });
        }
    }
    else if (playerType1 == EPlayerType.AI && playerType2 == EPlayerType.AI)
    {
        // AI vs AI: Ask for names for each AI player
        for (int i = 0; i < playerCount; i++)
        {
            string? playerName = "";
            while (true)
            {
                Console.Write($"AI Player {i + 1} name (min 1 letter):");
                playerName = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
                Console.WriteLine("Invalid input. Please enter a valid name.");
            }

            gameEngine.State.Players.Add(new Player()
            {
                NickName = playerName,
                PlayerType = EPlayerType.AI
            });
        }
    }

    return null;
}


    /*internal static string? SetPlayerCount(UnoGameEngine gameEngine, EPlayerType playerType1, EPlayerType playerType2)
    {
        int playerCount = 0;

        while (true)
        {
            Console.Clear();
            Console.Write($"Set {playerType1} and {playerType2} Players (1 - {gameEngine.GetMaxAmountOfPlayers() - playerCount})[{playerCount}]:");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                break;
            }

            if (int.TryParse(input, out int count) && count >= 1 && count <= gameEngine.GetMaxAmountOfPlayers() - playerCount)
            {
                playerCount = count;
                break;
            }

            Console.WriteLine("Invalid input. Please enter a number within the allowed range.");
        }

        for (int i = 0; i < playerCount; i++)
        {
            string? playerName = "";
            while (true)
            {
                Console.Write($"Player {i + 1} name (min 1 letter)[{GetPlayerTypeString(playerType1, i + 1)}]:");
                playerName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    playerName = GetPlayerTypeString(playerType1, i + 1);
                }

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
                Console.WriteLine("Invalid input. Please enter a valid name.");
            }
            
            EPlayerType currentPlayerType;

            if (playerType1 == EPlayerType.Human && playerType2 == EPlayerType.AI)
            {
                // Human vs AI: Assign AI type for odd-indexed players
                currentPlayerType = (i % 2 == 0) ? playerType1 : EPlayerType.AI;
            }
            else if (playerType1 == EPlayerType.AI && playerType2 == EPlayerType.Human)
            {
                // AI vs Human: Assign AI type for even-indexed players
                currentPlayerType = (i % 2 == 0) ? EPlayerType.AI : playerType1;
            }
            else
            {
                // AI vs AI or Human vs Human: Alternate player types
                currentPlayerType = (i % 2 == 0) ? playerType1 : playerType2;
            }
            
            gameEngine.State.Players.Add(new Player()
            {
                NickName = playerName,
                PlayerType = currentPlayerType
                //PlayerType = (i % 2 == 0) ? playerType1 : playerType2
            });
        }

        return null;
    }*/

    private static string GetPlayerTypeString(EPlayerType playerType, int index)
    {
        return $"{playerType.ToString().ToLower()}{index}";
    }


    /*internal static string? SetPlayerCount(UnoGameEngine gameEngine, EPlayerType playerType1, EPlayerType playerType2)
    {
        int playerCount = 0;

        while (true)
        {
            Console.Clear();
            Console.Write($"Set {playerType} Players (1 - {gameEngine.GetMaxAmountOfPlayers() - playerCount})[{playerCount}]:");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                break;
            }

            if (int.TryParse(input, out int count) && count >= 1 && count <= gameEngine.GetMaxAmountOfPlayers() - playerCount)
            {
                playerCount = count;
                break;
            }

            Console.WriteLine("Invalid input. Please enter a number within the allowed range.");
        }

        for (int i = 0; i < playerCount; i++)
        {
            string? playerName = "";
            while (true)
            {
                Console.Write($"Player {i + 1} name (min 1 letter)[{playerType.ToString().ToLower()}{i + 1}]:");
                playerName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    playerName = $"{playerType.ToString().ToLower()}{i + 1}";
                }

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
                Console.WriteLine("Invalid input. Please enter a valid name.");
            }

            gameEngine.State.Players.Add(new Player()
            {
                NickName = playerName,
                PlayerType = playerType
            });
        }

        return null;
    }*/
    
    
    
    
    /*public static void ConfigurePlayers(UnoGameEngine gameEngine)
    {
        var playerCount = 0;

        while (true)
        {
            Console.Write($"How many players (2 - {gameEngine.GetMaxAmountOfPlayers()})[2]:");
            var playerCountStr = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(playerCountStr)) playerCountStr = "2";
            if (int.TryParse(playerCountStr, out playerCount))
            {
                if (playerCount > 1 && playerCount <= gameEngine.GetMaxAmountOfPlayers()) break;
                Console.WriteLine("Number not in range...");
            }
        }


        for (int i = 0; i < playerCount; i++)
        {
            string? playerType = "";
            while (true)
            {
                Console.Write($"Player {i + 1} type (A - ai / H - human)[{(i % 2 == 0 ? "h" : "a")}]:");
                playerType = Console.ReadLine()?.ToLower().Trim();
                if (string.IsNullOrWhiteSpace(playerType))
                {
                    playerType = i % 2 == 0 ? "h" : "a";
                }

                if (playerType == "a" || playerType == "h") break;
                Console.WriteLine("Parse error...");
            }

            string? playerName = "";
            while (true)
            {
                Console.Write($"Player {i + 1} name (min 1 letter)[{playerType + (i + 1)}]:");
                playerName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    playerName = playerType + (i + 1);
                }

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
                Console.WriteLine("Parse error...");
            }

            gameEngine.State.Players.Add(new Player()
            {
                NickName = playerName,
                PlayerType = playerType == "h" ? EPlayerType.Human : EPlayerType.AI
            });
        }
    }*/
}