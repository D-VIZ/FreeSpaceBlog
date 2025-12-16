using FreeSpace;
using FreeSpace.Data;
using FreeSpace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace FreeSpace.Pages.Posts.Tutoriais
{
    public class CreateModel : PageModel
    {
        private readonly FreeSpace.Data.AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tutorials Tutoriais { get; set; } = default!;

        public SelectList Plataformas = new SelectList(new List<string> {"Não especificado",
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
            "Marmoset Toolbag" });

        public void OnGetAsync()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            if (Tutoriais.Media != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Tutoriais.Media.FileName);
                var filePath = Path.Combine("wwwroot/upfiles/" + fileName);

                using var stream = System.IO.File.Create(filePath);
                await Tutoriais.Media.CopyToAsync(stream);

                Tutoriais.FilePath = "/upfiles/" + fileName;
            }
            Tutoriais.CreatedDate = DateTime.Now;

            var tutorial = _context.Tutorials.Add(Tutoriais);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
