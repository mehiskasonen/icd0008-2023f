namespace Domain;

public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public List<GameCard> DrawDeck { get; set; } = new List<GameCard>();
    public DiscardPile DiscardPile { get; set; } = new DiscardPile();

    public int ActivePlayerNo { get; set; }

    public ETurnResult TurnResult { get; set; } = ETurnResult.NoneYet;

    public List<Player> Players { get; set; } = new List<Player>();
    public PlayerTurn? PreviousTurn { get; set; }

    public bool IsInitialised { get; set; } = false;

    public bool HasDrawnCard { get; set; } = false;
    
    public Dictionary<Guid, Guid> PlayerIdMapping = new Dictionary<Guid, Guid>();

}