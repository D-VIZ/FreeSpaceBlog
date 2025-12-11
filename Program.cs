using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FreeSpace.Data;
using FreeSpace.Models;

namespace FreeSpace
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // logs no console
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // DbContext - verifique appsettings.json -> ConnectionStrings:DefaultConnection
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity com Roles
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                // Regras de senha mais relaxadas para facilitar testes locais (remova em produção)
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();

            // ============ MIGRATE + SEED ============
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    // Força aplicar migrações (útil em dev)
                    var db = services.GetRequiredService<AppDbContext>();
                    logger.LogInformation("Aplicando migrações (Database.Migrate)...");
                    db.Database.Migrate();
                    logger.LogInformation("Migrações aplicadas.");

                    // Seed roles + admin
                    await SeedRolesAndAdminAsync(services, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro durante migração/seed na inicialização: {Msg}", ex.Message);
                    if (ex.InnerException != null)
                        logger.LogError("Inner exception: {Inner}", ex.InnerException.Message);

                    // Re-throw opcional dependendo se você quer que a app pare
                    throw;
                }
            }
            // ============ FIM MIGRATE + SEED ============

            app.Run();
        }

        private static async Task SeedRolesAndAdminAsync(IServiceProvider services, ILogger logger)
        {
            // Obter managers
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Roles que você precisa
            string[] roleNames = { "Admin", "User" };

            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole(role));
                    if (r.Succeeded)
                    {
                        Console.WriteLine($"Role criada: {role}");
                        logger.LogInformation("Role criada: {Role}", role);
                    }
                    else
                    {
                        Console.WriteLine($"Erro ao criar role {role}: {string.Join(", ", r.Errors.Select(e => e.Description))}");
                        logger.LogError("Erro ao criar role {Role}: {Errors}", role, string.Join(", ", r.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    Console.WriteLine($"Role já existe: {role}");
                    logger.LogInformation("Role já existe: {Role}", role);
                }
            }

            // Config do admin (troque para secrets / config em produção)
            var adminEmail = "diohadmin@greycube.com";
            var adminPass = "Admin963121@D";

            Console.WriteLine("Verificando usuário admin...");
            logger.LogInformation("Verificando usuário admin...");

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "DiohADM",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Administrador Geral",
                };

                var createResult = await userManager.CreateAsync(user, adminPass);
                if (!createResult.Succeeded)
                {
                    Console.WriteLine("Falha ao criar admin. Erros:");
                    logger.LogError("Falha ao criar admin. Erros:");
                    foreach (var err in createResult.Errors)
                    {
                        Console.WriteLine($" - {err.Code}: {err.Description}");
                        logger.LogError(" - {Code}: {Desc}", err.Code, err.Description);
                    }

                    // Se a criação falhar por causa de um problema de EF (DbUpdateException)
                    // ele será lançado no bloco de chamada (Main) e logado lá.
                    return;
                }

                Console.WriteLine($"Admin criado com sucesso: {adminEmail}");
                logger.LogInformation("Admin criado com sucesso: {Email}", adminEmail);

                var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!addRoleResult.Succeeded)
                {
                    Console.WriteLine("Falha ao atribuir role Admin ao usuário:");
                    foreach (var err in addRoleResult.Errors)
                        Console.WriteLine($" - {err.Code}: {err.Description}");

                    logger.LogError("Falha ao atribuir role Admin ao usuário: {Errors}", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                }
                else
                {
                    Console.WriteLine("Role Admin atribuída ao usuário.");
                    logger.LogInformation("Role Admin atribuída ao usuário.");
                }
            }
            else
            {
                Console.WriteLine("Admin já existe no banco: " + adminEmail);
                logger.LogInformation("Admin já existe no banco: {Email}", adminEmail);

                if (!await userManager.IsInRoleAsync(user, "Admin"))
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine("Role Admin atribuída ao usuário existente.");
                    logger.LogInformation("Role Admin atribuída ao usuário existente.");
                }
            }
        }
    }
}
