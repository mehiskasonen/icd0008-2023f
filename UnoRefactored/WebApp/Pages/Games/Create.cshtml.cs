using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;
using Domain.Database;
using Player = Domain.Database.Player;

namespace WebApp.Pages.Games
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;
        private readonly DAL.IGameRepository _repo;

        public CreateModel(DAL.AppDbContext context, IGameRepository repo)
        {
            _context = context;
            _repo = repo;
        }
        
        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid 
                || _context.Games == null) 
            {
                return Page();
            }
            
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}