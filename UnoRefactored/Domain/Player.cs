namespace Domain;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string NickName { get; set; } = default!;

    public EPlayerType PlayerType { get; set; }

    public List<GameCard> PlayerHand { get; set; } = new List<GameCard>();
}