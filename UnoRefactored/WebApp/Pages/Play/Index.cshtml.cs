using DAL;
using Domain;
using Domain.Database;
//using Domain.Database;
using GameEngine;
using MenuSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Player = Domain.Player;

namespace WebApp.Pages.Play;

[ValidateAntiForgeryToken]
public class Index : PageModel
{
    private readonly DAL.AppDbContext _context;
    private readonly IGameRepository _gameRepository = default!;
    private GameOptions gameOptions = new GameOptions();
    public UnoGameEngine Engine { get; set; } = default!;
    public GameState State { get; set; } = default!;
    Dictionary<Guid, Guid>playerIdMapping = new Dictionary<Guid, Guid>();
    public Player? ClickedPlayer { get; private set; }
    public bool HasDrawnCard { get; set; }
    
    public Index(AppDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEF(_context);
    }

    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid PlayerId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid CardId { get; set; }

    [BindProperty(SupportsGet = true)] public PlayerTurn Turn { get; set; } = new PlayerTurn();
    [BindProperty(SupportsGet = true)] public Guid ClickedPlayerId { get; set; }
    [BindProperty(SupportsGet = true)] public string? GameMode { get; set; }

    
    public IActionResult OnGet()
    {
        var gameState = _gameRepository.LoadGame(GameId);

        if (gameState == null)
        {
            gameState = new GameState
            {
                Id = GameId,
                IsInitialised = false,
            };
        }
        
        var waitingPlayers = _context.Players
            .Where(p => p.GameId == GameId && p.NickName.StartsWith("Waiting for Player nr"))
            .ToList();

        if (waitingPlayers.Any())
        {
            ViewData["WaitingForPlayers"] = true;
        }
        
        if (!gameState.IsInitialised)
            {
                Engine = new UnoGameEngine(gameOptions)
                {
                    State = gameState
                };

                foreach (var contextPlayer in _context.Players)
                {
                    if (contextPlayer.GameId == GameId)
                    {
                        _context.Entry(contextPlayer).State = EntityState.Detached;
                        var domainPlayer = new Domain.Player
                        {
                            Id = contextPlayer.Id,
                            NickName = contextPlayer.NickName,
                            PlayerType = contextPlayer.PlayerType,
                        };
                        Console.WriteLine(domainPlayer.Id);
                        gameState.Players.Add(domainPlayer);
                        playerIdMapping[contextPlayer.Id] = domainPlayer.Id;
                    }
                }
                gameState.PlayerIdMapping = playerIdMapping;
                Engine.DeckBuilder();
                Engine.ShuffleAndDistributeCards();
                HttpContext.Session.SetString("NotInitialised", "true");
                gameState.IsInitialised = true;
                gameState.ActivePlayerNo = 0;
                
                
                //var PlayerId = Engine.GetActivePlayer().Id;
                
                _gameRepository.SaveGame(GameId, gameState);
                
                
                //return RedirectToPage("/Play/Index", new { gameId = GameId, playerId = PlayerId });

            }
            else
            {
                Engine = new UnoGameEngine(gameOptions)
                {
                    State = gameState
                };
                Engine.State.IsInitialised = true;
                //_gameRepository.SaveGame(GameId, gameState);
            }
        
        //if AI turn take AI action
        //If human turn take human action
        
        Player activePlayer;
        activePlayer = Engine.GetActivePlayer();
        while (IsAiPlayerActive())
        {
            PlayerTurn currentTurn = new PlayerTurn()
            {
                Result = ETurnResult.GameStart,
                Card = Engine.State.DiscardPile.DiscardedCards.Last(),
                DeclaredColor = Engine.State.DiscardPile.DiscardedCards.Last()!.CardColor
            };
            
            currentTurn = Engine.PlayTurn(currentTurn, Engine.State.DrawDeck);
            if (currentTurn.Card != null)
            {
                Engine.State.DiscardPile.DiscardedCards.Add(currentTurn.Card);
                Engine.State.Players[Engine.State.ActivePlayerNo].PlayerHand.Remove(currentTurn.Card);
            }
            
            _gameRepository.SaveGame(GameId, gameState);
            Engine.NextPlayerTurn();
        }
        
        ClickedPlayerId = gameState.Players.FirstOrDefault(p => p.Id == PlayerId)?.Id ?? Engine.GetActivePlayer().Id;
        ClickedPlayer = gameState.Players.FirstOrDefault(p => p.Id == ClickedPlayerId && p.PlayerType == EPlayerType.Human) ?? Engine.GetActivePlayer();
        
        //ClickedPlayer = gameState.Players.FirstOrDefault(p => p.Id == PlayerId) ?? Engine.GetActivePlayer(); 
        _gameRepository.SaveGame(GameId, gameState);
        return Page();
    }
    
