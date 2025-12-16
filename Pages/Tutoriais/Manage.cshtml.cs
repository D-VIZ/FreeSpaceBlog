using FreeSpace.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FreeSpace.Models;

namespace FreeSpace.Pages.Posts.Tutoriais
{
    public class ViewModel : PageModel
    {
        private readonly FreeSpace.Data.AppDbContext _context;

        public ViewModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Tutorials> tutoriais { get; set; } = new List<Tutorials>();

        public async Task OnGetAsync()
        {
            tutoriais = await _context.Tutorials.OrderByDescending(t => t.CreatedDate).ToListAsync();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);
            if(tutorial != null)
            {
                _context.Remove(tutorial);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage();
        }
    }
}
