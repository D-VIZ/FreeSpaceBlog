using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FreeSpace.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