    public IActionResult OnPostDrawCard()
    {
            var gameState = _gameRepository.LoadGame(GameId);
            Engine = new UnoGameEngine(gameOptions)
            {
                State = gameState
            };
            GameCard Card;

            if (!gameState.HasDrawnCard)
            {
                Card = Engine.DrawCard(gameState)!;
                Engine.GetActivePlayer().PlayerHand.Add(Card);
                gameState.HasDrawnCard = true;
                _gameRepository.SaveGame(GameId, gameState);
                return RedirectToPage("/Play/Index", new { gameId = GameId, playerId = Engine.GetActivePlayer().Id });
            }
            else
            {
                TempData["ErrorMessage"] = "You have already drawn a card this turn.";
                return RedirectToPage("/Play/Index", new { gameId = GameId, playerId = Engine.GetActivePlayer().Id });
            }
            
    }

    public IActionResult OnPostEndTurn()
    {
        var gameState = _gameRepository.LoadGame(GameId);
        Engine = new UnoGameEngine(gameOptions)
        {
            State = gameState
        };
        bool hasPlayableCard =
            Engine.GetCardsPlayerCanPlay(gameState.Players[gameState.ActivePlayerNo]).Count > 0;
        if (!hasPlayableCard)
        {
            gameState.HasDrawnCard = false;
            Engine.SkipPlayerMove();
            Engine.NextPlayerTurn();
            _gameRepository.SaveGame(GameId, gameState);
            return RedirectToPage("/Play/Index", new { gameId = GameId });
        }
        TempData["ErrorMessage"] = "You have playable cards!";
        return Page();
    }


    public IActionResult OnPostPlayCard()
    {
        try
        {
            var gameId = Guid.Parse(Request.Form["GameId"]);
            var cardIndex = int.Parse(Request.Form["CardIndex"]);
            
            Console.WriteLine(cardIndex);
            var gameState = _gameRepository.LoadGame(GameId);
            Engine = new UnoGameEngine(gameOptions)
            {
                State = gameState
            };
            
            var currentPlayer = Engine.GetActivePlayer();
            //var currentPlayer = gameState.Players.FirstOrDefault(p => p.Id == PlayerId);
            if (currentPlayer == null || currentPlayer != Engine.GetActivePlayer())
            {
                TempData["ErrorMessage"] = $"It is not your turn. Wait for your turn! Current player name is {currentPlayer?.NickName}";
                return RedirectToPage("/Play/Index", new { gameId = GameId, playerId = currentPlayer!.Id });
            }
            
            if (!Engine.ValidatePlayerMove(cardIndex + 1))
            {
                TempData["ErrorMessage"] = "Invalid move. Please select a valid card to play.";
                return RedirectToPage("/Play/Index", new { gameId = GameId, playerId = Engine.GetActivePlayer().Id });
            }
            
            Engine.MakePlayerMove(cardIndex + 1);
            gameState.HasDrawnCard = false;
            Engine.NextPlayerTurn();

            if (!IsAiPlayerActive())
            {
                ClickedPlayer = currentPlayer;
            }
            
            
            _gameRepository.SaveGame(gameId, gameState);
            TempData["SuccessMessage"] = "Card played, wait for your next turn.";
            return RedirectToPage("/Play/Index", new { gameId = GameId, playerId = Engine.GetActivePlayer().Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in OnPostPlayCard: {ex.Message}");
            return new JsonResult(new { success = false, message = $"Error playing the card. Exception details: {ex.Message}" });
        }
    }
    
    private bool IsAiPlayerActive()
    {
        var activePlayerNo = Engine.State.ActivePlayerNo;

        // Check if activePlayerNo is a valid index
        if (activePlayerNo >= 0 && activePlayerNo < Engine.State.Players.Count)
        {
            var activePlayer = Engine.State.Players[activePlayerNo];

            // Check if activePlayer is not null and has PlayerType property
            if (activePlayer != null && (activePlayer.PlayerType == EPlayerType.AI || activePlayer.PlayerType == EPlayerType.Human))
            {
                return activePlayer.PlayerType == EPlayerType.AI;
            }
        }
        // Return false if any check fails
        return false;
    }
    
}
