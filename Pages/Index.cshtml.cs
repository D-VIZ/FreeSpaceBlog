using FreeSpace.Data;
using FreeSpace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSpace.Pages
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
        public Post Post { get; set; } = default!;

        [BindProperty]
        public string CommentText { get; set; } = default!;

        public IList<Post> Posts { get; set; } = new List<Post>();

        public IList<Post> FPosts { get; set; } = new List<Post>();

        public IList<Post> LPosts { get; set; } = new List<Post>();

        public Comment Comment { get; set; } = default!;

        public Like Like { get; set; } = default!;

        public FollowedPost FollowedPost { get; set; } = default!;

        public int TopCount { get; set; } = 10;

        public SelectList Tags { get; set; } = new SelectList(new List<string>
        {
            "Geral", "Erro", "Dúvida", "Projeto Final", "Notícia", "Discussão", "Estudo", "Tutorial"
        });

        public SelectList Plataformas { get; set; } = new SelectList(new List<string>
        {
            "Não especificado",
            "Blender",
            "Autodesk Maya",
            "Autodesk 3ds Max",
            "Cinema 4D",
            "ZBrush",
            "Houdini",
            "SketchUp",
            "Modo",
            "LightWave 3D",
            "Rhinoceros (Rhino)",
            "Substance Painter",
            "Unreal Engine",
            "Unity",
            "Marmoset Toolbag"
        });

        public async Task OnGetAsync()
        {
            await RefreshInfo();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            ModelState.Remove(nameof(CommentText));

            Post.UserId = _userManager.GetUserId(User);
            Post.CreatedDate = DateTime.Now;

            if (!ModelState.IsValid || _context.Posts == null)
            {
                await RefreshInfo();
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

            if (Post.UserId == null)
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
                await RefreshInfo();
                return Page();
            }

            if (CommentText.Length > 300)
            {
                ModelState.AddModelError(nameof(CommentText), "O comentário não pode ter mais de 300 caracteres.");
                await RefreshInfo();
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

            await RefreshInfo();

            var follow = _context.FollowedPosts.FirstOrDefault(p => p.PostId == id && p.UserId == userId);

            if(follow == null)
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

        private async Task RefreshInfo()
        {
            Posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.Likes)
                    .ThenInclude(l => l.User)
                .Include(f => f.Followers)
                    .ThenInclude(p => p.User)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            LPosts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.Likes)
                    .ThenInclude(l => l.User)
                .Include(f => f.Followers)
                    .ThenInclude(p => p.User)
                .OrderByDescending(p => p.Likes.Count())
                .Take(TopCount)
                .ToListAsync();

            FPosts = await _context.FollowedPosts
                .Where(fp => fp.UserId == _userManager.GetUserId(User))
                .Include(fp => fp.Post)
                    .ThenInclude(p => p.User)
                .Include(fp => fp.Post)
                    .ThenInclude(p => p.Comments)
                .Include(fp => fp.Post)
                    .ThenInclude(p => p.Likes)
                .Where(fp => fp.Post != null)
                .Select(fp => fp.Post!)
                .OrderByDescending(p => p.Likes.Count)
                .ToListAsync();
        }
    }
}
