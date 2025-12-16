using FreeSpace.Data;
using FreeSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FreeSpace.Pages.Posts.Tutoriais
{
    public class TutoriaisModel : PageModel
    {
        private readonly FreeSpace.Data.AppDbContext _context;

        public TutoriaisModel(AppDbContext context)
        {
            _context = context;
        }

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

        public IList<Tutorials> tutoriais { get; set; } = default!;

        public async Task OnGetAsync()
        {
            tutoriais = await _context.Tutorials.OrderByDescending(t => t.CreatedDate).ToListAsync();
        }

        public async Task<IActionResult> OnPostFilter(string plataforma)
        {
            if(plataforma == "Não especificado")
            {
                tutoriais = await _context.Tutorials.OrderByDescending(t => t.CreatedDate).ToListAsync();
            }
            else
            {
                tutoriais = await _context.Tutorials.OrderByDescending(t => t.CreatedDate).Where(t => t.Plataform == plataforma).ToListAsync();
            }
            return Page();
        }
    }
}
