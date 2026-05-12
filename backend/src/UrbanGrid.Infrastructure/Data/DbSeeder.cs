using Microsoft.EntityFrameworkCore;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(UrbanGridDbContext db)
    {
        if (!await db.Users.AnyAsync(u => u.Role == UserRole.ADMIN))
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Name = "System Administrator",
                Email = "admin@urbangrid.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
                Role = UserRole.ADMIN,
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.Users.Add(admin);
            await db.SaveChangesAsync();
            Console.WriteLine("✅ Admin user seeded: admin@urbangrid.com / Admin@1234");
        }
        else
        {
            Console.WriteLine("ℹ️ Admin already exists — skipping seed.");
        }
    }
}
