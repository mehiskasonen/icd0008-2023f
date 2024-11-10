namespace Domain;

public class DiscardPile
{
    public List<GameCard?> DiscardedCards { get; set; } = new List<GameCard?>();

    public void ResetDiscardPile()
    {
        DiscardedCards = new List<GameCard?>();
    }
    
    public void AddCardToDiscardPile(GameCard card)
    {
        DiscardedCards.Add(card);
    }
    
    public GameCard? GetLastCardInDiscardPile()
    {
        var lastCardInDiscardPile = DiscardedCards.LastOrDefault();
        return lastCardInDiscardPile;
    }
    
}