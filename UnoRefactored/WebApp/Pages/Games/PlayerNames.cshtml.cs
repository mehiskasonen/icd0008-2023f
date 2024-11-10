using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using DAL;
using Domain;
using Domain.Database;
using GameEngine;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Player = Domain.Database.Player;


namespace WebApp.Pages.Games;


public class PlayerNamesModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IGameRepository _gameRepository = default!;
    public UnoGameEngine Engine { get; set; } = default!;
    private GameOptions gameOptions = new GameOptions();


    public PlayerNamesModel(DAL.AppDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEF(_context);
    }

    [BindProperty] public Game Game { get; set; } = default!;
    [BindProperty] public List<string>? HumanPlayerNames { get; set; }
    [BindProperty] public List<string>? AiPlayerNames { get; set; }
    [BindProperty] public List<string>? PlayerNames { get; set; }

    
    public SelectList PlayersList { get; set; } = default!;
    public string CreatedAtDt { get; set; } = default!;
    public string UpdatedAtDt { get; set; } = default!;
    public string State { get; set; } = default!;
    public int NumPlayers { get; set; }
    public string GameType { get; set; } = default!;

    public string? GameModee { get; set; }
    public int HumanNum { get; set; }
    public int AiNum { get; set; }
    
    public void OnGet(string data)
    {
        // Decode the form data from the URL
        var decodedData = Uri.UnescapeDataString(data);

        // Deserialize the JSON data
        var formData = JsonConvert.DeserializeObject<FormData>(decodedData);

        // Set the properties for display on the Summary page
        State = formData!.State ?? "string.Empty";
        NumPlayers = formData.NumPlayers;
        GameType = formData.GameType ?? string.Empty;
        GameModee = formData.GameMode;
        HumanNum = formData.HumanNum;
        AiNum = formData.AiNum;
    }

    public void GenerateAiPlayers(int? aiPlayerCount)
    {
        if (aiPlayerCount != 0)
        {
            for (int i = 1; i <= aiPlayerCount; i++)
            {
                string aiPlayerName = $"AI Player {i}";
                AiPlayerNames?.Add(aiPlayerName);
                var aiPlayer = new Player
                {
                    NickName = aiPlayerName,
                    PlayerType = EPlayerType.AI,
                    GameId = Game.Id
                };
                _context.Players.Add(aiPlayer);
            }
        }
    }
    
    
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            
            foreach (var error in errors)
            {
                Console.WriteLine($"Model error: {error.ErrorMessage}");
            }
            return Page();
        }
        _context.Games.Add(Game);
        _context.SaveChanges();
        
        if (HumanPlayerNames != null)
            foreach (var playerName in HumanPlayerNames)
            {
                var humanPlayer = new Player
                {
                    NickName = playerName,
                    PlayerType = EPlayerType.Human,
                    GameId = Game.Id
                };
                _context.Players.Add(humanPlayer);
                _context.SaveChanges();
            }

        
        if (PlayerNames != null)
            for (int i = 0; i < PlayerNames?.Count; i++)
            {
                if (i == 0)
                {
                    var firstHumanPlayer = new Player
                    {
                        NickName = PlayerNames[i],
                        PlayerType = EPlayerType.Human,
                        GameId = Game.Id
                    };
                    _context.Players.Add(firstHumanPlayer);
                }
                string playerName = $"Waiting for Player nr {i + 2}";
                var humanPlayer = new Player
                {
                    NickName = playerName,
                    PlayerType = EPlayerType.Human,
                    GameId = Game.Id
                };
                _context.Players.Add(humanPlayer);
                _context.SaveChanges();
            }
        
        Console.WriteLine("Human player names count, Ai player names count, AiNum number");
        Console.WriteLine(HumanPlayerNames?.Count);
        Console.WriteLine(AiPlayerNames?.Count);
        Console.WriteLine(AiNum);
        GenerateAiPlayers(AiPlayerNames?.Count);
        _context.SaveChanges();
        
        return RedirectToPage("/Play/Index", new { gameId = Game.Id, playerId = Game.Players!.FirstOrDefault()!.Id, GameMode = GameModee });
    }
    

    public class FormData
    {
        public string? CreatedAtDt { get; set; }
        public string? UpdatedAtDt { get; set; }
        public string? State { get; set; }
        public int NumPlayers { get; set; }
        public string? GameType { get; set; }
        public string? GameMode { get; set; }
        public int HumanNum { get; set; }
        public int AiNum { get; set; }
    }
}