using Domain;

namespace ConsoleApp;

public static class OptionsChanger
{
    public static string? ConfigureHandSize(GameOptions gameOptions)
    {
        while (true)
        {
            Console.Write($"Enter hand size (2-{GetMaxHandSize(gameOptions)}):");
            var sizeStr = Console.ReadLine();

            if (sizeStr == null) continue;

            if (!int.TryParse(sizeStr, out var size))
            {
                Console.WriteLine("Parse error...");
                continue;
            }

            if (size < 2 || size > GetMaxHandSize(gameOptions))
            {
                Console.WriteLine("Out of range...");
                continue;
            }


            gameOptions.DeckSize = size;
            return null;
        }
    }

    // number of cards in one suite
    private static int GetMaxHandSize(GameOptions gameOptions) => gameOptions.UniqueWildCards ? 9 : 13;

    public static string? ConfigureSmallCards(GameOptions gameOptions)
    {
        while (true)
        {
            Console.Write("Use small cards (2-5) in deck (Y/N):");
            var choice = Console.ReadLine()?.ToLower().Trim();
            if (choice == "y" || choice == "n")
            {
                gameOptions.UniqueWildCards = choice == "y";
                return null;
            }

            Console.WriteLine("Parse error...");
        }
    }
}