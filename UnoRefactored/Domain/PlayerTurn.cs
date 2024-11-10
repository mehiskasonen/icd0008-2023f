namespace Domain;

public class PlayerTurn
{   
    
    /*private GameCard? _card;

    public GameCard? Card
    {
        get => _card ??= new GameCard(EColor.Default, EValue.Default); // Initialize with a new instance if null
        set => _card = value;
    }*/
    public GameCard? Card { get; set; }
    public EColor? DeclaredColor { get; set; }
    public ETurnResult Result { get; set; }
    
}