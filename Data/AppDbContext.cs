using FreeSpace.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreeSpace.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; } = default!;

        // Suas tabelas normais depois:
        // public DbSet<Filme> Filmes { get; set; }
    }
}
