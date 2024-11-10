using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;

namespace DAL;

public class GameRepositoryEF : IGameRepository
{
    private readonly AppDbContext _ctx;

    public GameRepositoryEF(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public void SaveGame(Guid id, GameState state)
    {
        // Check if a GameState with the given Id already exists in the database
        var existingGameState = _ctx.Games.FirstOrDefault(g => g.Id == id);
        if (existingGameState == null)
        {
            // No existing GameState, create a new one
            var newGameState = new Game()
            {
                Id = id,
                UpdatedAtDt = DateTime.Now,
                State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions),
                Players = state.Players.Select(p => new Domain.Database.Player()
                {
                    Id = p.Id,
                    NickName = p.NickName,
                    PlayerType = p.PlayerType
                }).ToList()
            };
            _ctx.Games.Add(newGameState);
        }
        else
        {
            // Update the existing GameState
            existingGameState.UpdatedAtDt = DateTime.Now;
            existingGameState.State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
        }
        var changeCount = _ctx.SaveChanges();
        Console.WriteLine("SaveChanges: " + changeCount);
    }


    public List<(Guid id, DateTime dt)> GetSavedGameIdentifiers()
    {
        return _ctx.Games
            .OrderByDescending(g => g.UpdatedAtDt)
            .ToList()
            .Select(g => (g.Id, g.UpdatedAtDt))
            .ToList();
    }

    public GameState LoadGame(Guid id)
    {
        var game = _ctx.Games.First(g => g.Id == id);
        return JsonSerializer.Deserialize<GameState>(game.State, JsonHelpers.JsonSerializerOptions)!;
    }
}