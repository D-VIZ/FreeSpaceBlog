using FreeSpace.Data;
using FreeSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FreeSpace.Pages.Posts.Tutoriais
{
    public class EditModel : PageModel
    {
        private readonly FreeSpace.Data.AppDbContext _context;

        public EditModel(AppDbContext context)
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                 return NotFound();
            }

            var tutorial = await _context.Tutorials.FirstOrDefaultAsync(m => m.Id == id);

            if(tutorial == null)
            {
                return NotFound();
            }

            Tutoriais = tutorial;

            return Page();
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
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

            _context.Attach(Tutoriais).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Manage");
        }
    }
}

