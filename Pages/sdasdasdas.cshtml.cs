using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreeSpace.Pages
{
    public class IndexModela : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModela(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Posts/Index");
        }
    }
}
