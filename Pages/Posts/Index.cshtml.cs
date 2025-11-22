using FreeSpace;
using FreeSpace.Data;
using FreeSpace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSpace.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly FreeSpace.Data.AppDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(AppDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }



        [BindProperty]
        public Post Post { get; set; } = default!;

        public IList<Post> Posts { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Posts = await _context.Posts.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (!ModelState.IsValid || _context.Posts == null)
            {
                return Page();
            }
            Post.CreatedDate = DateTime.Now;
            _context.Posts.Add(Post);
            await _context.SaveChangesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Posts/Index");
        }
    }
}
