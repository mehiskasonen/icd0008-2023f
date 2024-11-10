/*namespace Domain;

public class DeckBuilder
{
    private const int NrOfCards = 108;
    //public static List<Card> UnoDeck { get; set; } = new List<Card>();

    public List<GameCard> BuildUnoDeck()
    {
        GameState.DrawDeck = new List<GameCard>();
        //UnoDeck.Clear();
        foreach (EColor color in Enum.GetValues(typeof(EColor)))
        {
            if (color != EColor.Wild)
            {
                for (int i = 0; i <= 9; i++)
                {
                    if (i == 0)
                        GameState.DrawDeck.Add(new GameCard(color, EValue.Zero));
                    else
                    {
                        GameState.DrawDeck.Add(new GameCard(color, (EValue)i));
                        GameState.DrawDeck.Add(new GameCard(color, (EValue)i));
                    }
                }
                /*UnoDeck.Add(new Card(color, EValue.None, Card.EEffect.DrawTwo));
                UnoDeck.Add(new Card(color, EValue.None, Card.EEffect.DrawTwo));#1#

                GameState.DrawDeck.Add(new GameCard(color, EValue.Skip));
                GameState.DrawDeck.Add(new GameCard(color, EValue.Skip));
                GameState.DrawDeck.Add(new GameCard(color, EValue.Reverse));
                GameState.DrawDeck.Add(new GameCard(color, EValue.Reverse));
                GameState.DrawDeck.Add(new GameCard(color, EValue.DrawTwo));
                GameState.DrawDeck.Add(new GameCard(color, EValue.DrawTwo));

            }
        }

        for (int i = 0; i < 4; i++)
        {
            /*UnoDeck.Add(new Card.WildCard(EColor.Wild, Card.EType.Wild));
            UnoDeck.Add(new Card.WildCard(EColor.Wild, Card.EType.Wild, Card.EEffect.DrawFour));#1#

            GameState.DrawDeck.Add(new GameCard(EColor.Wild, EValue.Wild));
            GameState.DrawDeck.Add(new GameCard(EColor.Wild, EValue.DrawFour));
        }

        return GameState.DrawDeck;
    }
    
    public DeckBuilder()
    {
        GameState.DrawDeck = new List<GameCard>();
        BuildUnoDeck();
    }
    
    public void ShuffleCards(List<GameCard> deck)
    {
        Random rand = new Random();
        int n = deck.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            GameCard temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
        
    }
    
    public List<GameCard> Draw(int count)
    {
        var drawnCards = GameState.DrawDeck.Take(count).ToList();
        GameState.DrawDeck.RemoveAll(x => drawnCards.Contains(x));

        return drawnCards;
    }

    
    public static GameCard DrawCard(List<GameCard> deck)
    {
        GameCard drawnCard = deck[0];
        deck.RemoveAt(0);
        return drawnCard;
    }
}*/