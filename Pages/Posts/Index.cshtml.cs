using FreeSpace.Data;
using FreeSpace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FreeSpace.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(
            AppDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public string CommentText { get; set; } = default!;

        public Comment Comment { get; set; } = default!;

        public Like Like { get; set; } = default!;

        public Post? post { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return RedirectToPage("./Index");
            }

            await RefreshInfo(id);

            if (post == null)
            {
                return NotFound();
            }


            return Page();

        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLike(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = _userManager.GetUserId(User);

            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == id && l.UserId == userId);

            if (like != null)
            {
                _context.Likes.Remove(like);
            }
            else
            {
                like = new Like
                {
                    PostId = id,
                    UserId = userId
                };
                _context.Likes.Add(like);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int id)
        {
            ModelState.Clear();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (string.IsNullOrWhiteSpace(CommentText))
            {
                ModelState.AddModelError(nameof(CommentText), "Comentário não pode ficar vazio.");
                return Page();
            }

            if (CommentText.Length > 300)
            {
                ModelState.AddModelError(nameof(CommentText), "O comentário não pode ter mais de 300 caracteres.");
                await RefreshInfo(id);
                return Page();
            }

            var post = await _context.Posts.FindAsync(id);

            Comment = new Comment
            {
                Text = CommentText,
                CreatedDate = DateTime.Now,
                UserId = _userManager.GetUserId(User),
                PostId = id
            };

            _context.Comments.Add(Comment);
            await _context.SaveChangesAsync();

            CommentText = string.Empty;

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostFollowPost(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = _userManager.GetUserId(User);

            var post = new Post
            {
                Id = id
            };

            if (post == null)
            {
                return Page();
            }

            await RefreshInfo(id);

            var follow = _context.FollowedPosts.FirstOrDefault(p => p.PostId == id && p.UserId == userId);

            if (follow == null)
            {
                follow = new FollowedPost
                {
                    PostId = id,
                    UserId = _userManager.GetUserId(User),
                    LastRead = DateTime.Now
                };
                _context.FollowedPosts.Add(follow);
            }
            else
            {
                _context.FollowedPosts.Remove(follow);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task RefreshInfo(int? id)
        {
            post = await _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                    .Include(p => p.Likes)
                        .ThenInclude(l => l.User)
                    .Include(p => p.Followers)
                        .ThenInclude(f => f.User)
                    .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}

