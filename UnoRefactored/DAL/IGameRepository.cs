using Domain;

namespace DAL;

public interface IGameRepository
{
    void SaveGame(Guid id, GameState state);
    List<(Guid id, DateTime dt)> GetSavedGameIdentifiers();

    GameState LoadGame(Guid id);
}