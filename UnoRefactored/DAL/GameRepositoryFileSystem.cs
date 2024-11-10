using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    static List<string> gamesToLoad = new List<string>();
    private readonly string _filePrefix = "./SavedGames/"; // + Path.PathSeparator;
    
    public GameRepositoryFileSystem()
    {
        InitializeDirectory();
    }
    
    private void InitializeDirectory()
    {
        if (!Directory.Exists(_filePrefix))
        {
            Directory.CreateDirectory(_filePrefix);
        }
    }

    public void SaveGame(Guid id, GameState state)
    {
        InitializeDirectory();
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
        
        File.WriteAllText(Path.Combine(_filePrefix, fileName), content);
        gamesToLoad.Add(fileName);
    }
    
    public GameState LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
        var jsonStr = File.ReadAllText(Path.Combine(_filePrefix, fileName));
        var res = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");
        return res;
    }

    public List<(Guid id, DateTime dt)> GetSavedGameIdentifiers()
    {
        try
        {
            var identifiers = Directory.EnumerateFiles(_filePrefix);
            var result = new List<(Guid id, DateTime dt)>();
            foreach (var filePath in identifiers)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                if (Guid.TryParse(fileName, out Guid id))
                {
                    var lastWriteTime = File.GetLastWriteTime(filePath);
                    result.Add((id, lastWriteTime));
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while retrieving saved game identifiers: {ex.Message}");
            return new List<(Guid id, DateTime dt)>();
        }
    }

}