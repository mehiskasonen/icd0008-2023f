namespace Domain;

public class GameOptions
{
    // max hand size during gameplay
    public int DeckSize { get; set; } = 108;
    
    // discard 2-3-4-5 from initial deck
    public bool UniqueWildCards { get; set; } = false;

    public override string ToString() => $"deck size: {DeckSize}, new wild cards in deck: {(UniqueWildCards ? "yes": "no")}";
}