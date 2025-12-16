using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FreeSpace.Data;
using FreeSpace.Models;
using Microsoft.AspNetCore.Identity;

namespace FreeSpace.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Post Post { get; set; } = default!;

        public IList<Post> Posts { get; set; } = new List<Post>();

        public ApplicationUser user { get; set; }


        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            user = await _userManager.FindByIdAsync(id);

            Posts = await _context.Posts.Include(p => p.User).OrderByDescending(p => p.CreatedDate).ToListAsync();

            var posts = _context.Posts.Include(p => p.User).Where(p => p.UserId == id).OrderByDescending(p => p.CreatedDate);

            if (id == null)
            {
                return NotFound();
            }

            ViewData["Title"] = user?.UserName;
            Posts = await posts.ToListAsync();

            return Page();
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
            if (Post.Media != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Post.Media.FileName);
                var filePath = Path.Combine("wwwroot/uploads/" + fileName);

                using var stream = System.IO.File.Create(filePath);
                await Post.Media.CopyToAsync(stream);

                Post.MediaPath = "/uploads/" + fileName;
            }

            _context.Posts.Add(Post);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {

            var post = await _context.Posts.FindAsync(id);
            var userId = post?.UserId;

            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return RedirectToPage(new {id = userId});
            }

            return Page();
        }
    }
}
