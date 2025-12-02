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
using System.IO;

namespace FreeSpace.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly FreeSpace.Data.AppDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [BindProperty]
        public Post Post { get; set; } = default!;

        public IList<Post> Posts { get; set; } = new List<Post>();

        public async Task OnGetAsync()
        {
            Posts = await _context.Posts.Include(p => p.User).OrderByDescending(p => p.CreatedDate).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            Post.UserId = _userManager.GetUserId(User);
            Post.CreatedDate = DateTime.Now;

            if (!ModelState.IsValid || _context.Posts == null)
            {
                Posts = await _context.Posts.Include(p => p.User).OrderByDescending(p => p.CreatedDate).ToListAsync();
                return Page();
            }
            if(Post.Media != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Post.Media.FileName);
                var filePath = Path.Combine("wwwroot/uploads/" + fileName);

                using var stream = System.IO.File.Create(filePath);
                await Post.Media.CopyToAsync(stream);

                Post.MediaPath = "/uploads/" + fileName;
            }
            if(Post.UserId == null)
            {
                Post.Title = "UserId nao associado";
            }

            _context.Posts.Add(Post);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return RedirectToPage();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLike(int id)
        {
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                post.Likes++;
                await _context.SaveChangesAsync();
                return RedirectToPage();
            }

            return Page();
        }
    }
}
