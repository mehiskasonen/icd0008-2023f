using System.ComponentModel.DataAnnotations;

namespace Domain.Database;

public class Player : BaseEntity
{
    [MaxLength(128)]
    public string NickName { get; set; } = default!;

    public EPlayerType PlayerType { get; set; }

    // use convenience naming <class>Id
    // nullability decides relationship type - mandatory or not
    public Guid GameId { get; set; }
    public Game? Game { get; set; }
}