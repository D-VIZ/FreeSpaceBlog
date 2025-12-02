using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreeSpace.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        [NotMapped]
        public IFormFile? Photo { get; set; }

        public string? PhotoPath { get; set; } = "/userDefault.jpg";
    }
}
