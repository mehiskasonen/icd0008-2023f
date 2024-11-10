using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Domain.Database;
using GameEngine;
using Microsoft.VisualStudio.TextTemplating;

namespace WebApp.Pages.Games
{
    public class CreateNicknameModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateNicknameModel(AppDbContext context)
        {
            _context = context;
            _gameRepository = new GameRepositoryEF(_context);

        }

        [BindProperty(SupportsGet = true)]
        public Guid PlayerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid GameId { get; set; }

        [BindProperty]
        public string Nickname { get; set; } = null!;
        public UnoGameEngine Engine { get; set; } = default!;
        private readonly IGameRepository _gameRepository = default!;
        
        public IActionResult OnPost()
        {
            Console.WriteLine($"PlayerId: {PlayerId}, GameId: {GameId}");

            var player = _context.Players.FirstOrDefault(p => p.Id == PlayerId);
            if (player != null)
            {
                player.NickName = Nickname;
                _context.Players.Update(player);
                foreach (var pla in _context.Players)
                {
                    Console.WriteLine(pla.NickName);
                }
                var entries = _context.ChangeTracker.Entries();
                foreach (var entry in entries)
                {
                    Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
                }

                _context.SaveChanges();
            }
            
            var gameState = _gameRepository.LoadGame(GameId);
            var playerInGameState = gameState.Players.FirstOrDefault(p => p.Id == PlayerId);
            if (playerInGameState != null)
            {
                playerInGameState.NickName = Nickname;
            }
            _gameRepository.SaveGame(GameId, gameState);

            return RedirectToPage("/Play/Index", new { gameId = GameId, playerId = PlayerId });
        }
    }
}
