using Domain;

namespace ConsoleUI;

public static class ConsoleVisualization
{
    public static void DrawDesk(GameState state)
    {
        Console.WriteLine($"Discard pile has {state.DiscardPile.DiscardedCards.Count} cards");
        Console.WriteLine($"Cards in deck: {state.DrawDeck.Count}");

        for (var i = 0; i < state.Players.Count; i++)
        {
            Console.WriteLine(
                $"Player {i + 1} - {state.Players[i].NickName} has {state.Players[i].PlayerHand.Count} cards");
        }
        
        Console.WriteLine($"Top card in discard pile: " +
                          string.Join(" ", state.DiscardPile.DiscardedCards.Last()));

        
    }

    public static void DrawPlayerHand(Player player)
    {
        Console.WriteLine("Your current hand is: " +
                          string.Join(
                              "  ",
                              player.PlayerHand.Select((c, i) => (i+1) + ": " + c)
                          )
        );
    }
}