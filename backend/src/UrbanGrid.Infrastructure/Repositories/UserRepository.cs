using Microsoft.EntityFrameworkCore;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Interfaces;
using UrbanGrid.Infrastructure.Data;
using System.Linq.Expressions;

namespace UrbanGrid.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UrbanGridDbContext _ctx;
    public UserRepository(UrbanGridDbContext ctx) => _ctx = ctx;

    public async Task<User?> GetByEmailAsync(string email) =>
        await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _ctx.Users.FindAsync(id);

    public async Task<(IEnumerable<User> Items, int Total)> GetAllAsync(
        int page, int limit)
    {
        var total = await _ctx.Users.CountAsync();
        var items = await _ctx.Users
            .OrderBy(u => u.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
        return (items, total);
    }

    public async Task<User> CreateAsync(User user)
    {
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _ctx.Users.Update(user);
        await _ctx.SaveChangesAsync();
        return user;
    }

    public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate) =>
        await _ctx.Users.AnyAsync(predicate);

    // ✅ NEW — Delete user by id
    public async Task DeleteAsync(Guid id)
    {
        var user = await _ctx.Users.FindAsync(id);
        if (user != null)
        {
            _ctx.Users.Remove(user);
            await _ctx.SaveChangesAsync();
        }
    }
}
